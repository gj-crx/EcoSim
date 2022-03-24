using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{
    public GM gm;
    public GameObject MainMenuGUI;
    public GameObject InGameGUI;
    public GameObject MissionsPanel;
    public GameObject MainPanel;
    public GameObject LocalizationSelectionPanel;
    public GameObject InGameSettingsPanel;

    public Text[] IndicatorsTexts;
    public bio ClickedBio = null;
    public GameObject[] MissionCompleteCheckmarks = new GameObject[5];
    public GameObject[] MissionsIndicators = new GameObject[5];

    public bool IsTutorial = false;
    public GameObject[] TutorialHints = new GameObject[10];
    public GameObject[] MissionHints = new GameObject[10];
    public int TutorialChapter = 0;
    public GameObject[] HintFlashLights = new GameObject[5];

    [SerializeField]
    private Slider GameSpeedSlider;
    void Start()
    {
        gm = GameObject.Find("GM").GetComponent<GM>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void CloseButtonMissionsPanel()
    {
        MissionsPanel.SetActive(false);
        MainPanel.SetActive(true);
    }
    public void OpenMissionPanel()
    {
        MissionsPanel.SetActive(true);
        MainPanel.SetActive(false);
    }
    public void CreaturesButtonClicked()
    {
        if (IsTutorial && TutorialChapter == 1)
        {
            HintFlashLights[0].SetActive(false);
            TutorialHints[3].SetActive(true);
            TutorialChapter = 2;
        }
    }
    public void OpenOptionsPanel()
    {
        MainPanel.SetActive(false);
        LocalizationSelectionPanel.SetActive(true);
    }
    public void CloseInGameSettings()
    {
        InGameSettingsPanel.SetActive(false);
    }
    public void OpenInGameSettings()
    {
        if (InGameSettingsPanel.activeInHierarchy == false)
        {
            InGameSettingsPanel.SetActive(true);
        }
        else
        {
            InGameSettingsPanel.SetActive(false);
        }
    }
    public void ExitToMenuButton()
    {
        InGameSettingsPanel.SetActive(false);
        InGameGUI.SetActive(false);
        MainMenuGUI.SetActive(true);
        gm.MissionIsStarted = false;
    }
    public void GameSpeedSliderChange()
    {
        GameSpeedSlider.transform.Find("ValueText").GetComponent<Text>().text = "x" + GameSpeedSlider.value.ToString().Substring(0, Mathf.Min(GameSpeedSlider.value.ToString().Length, 4));
        Time.timeScale = GameSpeedSlider.value;
    }
    public void ExitGameButton()
    {
        Debug.Log("RIP");
        Application.Quit();
    }
}
