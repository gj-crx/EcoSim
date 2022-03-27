using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Plant : MonoBehaviour
{
    public bio l;
    public GameObject prefab;
    public float RandomRadius = 6;
    public float PlantHeight = 5;
    public float EnergyEfficiency = 2.5f;
    public float EnergyAfterBreed = 20;

    public int SunQuad = 0;

    public GM gm;

    void Start()
    {
        Init();
    }
    public void Init()
    {
        gm = GameObject.Find("GM").GetComponent<GM>();
        l = GetComponent<bio>();
        if (l.HabitationType == 1)
        {
            gm.AddNewUnit(gameObject, true, false);
        }
        else
        {
            gm.AddNewUnit(gameObject, true, true);
        }
        SunQuad = (int)(transform.position - gm.g1.WorldBottomBorder).y / 10 * 10;
        SunQuad += (int)(transform.position - gm.g1.WorldBottomBorder).x / 10;
    }

    // Update is called once per frame
    void Update()
    {
        if (l.energy > 95)
        {
            PlantBreed();
        }
    }
    public void PlantBreed()
    {
        l.energy = EnergyAfterBreed;
        GameObject n = Instantiate(prefab);
        Vector3 NewPos = transform.position + new Vector3(Random.Range(-RandomRadius, RandomRadius), Random.Range(-RandomRadius, RandomRadius), 0);
        int Counter = 0;
        if (l.HabitationType == 0)
        {
            while (gm.tc.WalkableTile(NewPos) == false && Counter < 6)
            {
                NewPos = transform.position + new Vector3(Random.Range(-RandomRadius, RandomRadius), Random.Range(-RandomRadius, RandomRadius), 0);
                Counter++;
            }
        }
        if (l.HabitationType == 1)
        {
            while (gm.tc.ZeroTilemapLevel.GetTile<Tile>(new Vector3Int((int)NewPos.x, (int)NewPos.y, 0)) == null && Counter < 6)
            {
                NewPos = transform.position + new Vector3(Random.Range(-RandomRadius, RandomRadius), Random.Range(-RandomRadius, RandomRadius), 0);
                Counter++;
            }
        }
        if (l.HabitationType == 1)
        {
            while (gm.tc.WalkableTile(NewPos) == false && gm.tc.ZeroTilemapLevel.GetTile<Tile>(new Vector3Int((int)NewPos.x, (int)NewPos.y, 0)) == false && Counter < 6)
            {
                NewPos = transform.position + new Vector3(Random.Range(-RandomRadius, RandomRadius), Random.Range(-RandomRadius, RandomRadius), 0);
                Counter++;
            }
        }
        if (Counter == 6) return; //не найдено
        n.transform.position = NewPos;
        if (n.transform.position.x < gm.g1.WorldBottomBorder.x)
        {
            n.transform.position += new Vector3(gm.g1.WorldBottomBorder.x + 3, 0, 0);
        }
        if (n.transform.position.y < gm.g1.WorldBottomBorder.y)
        {
            n.transform.position += new Vector3(0, gm.g1.WorldBottomBorder.y + 3, 0);
        }
        if (n.transform.position.x > gm.g1.WorldBottomBorder.x + gm.g1.xSize)
        {
            n.transform.position = new Vector3(gm.g1.WorldBottomBorder.x + gm.g1.xSize - 4, n.transform.position.y, 0);
        }
        if (n.transform.position.y > gm.g1.WorldBottomBorder.y + gm.g1.ySize)
        {
            n.transform.position = new Vector3(n.transform.position.x, gm.g1.WorldBottomBorder.y + gm.g1.ySize - 4, 0);
        }
        n.GetComponent<bio>().BioID = l.BioID;
        n.GetComponent<bio>().Age = 0;
    }
    
}
