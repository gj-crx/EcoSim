using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    public GM gm;
    public Gen1 g1;
    public int QuadSize = 10;

    public int[] CountPlantsPerQuad;
    public int[] CountPlantsPerQuadThisCycle;
    public float SunCycleTime = 10;
    public float EnergyPerQuad = 30;

    private float timerCycles = 0;
    void Start()
    {
        CountPlantsPerQuadThisCycle = new int[g1.ySize];
        CountPlantsPerQuad = new int[g1.ySize];
    }

    // Update is called once per frame
    void Update()
    {
        if (timerCycles > SunCycleTime)
        {
            timerCycles = 0;
            Photosynthesis();
        }
        else timerCycles += Time.deltaTime;
    }
    public void Photosynthesis()
    {
        CountPlantsPerQuad = CountPlantsPerQuadThisCycle;
        CountPlantsPerQuadThisCycle = new int[g1.ySize];
        foreach (GameObject g in gm.UnitsBiologicalAll)
        {
            if (g != null && g.tag == "plant")
            {
                Plant p = g.GetComponent<Plant>();
                CountPlantsPerQuadThisCycle[p.SunQuad]++;
                if (CountPlantsPerQuad[p.SunQuad] > 3)
                {
                   p.l.energy += EnergyPerQuad / Mathf.Max(1, CountPlantsPerQuad[p.SunQuad]) / 2;
                }
                else
                {
                    p.l.energy += EnergyPerQuad / Mathf.Max(1, CountPlantsPerQuad[p.SunQuad]);
                }
            }
        }
    }
}
