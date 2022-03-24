using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Hint : MonoBehaviour, IPointerClickHandler
{
    [TextArea] public string OptionalHintContainer = "empty";
    public bool IsFadingAway = false;
    public bool TutorialHint = false;
    public int TutorialHintID = 0;
    public bool ChainHint = false;
    public bool LastHint = true;
    public GameObject hintText;
    public GameObject HintFlashLight = null;
    public float LifeTimeOfHint = 5f;
    private float timerLifeTime = 0;
    private GUI gui;
    void Start()
    {
        gui = GameObject.Find("GUI").GetComponent<GUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsFadingAway)
        {
            if (timerLifeTime > LifeTimeOfHint)
            {
                timerLifeTime = 0;
                gameObject.SetActive(false);
            }
            else timerLifeTime += Time.deltaTime;
        }
    }
    public void HintButtonClicked()
    {
        hintText.SetActive(!hintText.activeInHierarchy);
        if (OptionalHintContainer != "empty")
        {
            hintText.transform.Find("Text").GetComponent<Text>().text = OptionalHintContainer;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (TutorialHint)
        {
            gameObject.SetActive(false);
            try
            {
                Hint h = gui.TutorialHints[TutorialHintID + 1].GetComponent<Hint>();
                if (h != null)
                {
                    if (ChainHint)
                    {
                        RemoveAllFlashlights();
                        h.gameObject.SetActive(true);
                    }
                    if (h.HintFlashLight != null)
                    {
                        h.HintFlashLight.SetActive(true);
                    }
                }
                if (TutorialHintID == 2) gui.TutorialChapter = 1;
                if (TutorialHintID == 8) gui.TutorialChapter = 3;
                if (TutorialHintID == 9) gui.TutorialChapter = 5;
            }
            catch
            {
                Debug.Log("Неправильный формат оформления подсказок для туториала");
            }
        }
        else
        {
            if (IsFadingAway)
            {
                gameObject.SetActive(false);
            }
        }
        
    }
    public void RemoveAllFlashlights()
    {
        if (gui == null)
        {
            gui = GameObject.Find("GUI").GetComponent<GUI>();
        }
        for (int i = 0; i < gui.HintFlashLights.Length; i++)
        {
            if (gui.HintFlashLights[i] != null)
            {
                gui.HintFlashLights[i].SetActive(false);
            }
        }
    }
    
}
