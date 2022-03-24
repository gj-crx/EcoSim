using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gen2 : MonoBehaviour
{
    public bool[,] Map = new bool[100, 100];
    public int MapSize = 100;
    public Vector3Int WorldBottomPoint = new Vector3Int(0, 0, 0);
    public TileController tc;
    public int MinimalGroundToGenerate = 200;
    public int GroundGenerated = 0;

    void Start()
    {
        tc = GetComponent<TileController>();
        GenerateSomeLand();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GenerateSomeLand(int IslandCount = 10)
    {
        Map = new bool[MapSize, MapSize];
        int count = 0;
        while (GroundGenerated < MinimalGroundToGenerate ||  count < 10)
        {
            count++;
            int RandomPosX = Random.Range(0, MapSize) + WorldBottomPoint.x;
            int RandomPosY = Random.Range(0, MapSize) + WorldBottomPoint.y;
            GenerateIsland(RandomPosX, RandomPosY, Random.Range(14, 100));
        }
    }
    public void GenerateIsland(int xPos, int yPos, int IslandEstimatedSize)
    {
        int GeneratedCurrent = 0;
        int radius = (int)Mathf.Sqrt(IslandEstimatedSize);
        bool Generating = true;
        int x = 0; int y = 0;
        while (Generating)
        {
            if (xPos + x > 0 && xPos + x < MapSize && yPos + y > 0 && yPos + y < MapSize)
            {
                if (Map[xPos + x, yPos + y] == false)
                {
                    float rnd = Random.Range(0.0f, 1.0f);
                    float chance = (1 - GeneratedCurrent / IslandEstimatedSize) + 0.5f;
                    if (rnd < chance)
                    {

                        Map[xPos + x, yPos + y] = true;
                        tc.JustPlace(WorldBottomPoint + new Vector3Int(xPos + x, yPos + y, 0));
                        GroundGenerated++;
                        GeneratedCurrent++;
                    }
                    else return;
                }
            }
            //+ -
            if (xPos + x > 0 && xPos + x < MapSize && yPos - y > 0 && yPos - y < MapSize)
            {
                if (Map[xPos + x, yPos - y] == false)
                {
                    float rnd = Random.Range(0.0f, 1.0f);
                    float chance = (1 - GeneratedCurrent / IslandEstimatedSize) + 0.5f;
                    if (rnd < chance)
                    {

                        Map[xPos + x, yPos - y] = true;
                        tc.JustPlace(WorldBottomPoint + new Vector3Int(xPos + x, yPos - y, 0));
                        GroundGenerated++;
                        GeneratedCurrent++;
                    }
                    else return;
                }
            }
            //- +
            if (xPos - x > 0 && xPos - x < MapSize && yPos + y > 0 && yPos + y < MapSize)
            {
                if (Map[xPos - x, yPos + y] == false)
                {
                    float rnd = Random.Range(0.0f, 1.0f);
                    float chance = (1 - GeneratedCurrent / IslandEstimatedSize) + 0.5f;
                    if (rnd < chance)
                    {

                        Map[xPos - x, yPos + y] = true;
                        tc.JustPlace(WorldBottomPoint + new Vector3Int(xPos - x, yPos + y, 0));
                        GroundGenerated++;
                        GeneratedCurrent++;
                    }
                    else return;
                }
            }
            //- -
            if (xPos - x > 0 && xPos - x < MapSize && yPos - y > 0 && yPos - y < MapSize)
            {
                if (Map[xPos - x, yPos - y] == false)
                {
                    float rnd = Random.Range(0.0f, 1.0f);
                    float chance = (1 - GeneratedCurrent / IslandEstimatedSize) + 0.5f;
                    if (rnd < chance)
                    {

                        Map[xPos - x, yPos - y] = true;
                        tc.JustPlace(WorldBottomPoint + new Vector3Int(xPos - x, yPos - y, 0));
                        GroundGenerated++;
                        GeneratedCurrent++;
                    }
                    else return;
                }
            }
            if (Random.Range(1, 3) == 1) x++;
            else y++;
        }
    }
}
