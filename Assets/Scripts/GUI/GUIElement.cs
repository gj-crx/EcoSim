using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIElement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CloseParentElemenent()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
    }
}
