using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Tilemaps;
using UnityEngine;
using Pathfinding;
using System;

public class GM : MonoBehaviour
{
    public TileController tc;
    public GUI gui;
    public Gen1 g1;
    public SpeciesCreation sc;
    public FileWork fl;
    public Loc loc;
    public Sun sun;

    public bool AndroidRunning = true;
    public bool MissionIsStarted = false;
    /// <summary>
    /// первый разряд - массив разных типов ресурсов
    /// </summary>
    public GameObject[][] Resources = new GameObject[3][];
    public GameObject[] UnitsBiologicalLand = new GameObject[350];
    public GameObject[] UnitsBiologicalMarine = new GameObject[200];
    public GameObject[] UnitsBiologicalAll = new GameObject[650];
    public GameObject[] UnitsBuildings = new GameObject[0];

    [HideInInspector]
    public int[] ResourcesGeneratedCount = new int[2];

    
    public Gamedata gamedata;


    void Start()
    {
        Resources[1] = new GameObject[1000];
        Resources[2] = new GameObject[150];
        fl.LoadData();
        if (gamedata.SelectedLanguage == null || gamedata.SelectedLanguage == "nothing")
        {
            gui.MainPanel.SetActive(false);
            gui.LocalizationSelectionPanel.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
           
        }
    }
    public struct Gamedata
    {
        public bool[] MissionsCompleted;
        public string SelectedLanguage;
    }

    public void ApplyNewBuilding(Vector3 pos, int BuildingSize = 4)
    {
        if (BuildingSize == 1)
        {
            Vector3Int cp = new Vector3Int((int)pos.x, (int)pos.y, 0);
            tc.BuildingTiles[cp.x, cp.y] = true;
        }
        if (BuildingSize == 4)
        {
            Vector3Int cp = new Vector3Int((int)pos.x, (int)pos.y, 0);
            tc.BuildingTiles[cp.x, cp.y] = true;
            tc.BuildingTiles[cp.x - 1, cp.y] = true;
            tc.BuildingTiles[cp.x, cp.y - 1] = true;
            tc.BuildingTiles[cp.x - 1, cp.y - 1] = true;
        }
    }
    public void AddNewUnit(GameObject unit, bool BiologicalUnit = true, bool LandUnit = true)
    {
        if (BiologicalUnit)
        {
            for (int i = 0; i < UnitsBiologicalAll.Length; i++)
            {
                if (UnitsBiologicalAll[i] == null)
                {
                    UnitsBiologicalAll[i] = unit;
                    break;
                }
            }
            if (LandUnit)
            {
                for (int i = 0; i < UnitsBiologicalLand.Length; i++)
                {
                    if (UnitsBiologicalLand[i] == null)
                    {
                        UnitsBiologicalLand[i] = unit;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < UnitsBiologicalMarine.Length; i++)
                {
                    if (UnitsBiologicalMarine[i] == null)
                    {
                        UnitsBiologicalMarine[i] = unit;
                        break;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < UnitsBuildings.Length; i++)
            {
                if (UnitsBuildings[i] == null)
                {
                    UnitsBuildings[i] = unit;
                    break;
                }
            }
        }
    }
    
    
    public void StartMission(int missionID)
    {
        foreach (GameObject g in gui.MissionHints)
        {
            if (g != null)
            {
                g.SetActive(false);
            }
        }
        gui.MissionHints[missionID].SetActive(true);
        g1.BasicSettings.ApplySettings(g1);
        if (missionID == 0)
        {
           //особые условия миссии 0

        }
        if (missionID == 1)
        {
            g1.CreatureTypesAmountToSpawn[1] = 2;
            g1.CreatureTypesAmountToSpawn[5] = 1;
        }
        if (missionID == 2)
        {
            sun.EnergyPerQuad = 24;
            g1.CreatureTypesAmountToSpawn[0] = 17;
            g1.CreatureTypesAmountToSpawn[1] = 4;
            g1.CreatureTypesAmountToSpawn[4] = 14;
            g1.CreatureTypesAmountToSpawn[5] = 3;
        }
        if (missionID == 3)
        {
            sun.EnergyPerQuad = 65;
            g1.CreatureTypesAmountToSpawn[5] = 7;
            g1.CreatureTypesAmountToSpawn[6] = 5;
        }
        if (missionID == 4)
        {
            g1.MaxGroundCells = 2000;
        }
        sc.CurrentSpeciesSpawnedCount = 0;
        GetComponent<QuestTracking>().CurrentMission = missionID;
        g1.Generate();
        gui.MainMenuGUI.SetActive(false);
        gui.InGameGUI.SetActive(true);
        for (int i = 0; i < gui.MissionsIndicators.Length; i++)
        {
            if (gui.MissionsIndicators[i] != null)
            {
                gui.MissionsIndicators[i].SetActive(false);
            }
        }
        gui.MissionsIndicators[missionID].SetActive(true);
        MissionIsStarted = true;
        
    }
    public void StartFreePlay()
    {
        sc.CurrentSpeciesSpawnedCount = 0;
        GetComponent<QuestTracking>().CurrentMission = 100;
        g1.BasicSettings.ApplySettings(g1);
        for (int i = 0; i < g1.CreatureTypesAmountToSpawn.Length; i++)
        {
            g1.CreatureTypesAmountToSpawn[i] = 0;
        }
        g1.Generate();
        gui.MainMenuGUI.SetActive(false);
        gui.InGameGUI.SetActive(true);
        for (int i = 0; i < gui.MissionsIndicators.Length; i++)
        {
            if (gui.MissionsIndicators[i] != null)
            {
                gui.MissionsIndicators[i].SetActive(false);
            }
        }
        MissionIsStarted = false;
        sc.SpeciesSpawnedLimit = 999;
    }
    public void StartTutorial()
    {
        sc.CurrentSpeciesSpawnedCount = 0;
        gui.IsTutorial = true;
        gui.TutorialHints[0].SetActive(true);
        GetComponent<QuestTracking>().CurrentMission = 101;
        g1.Generate();
        gui.MainMenuGUI.SetActive(false);
        gui.InGameGUI.SetActive(true);
        for (int i = 0; i < gui.MissionsIndicators.Length; i++)
        {
            if (gui.MissionsIndicators[i] != null)
            {
                gui.MissionsIndicators[i].SetActive(false);
            }
        }
        MissionIsStarted = false;
        g1.BasicSettings.ApplySettings(g1);
        sc.SpeciesSpawnedLimit = 25;
    }
}
