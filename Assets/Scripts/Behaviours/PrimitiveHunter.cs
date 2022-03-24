using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;

public class PrimitiveHunter : MonoBehaviour
{
    /// <summary>
    /// 0 - no actions, 1 - hunting, 2 - resting, 3 - running away, 4 - migrating, 5 - grazing
    /// </summary>
    public string CurrentAction = "nothing";
    [HideInInspector]
    public GM gm;
    public Fighting f;
 //   public AIMovement am;
    AIDestinationSetter ads;
    AIBase ab;

    public bio l;
    [HideInInspector]
    public GameObject Target;
    public float SearchingInterval = 5;
    public bool IsResting;
    public float RestingModeEnergyCap = 70;
    public float DefendingReactionTime = 5;
    [HideInInspector]
    public GameObject LastAttacker = null;

    public bool ManuallyControlled = false;


    private float TimerSearching = 0;
    private float TimerDefendingReaction = 0;

    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (ManuallyControlled)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                //   ap.destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //seeker.StartPath(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), OnPathComplete);

            }
            return;
        }
        if (TimerSearching > SearchingInterval)
        {
            TimerSearching = 0;
            ads.target = null;
            if (l.energy > RestingModeEnergyCap || l.IsAdult == false)
            {
                IsResting = true;
                l.Pregnant = true;
                RestingAlgorythm();
                return;
            }
            IsResting = false;
        //    am.IsRunningAway = false;
            if (CheckForDanger() == true) return;
            if (l.FoodType == 1 || l.FoodType == 2)
            {
                SearchForPrey();
            }
            if (l.FoodType == 0)
            {
                SearchForFoodOmnivore();
            }
            if (Target != null)
            {
                f.Target = Target;
                f.Attacking = true;
                ads.target = Target.transform;
                if (Target.GetComponent<PrimitiveHunter>() != null)
                {
                    Target.GetComponent<PrimitiveHunter>().LastAttacker = gameObject;
                }
            }
            else
            {
                Migrate();
            }
        }
        else
        {
            TimerSearching += Time.deltaTime;
        }
        if (TimerDefendingReaction > DefendingReactionTime)
        {
            TimerDefendingReaction = 0;
            if (LastAttacker != null)
            {
                if (l.AgressionToAgression)
                {
                    Target = LastAttacker;
                    f.Target = Target;
                    f.Attacking = true;
                    ads.target = Target.transform;
                }
                else
                {
                    RunAway(LastAttacker.transform.position);
                }
                LastAttacker = null;
            }
        }
        else TimerDefendingReaction += Time.deltaTime;

        
    }
    public void Init()
    {
        gm = GameObject.Find("GM").GetComponent<GM>();
        if (l.HabitationType == 1)
        {
            gm.AddNewUnit(gameObject, true, false);
        }
        else
        {
            gm.AddNewUnit(gameObject, true, true);
        }
        TimerSearching = Random.Range(0, SearchingInterval * 1.4f);
     //   am = GetComponent<AIMovement>();
        try
        {
            ab = GetComponent<AIBase>();
            ads = GetComponent<AIDestinationSetter>();
        }
        catch
        {

        }
        if (l.HabitationType == 0)
        {
            
        }
    }

    public void SearchForPrey()
    {
        Target = null;
        float MinDist = l.SenceRange;
        GameObject[] FoodGroup = null;
        if (l.HabitationType == 0) FoodGroup = gm.UnitsBiologicalLand;
        if (l.HabitationType == 1) FoodGroup = gm.UnitsBiologicalMarine;
        if (l.HabitationType == 2) FoodGroup = gm.UnitsBiologicalAll;
        foreach (GameObject g in FoodGroup)
        {
            if (g != null)
            {
                bio p = g.GetComponent<bio>();
                if (l.FoodType == 2)
                {
                    if (p.IsPlant && p.gameObject != gameObject && (p.BioID != l.BioID || l.BioID == 0))
                    {
                        float CurDist = Vector2.Distance(transform.position, p.transform.position);
                        if (CurDist < MinDist)
                        {
                            MinDist = CurDist;
                            Target = p.gameObject;
                        }
                    }
                }
                if (l.FoodType == 1)
                {
                    if (p.IsPlant == false && p.gameObject != gameObject && (p.BioID != l.BioID || l.BioID == 0))
                    {
                        float CurDist = Vector2.Distance(transform.position, p.transform.position);
                        if (CurDist < MinDist)
                        {
                            MinDist = CurDist;
                            Target = p.gameObject;
                        }
                    }
                }
                if (l.FoodType == 0)
                {
                    if (p.gameObject != gameObject && (p.BioID != l.BioID || l.BioID == 0))
                    {
                        float CurDist = Vector2.Distance(transform.position, p.transform.position);
                        if (CurDist < MinDist)
                        {
                            MinDist = CurDist;
                            Target = p.gameObject;
                        }
                    }
                }
            }
        }
        if (l.FoodType != 2)
        {
            CurrentAction = gm.loc.ActionsLocalized[1];
        }
        else
        {
            CurrentAction = gm.loc.ActionsLocalized[5];
        }
    }
    public void SearchForFoodOmnivore()
    {
        Target = null;
        float MinDist = l.SenceRange;
        GameObject[] FoodGroup = null;
        if (l.HabitationType == 0) FoodGroup = gm.UnitsBiologicalLand;
        if (l.HabitationType == 1) FoodGroup = gm.UnitsBiologicalMarine;
        if (l.HabitationType == 2) FoodGroup = gm.UnitsBiologicalAll;
        foreach (GameObject g in FoodGroup)
        {
            if (g != null)
            {
                bio p = g.GetComponent<bio>();
                if (p.gameObject != gameObject && (p.BioID != l.BioID || l.BioID == 0))
                {
                    float CurDist = Vector2.Distance(transform.position, p.transform.position);
                    if (CurDist < MinDist)
                    {
                        MinDist = CurDist;
                        Target = p.gameObject;
                    }
                }
            }
        }
        CurrentAction = gm.loc.ActionsLocalized[1];
    }
    public bool CheckForDanger()
    {
        if (l.AgressionToAgression) return false;
        float MinDist = l.FearRange;
        GameObject fearObject = null;
        GameObject[] DangerGroup = null;
        if (l.HabitationType == 0) DangerGroup = gm.UnitsBiologicalLand;
        if (l.HabitationType == 1) DangerGroup = gm.UnitsBiologicalMarine;
        if (l.HabitationType == 2) DangerGroup = gm.UnitsBiologicalAll;
        foreach (GameObject g in DangerGroup)
        {
            if (g != null)
            {
                bio p = g.GetComponent<bio>();
                if (l.FoodType == 1 || l.FoodType == 0)
                {

                }
                if (p.BioID != l.BioID && p.IsPlant == false && p.IsAdult)
                {
                    float CurDist = Vector2.Distance(transform.position, p.transform.position);
                    if (CurDist < MinDist)
                    {
                        MinDist = CurDist;
                        fearObject = p.gameObject;
                    }
                }
            }
        }
        if (fearObject != null)
        {
            RunAway(fearObject.transform.position);
            return true;
        }
        else
        {
            return false;
        }
    }
 
    public void RestingAlgorythm()
    {
        CurrentAction = gm.loc.ActionsLocalized[2];
        Target = null;
        float MinDist = l.SenceRange;
        foreach (GameObject g in gm.UnitsBiologicalLand)
        {
            if (g != null)
            {
                bio p = g.GetComponent<bio>();
                if (g != gameObject && p.IsPlant == false && p.IsAdult && (p.BioID != l.BioID || l.BioID == 0))
                {
                    float CurDist = Vector2.Distance(transform.position, p.transform.position);
                    if (CurDist < MinDist)
                    {
                        MinDist = CurDist;
                        Target = p.gameObject;
                    }
                }
            }
        }
        if (Target != null && MinDist < l.FearRange)
        {
            RunAway(Target.transform.position);
        }
        else
        {
            if (l.energy > RestingModeEnergyCap && l.IsAdult)
            {
                l.Breed();
                l.Pregnant = false;
            }
        }
    }
    
    public void Breed()
    {
        l.energy = l.EnergyAfterBreed;
        GameObject m = Instantiate(gameObject, transform.position + new Vector3(Random.Range(-4f, 4f), Random.Range(-4f, 4f), 0), Quaternion.identity);
        m.GetComponent<bio>().Age = 0;
    }
    public void RunAway(Vector3 Target)
    {
        ads.target = null;
        Vector3 away = transform.position - Target;
        float sum = Mathf.Abs(away.x) + Mathf.Abs(away.y);
        away = new Vector3(away.x / sum, away.y / sum, 0) * 16f + transform.position;
        Vector3Int NewPath = new Vector3Int((int)away.x, (int)away.y, 0);
        //     am.IsRunningAway = true;
        if (l.HabitationType == 0)
        {
            if (gm.tc.ZeroTilemapLevel.GetTile<Tile>(NewPath) == null)
            {
                ab.destination = away;
            }
            else
            {
                ab.destination = gm.g1.LandObjectSpawningPossiblePositions[Random.Range(0, gm.g1.LandObjectSpawningPossiblePositionsCount)];
            }
        }
        if (l.HabitationType == 1)
        {
            if (gm.tc.tl.GetTile<Tile>(NewPath) == null)
            {
                ab.destination = away;
            }
            else
            {
                //заменить на морские точки
                ab.destination = gm.g1.NavalObjectSpawningPossiblePositions[Random.Range(0, gm.g1.NavalObjectSpawningPossiblePositionsCount)];
            }
        }
        if (l.HabitationType == 2)
        {
            if (gm.tc.tl.GetTile<Tile>(NewPath) != null || gm.tc.ZeroTilemapLevel.GetTile<Tile>(NewPath) == null)
            {
                ab.destination = away;
            }
            else
            {
                ab.destination = gm.g1.LandObjectSpawningPossiblePositions[Random.Range(0, gm.g1.LandObjectSpawningPossiblePositionsCount)];
            }
        }
        CurrentAction = gm.loc.ActionsLocalized[3];
    }


    public void ChangePosition()
    {
        if (gm.tc.WalkableTile(transform.position) == false)
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                    if (x != 0 || y != 0)
                    {
                        if (gm.tc.WalkableTile(transform.position + new Vector3(x, y, 0)))
                        {
                            transform.position = transform.position + new Vector3(x, y, 0);
                            return;
                        }
                    }
            }
        }
    }
    public void Migrate()
    {
        if (l.HabitationType == 0 || l.HabitationType == 2)
        {
            ab.destination = gm.g1.LandObjectSpawningPossiblePositions[Random.Range(0, gm.g1.LandObjectSpawningPossiblePositionsCount)];
        }
        else
        {
            ab.destination = gm.g1.NavalObjectSpawningPossiblePositions[Random.Range(0, gm.g1.NavalObjectSpawningPossiblePositionsCount)];
        }
        CurrentAction = gm.loc.ActionsLocalized[4];
    }
}
