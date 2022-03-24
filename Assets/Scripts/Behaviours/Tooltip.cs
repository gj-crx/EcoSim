using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public float timeBeforeDisappear = 3;
    private float timerDisappear = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timerDisappear > timeBeforeDisappear)
        {
            timerDisappear = 0;
            gameObject.SetActive(false);
        }
        else timerDisappear += Time.deltaTime;
    }

}
