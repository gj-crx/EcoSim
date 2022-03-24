using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Pathfinding;

public class SpeciesCreation : MonoBehaviour
{
    GUI gui;
    public int Points = 5;
    public bio CurrentModifiedBio;
    public List<int> CustomCreaturesBioIDs = new List<int>();

    public string[] SavedSpecies = new string[100];

    public int SpeciesSpawnedLimit = 5;
    public int CurrentSpeciesSpawnedCount = 0;
    public string CurrentSelectedSpeciesToSpawnName = "Unknown creature";

    public int ModelSelected = 0;
    public Sprite[] Models = new Sprite[2];

    public float[] StatsPerPoint = new float[10];
    public int[] SpeciesTypesCost = new int[9];
    public int[] PreviousSpeciesTypeSelected = new int[4];
    public Toggle[] SpeciesTypeToggles = new Toggle[9];
    public Text PointsText;
    public Text[] StatTexts = new Text[4];

    public int SelectedCreaturePrefab = 0;
    public GameObject[] CreaturesPrefabs = new GameObject[5];
    public InputField SpeciesNameInput;
    public GameObject WarningCreaturesSpawningLimit;
    public GameObject WarningMissionComplete;
    public GameObject WarningNoCreaturesToSpawn;
    public Text WarningSelectedSpeciesToSpawn;
    public GameObject contentCreatedSpeciesList;
    public GameObject[] listSpeciesBars = new GameObject[100];

    public GameObject prefabCreatedSpecies;


    void Start()
    {
        gui = GetComponent<GUI>();
        LoadSpecies();
    }

    public void PositiveStatButtonClicked(int StatID)
    {
        if (Points > 0)
        {
            if (StatID == 0)
            {
                CurrentModifiedBio.Damage += StatsPerPoint[StatID];
                StatTexts[StatID].text = CurrentModifiedBio.Damage.ToString();
                Points--;
            }
            if (StatID == 1)
            {
                CurrentModifiedBio.HPMax += StatsPerPoint[StatID];
                CurrentModifiedBio.HPCurrent += StatsPerPoint[StatID];
                StatTexts[StatID].text = CurrentModifiedBio.HPCurrent.ToString();
                Points--;
            }
            if (StatID == 2)
            {
                CurrentModifiedBio.SenceRange += StatsPerPoint[StatID];
                StatTexts[StatID].text = CurrentModifiedBio.SenceRange.ToString();
                Points--;
            }
            if (StatID == 3)
            {
                CurrentModifiedBio.MaxAge += StatsPerPoint[StatID];
                StatTexts[StatID].text = CurrentModifiedBio.MaxAge.ToString();
                Points--;
            }
        }
        PointsUpdate();
    }
    public void NegativeStatButtonClicked(int StatID)
    {
        if (StatID == 0)
        {
            if (CurrentModifiedBio.Damage >= StatsPerPoint[StatID])
            {
                CurrentModifiedBio.Damage -= StatsPerPoint[StatID];
                StatTexts[StatID].text = CurrentModifiedBio.Damage.ToString();
                Points++;
            }
        }
        if (StatID == 1)
        {
            if (CurrentModifiedBio.HPMax >= StatsPerPoint[StatID])
            {
                CurrentModifiedBio.HPMax -= StatsPerPoint[StatID];
                CurrentModifiedBio.HPCurrent -= StatsPerPoint[StatID];
                StatTexts[StatID].text = CurrentModifiedBio.HPCurrent.ToString();
                Points++;
            }
        }
        if (StatID == 2)
        {
            if (CurrentModifiedBio.SenceRange >= StatsPerPoint[StatID])
            {
                CurrentModifiedBio.SenceRange -= StatsPerPoint[StatID];
                StatTexts[StatID].text = CurrentModifiedBio.SenceRange.ToString();
                Points++;
            }
        }
        if (StatID == 3)
        {
            if (CurrentModifiedBio.MaxAge >= StatsPerPoint[StatID])
            {
                CurrentModifiedBio.MaxAge -= StatsPerPoint[StatID];
                StatTexts[StatID].text = CurrentModifiedBio.MaxAge.ToString();
                Points++;
            }
        }
        PointsUpdate();
    }
    public void SpeciesTypeSelect(int TypeID)
    {
        if (TypeID == 1)
        {

            if (SpeciesTypeToggles[0].isOn)
            {
                if (Points >= SpeciesTypesCost[0])
                {
                    Points -= SpeciesTypesCost[0];
                    Points += SpeciesTypesCost[PreviousSpeciesTypeSelected[1]];
                    PreviousSpeciesTypeSelected[1] = 0;
                    CurrentModifiedBio.HabitationType = 0;
                }
                else SpeciesTypeToggles[0].SetIsOnWithoutNotify(false);
            }
            if (SpeciesTypeToggles[1].isOn)
            {
                if (Points >= SpeciesTypesCost[1])
                {
                    Points -= SpeciesTypesCost[1];
                    Points += SpeciesTypesCost[PreviousSpeciesTypeSelected[1]];
                    PreviousSpeciesTypeSelected[1] = 1;
                    CurrentModifiedBio.HabitationType = 2;
                }
                else SpeciesTypeToggles[1].SetIsOnWithoutNotify(false);
            }
            if (SpeciesTypeToggles[2].isOn)
            {
                if (Points >= SpeciesTypesCost[2])
                {
                    Points -= SpeciesTypesCost[2];
                    Points += SpeciesTypesCost[PreviousSpeciesTypeSelected[1]];
                    PreviousSpeciesTypeSelected[1] = 2;
                    CurrentModifiedBio.HabitationType = 1;
                }
                else SpeciesTypeToggles[2].SetIsOnWithoutNotify(false);
            }
        }
        if (TypeID == 2)
        {
            if (SpeciesTypeToggles[3].isOn)
            {
                if (Points >= SpeciesTypesCost[3])
                {
                    Points -= SpeciesTypesCost[3];
                    Points += SpeciesTypesCost[PreviousSpeciesTypeSelected[2]];
                    PreviousSpeciesTypeSelected[2] = 3;
                    CurrentModifiedBio.FoodType = 1;
                }
                else SpeciesTypeToggles[3].SetIsOnWithoutNotify(false);
            }
            if (SpeciesTypeToggles[4].isOn)
            {
                if (Points >= SpeciesTypesCost[4])
                {
                    Points -= SpeciesTypesCost[4];
                    Points += SpeciesTypesCost[PreviousSpeciesTypeSelected[2]];
                    PreviousSpeciesTypeSelected[2] = 4;
                    CurrentModifiedBio.FoodType = 2;
                }
                else SpeciesTypeToggles[4].SetIsOnWithoutNotify(false);
            }
            if (SpeciesTypeToggles[5].isOn)
            {
                if (Points >= SpeciesTypesCost[5])
                {
                    Points -= SpeciesTypesCost[5];
                    Points += SpeciesTypesCost[PreviousSpeciesTypeSelected[2]];
                    PreviousSpeciesTypeSelected[2] = 5;
                    CurrentModifiedBio.FoodType = 0;
                }
                else SpeciesTypeToggles[5].SetIsOnWithoutNotify(false);
            }
        }
        PointsUpdate();
    }
    public void PointsUpdate()
    {
        PointsText.text = Points.ToString();
    }
    public void CreateCreature()
    {
        CurrentModifiedBio.BioID = Random.Range(1001, 10000);
        gui.gm.fl.WriteBio(CurrentModifiedBio);
        for (int i = 0; i < SavedSpecies.Length; i++)
        {
            if (SavedSpecies[i] != null)
            {
                SavedSpecies[i] = CurrentModifiedBio.CreatureName;
                break;
            }
            if (i == SavedSpecies.Length - 1) return;
        }
        CreateNewSpeciesGUIPanel(CurrentModifiedBio);
    }
    public void CreateNewSpeciesGUIPanel(bio nb)
    {
        GameObject n = Instantiate(prefabCreatedSpecies);
        n.transform.SetParent(contentCreatedSpeciesList.transform);
        n.transform.Find("name").GetComponent<Text>().text = nb.CreatureName;
        n.transform.Find("damage").GetComponent<Text>().text = "Damage: " + nb.Damage.ToString();
        n.transform.Find("HP").GetComponent<Text>().text = "HP: " + nb.HPMax.ToString();
        n.transform.Find("MaxAge").GetComponent<Text>().text = "Max Age: " + nb.MaxAge.ToString();
        n.transform.Find("Icon").GetComponent<Image>().sprite = Models[nb.ModelID];
        if (nb.IsPlant == false)
        {
            if (nb.HabitationType == 0)
            {
                n.transform.Find("habitation").GetComponent<Text>().text = "Land animal";
            }
            if (nb.HabitationType == 1)
            {
                n.transform.Find("habitation").GetComponent<Text>().text = "Waterfowl animal";
            }
            if (nb.HabitationType == 2)
            {
                n.transform.Find("habitation").GetComponent<Text>().text = "Amphibious animal";
            }
            if (nb.FoodType == 0)
            {
                n.transform.Find("food type").GetComponent<Text>().text = "Omnivore";
            }
            if (nb.FoodType == 1)
            {
                n.transform.Find("food type").GetComponent<Text>().text = "Carnivore";
            }
            if (nb.FoodType == 2)
            {
                n.transform.Find("food type").GetComponent<Text>().text = "Herbivore";
            }
        }
        else
        {
            if (nb.HabitationType == 0)
            {
                n.transform.Find("habitation").GetComponent<Text>().text = "Land plant";
            }
            if (nb.HabitationType == 1)
            {
                n.transform.Find("habitation").GetComponent<Text>().text = "Water plant";
            }
            if (nb.HabitationType == 2)
            {
                n.transform.Find("habitation").GetComponent<Text>().text = "Amphibious plant";
            }
            n.transform.Find("food type").GetComponent<Text>().text = "Photosynthetic";
        }
        n.GetComponent<IndicatorElement>().CreatureName = nb.CreatureName;
        WarningSelectedSpeciesToSpawn.text = "Selected: " + nb.CreatureName;
        CurrentSelectedSpeciesToSpawnName = nb.CreatureName;
        for (int i = 0; i < listSpeciesBars.Length; i++)
        {
            if (listSpeciesBars[i] == null)
            {
                listSpeciesBars[i] = n;
                break;
            }
        }
    }
    public void LoadSpecies()
    {
        int count = 0;
        string path = gui.gm.fl.SpeciesDataPath;
        if (gui.gm.AndroidRunning)
        {
            path = Application.persistentDataPath + "/" + path;
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
        }
        foreach (string p in Directory.GetFiles(path))
        {
            SavedSpecies[count] = p;
            CreateNewSpeciesGUIPanel(gui.gm.fl.LoadBio(p));
            count++;
        }
    }
    public void SpawnCreature()
    {
        if (File.Exists(Application.persistentDataPath + "/" + gui.gm.fl.SpeciesDataPath + CurrentSelectedSpeciesToSpawnName + ".txt") == false)
        {
            WarningNoCreaturesToSpawn.SetActive(true);
            return;
        }
        if (CurrentSpeciesSpawnedCount < SpeciesSpawnedLimit)
        {
            CurrentSpeciesSpawnedCount++;
        }
        else
        {
            WarningCreaturesSpawningLimit.SetActive(true);
            return;
        }
        GameObject n = Instantiate(CreaturesPrefabs[SelectedCreaturePrefab], Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, Screen.height / 2)) + new Vector3(0, 0, 1), Quaternion.identity);
        bio newbio = n.GetComponent<bio>();
        newbio.CopyBio(gui.gm.fl.LoadBio(Application.persistentDataPath + "/" + gui.gm.fl.SpeciesDataPath + CurrentSelectedSpeciesToSpawnName + ".txt"));
        n.GetComponent<SpriteRenderer>().color = newbio.color;
        n.GetComponent<SpriteRenderer>().sprite = Models[newbio.ModelID]; 
        if (newbio.HabitationType == 1)
        {
            n.layer = 4;
        }
        if (newbio.CreatureName != "")
        {
            n.gameObject.name = newbio.CreatureName + " breed0";
        }
        else n.gameObject.name = "Custom creature breed0";
        CustomCreaturesBioIDs.Add(newbio.BioID);
    }
    public void SetName()
    {
        CurrentModifiedBio.CreatureName = SpeciesNameInput.text;
    }
}
