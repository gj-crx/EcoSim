using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestTracking : MonoBehaviour
{
    GM gm;
    /// <summary>
    /// 0-4 обычные миссии, 5 - туториал
    /// </summary>
    public int CurrentMission = 0;
    
    public float MissionConditionCheckTime = 3f;
    public float CurrentMissionConditionTime = 0;
    private float timerConditionCheck = 0;
    

    void Start()
    {
        gm = GetComponent<GM>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.MissionIsStarted)
        {
            if (timerConditionCheck > MissionConditionCheckTime)
            {
                timerConditionCheck = 0;
                if (CurrentMission == 5)
                {
                    int count = 0;
                    int max = 0;
                    foreach (int bID in gm.sc.CustomCreaturesBioIDs)
                    {
                        count = CountCreatures(bID);
                        if (count > max) max = count;
                    }
                    gm.gui.MissionsIndicators[5].transform.Find("Text").GetComponent<Text>().text = max.ToString() + " / 6";
                    if (max > 6)
                    { //миссия пройдена
                        CompleteMission();
                    }
                }
                if (CurrentMission == 0)
                {
                    int count = 0;
                    int max = 0;
                    foreach (int bID in gm.sc.CustomCreaturesBioIDs)
                    {
                        count = CountCreatures(bID);
                        if (count > max) max = count;
                    }
                    gm.gui.MissionsIndicators[0].transform.Find("Text").GetComponent<Text>().text = max.ToString() + " / 12";
                    if (max > 12)
                    { //миссия пройдена
                        CompleteMission();
                    }
                }
                if (CurrentMission == 1)
                {
                    int count = CountCreaturesAll();
                    if (count > 25)
                    { //миссия пройдена
                        if (CurrentMissionConditionTime > 35f)
                        {
                            CompleteMission();
                        }
                        else
                        {
                            CurrentMissionConditionTime += MissionConditionCheckTime;
                        }
                    }
                    else
                    {
                        CurrentMissionConditionTime = 0;
                    }
                    gm.gui.MissionsIndicators[CurrentMission].transform.Find("Text").GetComponent<Text>().text = count.ToString() + " / 25";
                    gm.gui.MissionsIndicators[CurrentMission].transform.Find("subindicator2_Time").Find("Text").GetComponent<Text>().text = CurrentMissionConditionTime.ToString() + " / 35";
                }
                if (CurrentMission == 2)
                {
                    int count = CountCreaturesAll();
                    if (count > 10)
                    { //миссия пройдена
                        if (CurrentMissionConditionTime > 90f)
                        {
                            CompleteMission();
                        }
                        else
                        {
                            CurrentMissionConditionTime += MissionConditionCheckTime;
                        }
                    }
                    else
                    {
                        CurrentMissionConditionTime = 0;
                    }
                    gm.gui.MissionsIndicators[CurrentMission].transform.Find("Text").GetComponent<Text>().text = count.ToString() + " / 10";
                    gm.gui.MissionsIndicators[CurrentMission].transform.Find("subindicator2_Time").Find("Text").GetComponent<Text>().text = CurrentMissionConditionTime.ToString() + " / 90";
                }
                if (CurrentMission == 3)
                {
                    int count = CountCreaturesAll();
                    if (count <= 0)
                    { //миссия пройдена
                        CompleteMission();
                    }
                    gm.gui.MissionsIndicators[CurrentMission].transform.Find("Text")
                        .GetComponent<Text>().text = count.ToString() + " / 0";
                }
                if (CurrentMission == 4)
                {
                    int count = CountCreaturesAll();
                    if (count > 25)
                    { //миссия пройдена
                        if (CurrentMissionConditionTime > 20f)
                        {
                            CompleteMission();
                        }
                        else
                        {
                            CurrentMissionConditionTime += MissionConditionCheckTime;
                        }
                    }
                    else
                    {
                        CurrentMissionConditionTime = 0;
                    }
                    gm.gui.MissionsIndicators[CurrentMission].transform.Find("Text").GetComponent<Text>().text = count.ToString() + " / 22";
                    gm.gui.MissionsIndicators[CurrentMission].transform.Find("subindicator2_Time").Find("Text").GetComponent<Text>().text = CurrentMissionConditionTime.ToString() + " / 20";
                }
                if (CurrentMission == 5)
                {
                    int count = 0;
                    int max = 0;
                    foreach (int bID in gm.sc.CustomCreaturesBioIDs)
                    {
                        count = CountCreatures(bID);
                        if (count > max) max = count;
                    }
                    gm.gui.MissionsIndicators[5].transform.Find("Text").GetComponent<Text>().text = max.ToString() + " / 6";
                    if (max > 5)
                    { //миссия пройдена
                        gm.gui.TutorialHints[13].SetActive(true);
                        CompleteMission();
                    }
                }
            }
            else timerConditionCheck += Time.deltaTime;
        }
    }
    public void CompleteMission()
    {
        gm.MissionIsStarted = false;
        gm.gamedata.MissionsCompleted[CurrentMission] = true;
        gm.fl.SaveData();
        gm.fl.LoadData();
        gm.sc.WarningMissionComplete.SetActive(true);
    }
    public int CountCreatures(int BioID)
    {
        int count = 0;
        foreach (GameObject g in gm.UnitsBiologicalAll)
        {
            if (g != null)
            {
                bio b = g.GetComponent<bio>();
                if (b.BioID == BioID) count++;
            }
        }
        return count;
    }
    public int CountCreaturesAll(bool IncludePlants = false)
    {
        int count = 0;
        foreach (GameObject g in gm.UnitsBiologicalAll)
        {
            if (g != null)
            {
                if (IncludePlants)
                {
                    count++;
                }
                else
                {
                    if (g.GetComponent<bio>().IsPlant == false)
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }
    public void TutorialButtonEndSpeciesEditor()
    {
        if (gm.gui.IsTutorial && gm.gui.TutorialChapter == 3)
        {
            CurrentMission = 5;
            gm.gui.TutorialChapter = 4;
            gm.gui.MissionsIndicators[5].SetActive(true);
            gm.gui.TutorialHints[9].SetActive(true);
        }
    }
    public void TutorialButtonSpawnCreature()
    {
        if (gm.gui.IsTutorial && gm.gui.TutorialChapter == 5)
        {
            gm.gui.TutorialChapter = 6;
            gm.MissionIsStarted = true;
            gm.gui.TutorialHints[10].SetActive(true);
            gm.gui.TutorialHints[10].GetComponent<Hint>().RemoveAllFlashlights();
        }
    }
    
}
