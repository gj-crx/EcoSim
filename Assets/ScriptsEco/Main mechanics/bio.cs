using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bio : MonoBehaviour
{ //сборник биологических характеристик существа, а так же немного его типизации
    public string CreatureName = "Unknown creature";
    public int ModelID = 0;
    public Color color = new Color(255, 255, 255);
    public float HPCurrent = 100;
    public float HPMax = 100;
    public float energy = 50;
    public float EnergyMaximum = 100;
    public float Damage = 20;
    public float AttackSpeed = 1.7f;
    public float AttackRange = 1.5f;
    public float MoveSpeed = 5;
    public float Regen = 0.5f;
    public float EnergyRegen = -0.5f;
    public float SenceRange = 15f;
    public float FearRange = 7;

    public float Age = 0;
    public float MaxAge = 400;
    public float AdultAge = 30;
    public float CurrentScale = 1.0f;
    public bool IsAdult = true;
    public float EnergyAfterBreed = 20;
    public float EnergyGivenToKiller = 35;
    public bool Pregnant = false;

    public float ReincarnateChance = 0.0f;
    public float ReincarnateTime = 30f;
    public bool Reincarnating = false;


    public float TargetCheckInterval = 2;
    public float IntervalGUIUpdate = 0.5f;


    //не биология
    /// <summary>
    /// номер биологического вида, 0 - животное не имеет видовой принадлежности и будет атаковать своих же
    /// </summary>
    public int BioID = 0;
    public bool IsPlant = false;
    public bool SimpleBreeder = false;
    /// <summary>
    /// 0 - land, 1 - marine, 2 - amphibious
    /// </summary>
    public int HabitationType = 0;
    /// <summary>
    /// 0 - omnivore, 1 - carnivore, 2 - herbivore
    /// </summary>
    public int FoodType = 0;

    //атрибуты поведения
    /// <summary>
    /// цели с этим атрибутом будут атаковать в ответ, без него - убегать при получении ударов
    /// </summary>
    public bool AgressionToAgression = true;
    public bool Experimental = false;

    private GameObject SelfPrefab;
    private bool IsClicked = false;
    Plant p = null;
    SpriteRenderer sprite;
    PrimitiveHunter ph;
    GUI gui;
    Color NormalColor;
    private float timerReincarnation = 0;
    private float timerGrowing = 0;
    private float timerGUIUPdate = 0;
    private float GrowingInterval = 6;
    private Vector3 NormalScale;

    void Start()
    {
        gui = GameObject.Find("GUI").GetComponent<GUI>();
        SelfPrefab = gameObject;
        
        try
        {
            p = GetComponent<Plant>();
            ph = GetComponent<PrimitiveHunter>();
            sprite = GetComponent<SpriteRenderer>();
            NormalColor = sprite.color;
        }
        catch
        {

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Experimental) return;

        Aging();
        if (IsClicked)
        {
            if (timerGUIUPdate > IntervalGUIUpdate)
            {
                timerGUIUPdate = 0;
                GUIUpdate();
            }
            else timerGUIUPdate += Time.deltaTime;
        }
        if (Reincarnating)
        {
            if (timerReincarnation > ReincarnateTime)
            {
                Reincarnating = false;
                p.gm.AddNewUnit(gameObject);
                sprite.color = NormalColor;
                HPCurrent = HPMax;
            }
            else timerReincarnation += Time.deltaTime;
            return;
        }
        if (HPCurrent < HPMax)
        {
            HPCurrent += Regen * Time.deltaTime;
        }
        if (EnergyRegen > 0)
        {
            if (energy < EnergyMaximum)
            {
                energy += EnergyRegen * Time.deltaTime;
            }
        }
        else
        {
            if (IsAdult)
            {
                if (energy > 0)
                {
                    energy += EnergyRegen * Time.deltaTime;
                }
                else
                {
                    Death();
                }
            }
        }
        if (energy >= EnergyMaximum - 2 && SimpleBreeder)
        {
            energy = EnergyAfterBreed;
            Breed();
        }
        
    }
    public void Death(GameObject killer = null)
    {
        bio killerbio = null;
        try
        {
            killerbio = killer.GetComponent<bio>();
        }
        catch
        {

        }
        if (killerbio != null)
        {
            killerbio.energy += EnergyGivenToKiller;
        }
        if (killerbio != null && ReincarnateChance > Random.Range(0.0f, 1.0f))
        {
            Reincarnating = true;
            sprite.color = new Color(17, 60, 2);
            if (IsPlant)
            {
                for (int i = 0; i < p.gm.UnitsBiologicalLand.Length; i++)
                {
                    if (p.gm.UnitsBiologicalLand[i] == gameObject)
                    {
                        p.gm.UnitsBiologicalLand[i] = null;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < ph.gm.UnitsBiologicalLand.Length; i++)
                {
                    if (ph.gm.UnitsBiologicalLand[i] == gameObject)
                    {
                        ph.gm.UnitsBiologicalLand[i] = null;
                        break;
                    }
                }
            }
            if (killer != null)
            {
                killer.GetComponent<PrimitiveHunter>().Target = null;
                killer.GetComponent<Fighting>().Target = null;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Breed()
    {
        energy = EnergyAfterBreed;
        GameObject n = Instantiate(SelfPrefab, transform.position, Quaternion.identity);
        bio nb = n.GetComponent<bio>();
        n.transform.position = transform.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f), 0);
        nb.Age = 0;
        nb.IsAdult = false;
        nb.CurrentScale = 0.4f;
        nb.NormalScale = transform.localScale;
        n.transform.localScale = nb.NormalScale * nb.CurrentScale;
        nb.energy = EnergyAfterBreed;
        nb.HPCurrent = nb.HPMax;
        n.name = gameObject.name.Substring(0, gameObject.name.Length - 1) + (int.Parse(gameObject.name.Substring(gameObject.name.Length - 1, 1)) + 1).ToString();
        
    }
    public void Aging()
    {
        Age += Time.deltaTime;
        if (IsAdult == false)
        {
            if (timerGrowing > GrowingInterval)
            {
                timerGrowing = 0;
                if (Age < AdultAge)
                {
                    CurrentScale = Mathf.Max(0.4f, Age / AdultAge);
                    transform.localScale = NormalScale * CurrentScale;
                    ph.ChangePosition();
                }
                else
                {
                    IsAdult = true;
                    CurrentScale = 1;
                    transform.localScale = NormalScale;
                    ph.ChangePosition();
                }
            }
            else timerGrowing += Time.deltaTime;
        }
        if (Age > MaxAge)
        {
            Death();
        }
    }
    private void OnMouseDown()
    {
        GUIUpdate();
        IsClicked = true;
        if (gui.ClickedBio != null)
        {
            gui.ClickedBio.IsClicked = false;
        }
        gui.ClickedBio = this;
    }
    public void GUIUpdate()
    {
        gui.IndicatorsTexts[0].text = CreatureName;
        gui.IndicatorsTexts[1].text = Damage.ToString().Substring(0, Mathf.Min(4, Damage.ToString().Length));
        gui.IndicatorsTexts[2].text = HPCurrent.ToString().Substring(0, Mathf.Min(5, HPCurrent.ToString().Length)) + " / " +
            HPMax.ToString().Substring(0, Mathf.Min(5, HPMax.ToString().Length));
        if (ph != null)
        {
            gui.IndicatorsTexts[3].text = ph.CurrentAction;
        }
        else
        {
            gui.IndicatorsTexts[3].text = "Growing";
        }
        gui.IndicatorsTexts[4].text = energy.ToString().Substring(0, Mathf.Min(4, energy.ToString().Length));
        gui.IndicatorsTexts[5].text = Age.ToString().Substring(0, Mathf.Min(4, Age.ToString().Length));
    }
    private void OnDestroy()
    {
        if (IsClicked)
        {
            gui.IndicatorsTexts[2].text = "Dead";
        }
    }
    public void CopyBio(bio b)
    {
        CreatureName = b.CreatureName;
        BioID = b.BioID;
        ModelID = b.ModelID;
        color = b.color;
        Damage = b.Damage;
        HPCurrent = b.HPCurrent;
        HPMax = b.HPMax;
        SenceRange = b.SenceRange;
        energy = b.energy;
        EnergyMaximum = b.EnergyMaximum;
        AttackSpeed = b.AttackSpeed;
        AttackRange = b.AttackRange;
        MoveSpeed = b.MoveSpeed;
        Regen = b.Regen;
        EnergyRegen = b.EnergyRegen;
        FearRange = b.FearRange;

        Age = b.Age;
        MaxAge = b.MaxAge;
        AdultAge = b.AdultAge;
        EnergyAfterBreed = b.EnergyAfterBreed;
        EnergyGivenToKiller = b.EnergyGivenToKiller;

        HabitationType = b.HabitationType;
        FoodType = b.FoodType;
        SimpleBreeder = b.SimpleBreeder;
        AgressionToAgression = b.AgressionToAgression;

        ReincarnateChance = b.ReincarnateChance;
        ReincarnateTime = b.ReincarnateTime;
        
        if (HabitationType == 1)
        {
            GetComponent<CapsuleCollider2D>().isTrigger = true;
        }
    }

}
