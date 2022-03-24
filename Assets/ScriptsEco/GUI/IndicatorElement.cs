using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IndicatorElement : MonoBehaviour, IPointerClickHandler
{
    public GameObject PreviewModel;
    public SpeciesCreation sc;
    Slider slider;
    public bool IsSelected = false;
    public string CreatureName = "Unknown creature";
    public string LocalizationPath = "";
    

    void Start()
    {
        sc = GameObject.Find("GUI").GetComponent<SpeciesCreation>();
        try
        {
            slider = GetComponent<Slider>();
        }
        catch
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //WIP
    }
    public void OnColorSliderChange(int colorNumber)
    {
        if (colorNumber == 1)
        {
            sc.CurrentModifiedBio.color = new Color(slider.value / 255, sc.CurrentModifiedBio.color.g, sc.CurrentModifiedBio.color.b);
        }
        if (colorNumber == 2)
        {
            sc.CurrentModifiedBio.color = new Color(sc.CurrentModifiedBio.color.r, slider.value / 255, sc.CurrentModifiedBio.color.b);
        }
        if (colorNumber == 3)
        {
            sc.CurrentModifiedBio.color = new Color(sc.CurrentModifiedBio.color.r, sc.CurrentModifiedBio.color.g, slider.value / 255);
        }
        PreviewModel.GetComponent<Image>().color = sc.CurrentModifiedBio.color;
    }
    public void ChangeModelButton(int Change)
    {
        if (Change == 1)
        {
            if (sc.ModelSelected + 1 >= sc.Models.Length)
            {
                sc.ModelSelected = 0;
            }
            else sc.ModelSelected++;
        }
        else
        {
            if (sc.ModelSelected - 1 < 0)
            {
                sc.ModelSelected = sc.Models.Length - 1;
            }
            else sc.ModelSelected--;
        }
        PreviewModel.GetComponent<Image>().sprite = sc.Models[sc.ModelSelected];
        sc.CurrentModifiedBio.ModelID = sc.ModelSelected;
    }
    public void PickSpecies()
    {
        sc.CurrentSelectedSpeciesToSpawnName = CreatureName;
        foreach (GameObject n in sc.listSpeciesBars)
        {
            if (n != null)
            {
                if (n.GetComponent<IndicatorElement>().IsSelected)
                {
                    n.GetComponent<IndicatorElement>().IsSelected = false;
                }
            }
        }
        IsSelected = true;
        sc.WarningSelectedSpeciesToSpawn.text = "Selected: " + CreatureName;
        DehighlightEveryone();
        transform.Find("outline").GetComponent<Image>().color = new Color((float)183 / 255, (float)185 / 255, (float)13 / 255, 255);
    }
    public void DehighlightEveryone()
    {
        for (int i = 0; i < sc.listSpeciesBars.Length; i++)
        {
            if (sc.listSpeciesBars[i] != null)
            {
                sc.listSpeciesBars[i].transform.Find("outline").GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1);
            }
        }
    }
    public void SelectLocalization()
    {
        GameObject.Find("GM").GetComponent<Loc>().ApplyLocalization(LocalizationPath);
    }

}
