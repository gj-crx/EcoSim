using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class SimpleActions : MonoBehaviour
{
    Seeker seeker;
    AIDestinationSetter ads;
    AIPath ap;
    AIBase ab;
    void Start()
    {
        seeker = GetComponent<Seeker>();
        ab = GetComponent<AIBase>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ab.destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
