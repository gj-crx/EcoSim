using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;


public class Gen3 : MonoBehaviour
{
    public bool[,] Map;
    public int MapSize = 100;

    public TileController tc;
    public GM gm;
    public Tilemap tl;
    public Tilemap BlockerLevel;
    public GameObject cam;
    public Vector3 CamStartingPosition;
    public float CamNormalSize = 11.5f;
    public GameObject NaturalObjects;
    public int xSize = 200;
    public int ySize = 200;
    public Vector3Int WorldBottomBorder;
    public int Continents = 3;
    public int RandomIslandsCount = 10;
    public int MaxGroundCells = 1000;
    public float MinimumOutOfMax = 0.5f;
    public float MainContinentChunkChance = 0.8f;
    public float SecondContinentChunkChance = 0.4f;
    public float ContinentEnlargementChance = 0.3f;

    public float[] ResourceGenerationChances = new float[1];
    public GameObject[] ResourcesPrefabs = new GameObject[1];

    public bool DontSpawnAnything = false;
    public GameObject[] CreaturesToSpawn = new GameObject[25];
    public int[] CreatureTypesAmountToSpawn = new int[25];
    public float[] CreatureTypesAmountRandomizationFactor = new float[25];

    public Vector3[] LandObjectSpawningPossiblePositions = new Vector3[250];
    public int LandObjectSpawningPossiblePositionsCount = 0;
    public Vector3[] NavalObjectSpawningPossiblePositions = new Vector3[250];
    public int NavalObjectSpawningPossiblePositionsCount = 0;


    public bool SpreadCellsMaximum = true;

    public int GroundCellsGenerated = 0;

    public bool SuccesfullyGenerated = true;
    public bool OrderToGenerate = false;



    private float timer = 0;
    private bool Scanned = false;

    void Start()
    {
        tc = GetComponent<TileController>();
        gm = GetComponent<GM>();

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (Scanned == false)
        {
            if (timer > 0.35f)
            {
                AstarPath.active.Scan();
                Scanned = true;
            }
            else timer += Time.deltaTime;
        }
        if (OrderToGenerate)
        {
            OrderToGenerate = false;
            Generate();
        }
        if (SuccesfullyGenerated == false)
        {
            Debug.Log("?? ?????????????");
            MaxGroundCells -= 1000;
            xSize -= 35;
            ySize -= 35;
            Generate();
        }
    }
    public void Generate()
    {
        Map = new bool[MapSize, MapSize];
        tc.BuildingTiles = new bool[xSize, ySize];
        tc.ResourceSources = new sbyte[xSize, ySize];
        GroundCellsGenerated = 0;
        SuccesfullyGenerated = false;
        ActuallyPlaceAllTiles();

        tl.ClearAllTiles();
        tc.tl.ClearAllTiles();
        RemoveAllObjects();
        //????????? ??? ?????
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                tl.SetTile(WorldBottomBorder + new Vector3Int(x, y, 0), tc.WaterTile);
            }
        }

        int CurrentMaxGroundCells = MaxGroundCells;
        if (SpreadCellsMaximum)
        {
            CurrentMaxGroundCells = MaxGroundCells / Continents;
        }
        //?????????? ??????????
        GenerateGround(Continents, CurrentMaxGroundCells);
        //?????????? ?????? ????????
        for (int i = 0; i < RandomIslandsCount; i++)
        {
            Vector3Int CurrentPos = WorldBottomBorder + new Vector3Int(Random.Range(0, xSize), Random.Range(0, ySize), 0);
            Map[CurrentPos.x, CurrentPos.y] = true;
            GroundCellsGenerated++;
            SecondGenerationProcess(CurrentPos, CurrentMaxGroundCells);
        }
        while (GroundCellsGenerated < CurrentMaxGroundCells * MinimumOutOfMax)
        { //????????????? ?????? ????????? ?????????? ???
            SpreadCellsMaximum = false;
            GenerateGround(1, CurrentMaxGroundCells);
        }
        GenerateWordBorder();
        CorrectAllTiles();
        GenerateWaterSpots();

        CreatureSpawning();

        SuccesfullyGenerated = true;
        Debug.Log("??????? ?????????????");
    }
    public void GenerateGround(int GroundSpotsToGenerate, int currentMax)
    {
        //?????????? ???????? ?????
        for (int cont = 0; cont < GroundSpotsToGenerate; cont++)
        {
            if (SpreadCellsMaximum)
            {
                GroundCellsGenerated = 0;
            }
            Vector3Int CurrentPos = WorldBottomBorder + new Vector3Int(Random.Range(0, xSize), Random.Range(0, ySize), 0);
            Map[CurrentPos.x, CurrentPos.y] = true;
            GroundCellsGenerated++;
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (CurrentPos.x + x < 0 || CurrentPos.y + y < 0 || CurrentPos.x + x >= xSize || CurrentPos.y + y >= ySize) return;
                    if (Random.Range(0, 1.0f) < MainContinentChunkChance)
                    {
                        if (tc.WalkableTile(CurrentPos + new Vector3Int(x, y, 0)) == false)
                        {
                            Map[CurrentPos.x + y, CurrentPos.y + y] = true;
                            SecondGenerationProcess(CurrentPos + new Vector3Int(x, y, 0), currentMax);
                            GroundCellsGenerated++;
                        }
                    }
                }
            }
        }
    }
    public void GenerateWordBorder()
    {
        //?????????? ??????? ????
        for (int i = 0; i < xSize; i++)
        {
            for (int n = 0; n < 3; n++)
            {
                tc.ZeroTilemapLevel.SetTile(WorldBottomBorder + new Vector3Int(i, n, 0), tc.WaterTile);
                tc.ZeroTilemapLevel.SetTile(WorldBottomBorder + new Vector3Int(n, i, 0), tc.WaterTile);
                tc.ZeroTilemapLevel.SetTile(WorldBottomBorder + new Vector3Int(i, ySize - n, 0), tc.WaterTile);
                tc.ZeroTilemapLevel.SetTile(WorldBottomBorder + new Vector3Int(xSize - n, i, 0), tc.WaterTile);

                tc.tl.SetTile(WorldBottomBorder + new Vector3Int(i, n, 0), null);
                tc.tl.SetTile(WorldBottomBorder + new Vector3Int(n, i, 0), null);
                tc.tl.SetTile(WorldBottomBorder + new Vector3Int(i, ySize - n, 0), null);
                tc.tl.SetTile(WorldBottomBorder + new Vector3Int(xSize - n, i, 0), null);

                BlockerLevel.SetTile(WorldBottomBorder + new Vector3Int(i, -1, 0), tc.WaterTile);
                BlockerLevel.SetTile(WorldBottomBorder + new Vector3Int(-1, i, 0), tc.WaterTile);
                BlockerLevel.SetTile(WorldBottomBorder + new Vector3Int(i, ySize, 0), tc.WaterTile);
                BlockerLevel.SetTile(WorldBottomBorder + new Vector3Int(xSize, i, 0), tc.WaterTile);
            }

        }
        BlockerLevel.SetTile(WorldBottomBorder + new Vector3Int(0, 0, 0), tc.WaterTile);
        BlockerLevel.SetTile(WorldBottomBorder + new Vector3Int(0, ySize, 0), tc.WaterTile);
        BlockerLevel.SetTile(WorldBottomBorder + new Vector3Int(xSize, ySize, 0), tc.WaterTile);
        BlockerLevel.SetTile(WorldBottomBorder + new Vector3Int(xSize, 0, 0), tc.WaterTile);
    }
    public void SecondGenerationProcess(Vector3Int CurPos, int CurrentMaximumCells)
    { //????? ???? ???????? ? ?????? ??????
        if (GroundCellsGenerated > CurrentMaximumCells) return;
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (CurPos.x + x < 0 || CurPos.y + y < 0 || CurPos.x + x >= xSize || CurPos.y + y >= ySize) return;
                if (Random.Range(0, 1.0f) < SecondContinentChunkChance)
                {
                    if (tc.WalkableTile(CurPos + new Vector3Int(x, y, 0)) == false)
                    {
                        Map[CurPos.x + x, CurPos.y + y] = true;
                        GroundCellsGenerated++;
                    }
                    if (Random.Range(0, 1.0f) < ContinentEnlargementChance)
                    {
                        SecondGenerationProcess(CurPos + new Vector3Int(x, y, 0), CurrentMaximumCells);
                    }
                }
            }
        }
    }
    public void CorrectAllTiles()
    {
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < ySize; x++)
            {
                tc.ClearUselessTiles(WorldBottomBorder + new Vector3Int(x, y, 0));
            }
        }
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < ySize; x++)
            {
                if (tc.WalkableTile(WorldBottomBorder + new Vector3Int(x, y, 0)))
                {
                    Map[WorldBottomBorder.x + x, WorldBottomBorder.y + y] = true;
                }
            }
        }
    }
    public void GenerateResourcesOnTile(Vector3Int pos)
    {
        for (int i = 1; i < ResourceGenerationChances.Length; i++)
        {
            if (Random.Range(0.0f, 1.0f) < ResourceGenerationChances[i])
            {
                if (i == 1)
                {
                    if (tc.WalkableTile(pos))
                    {
                        if (LandObjectSpawningPossiblePositionsCount < LandObjectSpawningPossiblePositions.Length)
                        {
                            LandObjectSpawningPossiblePositions[LandObjectSpawningPossiblePositionsCount] = pos;
                            LandObjectSpawningPossiblePositionsCount++;
                        }
                        else
                        {
                            Debug.Log("???????????? ??????? ?? ????");
                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    GameObject NewRes = Instantiate(ResourcesPrefabs[i], pos, Quaternion.identity);
                    NewRes.transform.SetParent(NaturalObjects.transform);
                    tc.ResourceSources[pos.x, pos.y] = (sbyte)i;
                    gm.Resources[i][gm.ResourcesGeneratedCount[i]] = NewRes;
                    gm.ResourcesGeneratedCount[i]++;
                    if (i == 1)
                    {
                        gm.ApplyNewBuilding(pos, 1);
                    }
                    if (i == 4)
                    {
                        gm.ApplyNewBuilding(pos, 4);
                    }
                }
            }
        }
    }
    public void RemoveAllObjects()
    {
        foreach (GameObject g in gm.UnitsBuildings)
        {
            Destroy(g);
        }
        foreach (GameObject g in gm.UnitsBiologicalLand)
        {
            Destroy(g);
        }
        foreach (GameObject g in gm.Resources[1])
        {
            Destroy(g);
        }
        foreach (GameObject g in gm.Resources[2])
        {
            Destroy(g);
        }
    }
    public void SetCamOnPreviewStance()
    {
        cam.transform.position = new Vector3(WorldBottomBorder.x + xSize / 2, WorldBottomBorder.y + ySize / 2, -1);
        cam.GetComponent<Camera>().orthographicSize = xSize / 2;
    }
    public void SetCamOnNormalStance()
    {
        cam.transform.position = CamStartingPosition;
        cam.GetComponent<Camera>().orthographicSize = CamNormalSize;
    }
    public void CreatureSpawning()
    {
        if (DontSpawnAnything) return;
        for (int i = 0; i < CreatureTypesAmountToSpawn.Length; i++)
        {
            CreatureTypesAmountToSpawn[i] = (int)(CreatureTypesAmountToSpawn[i] * (1 + Random.Range(-CreatureTypesAmountRandomizationFactor[i], CreatureTypesAmountRandomizationFactor[i])));
            for (int k = 0; k < CreatureTypesAmountToSpawn[i]; k++)
            {
                if (CreaturesToSpawn[i].GetComponent<bio>().HabitationType == 0 || CreaturesToSpawn[i].GetComponent<bio>().HabitationType == 2)
                {
                    GameObject sp = Instantiate(CreaturesToSpawn[i], LandObjectSpawningPossiblePositions[Random.Range(0, LandObjectSpawningPossiblePositionsCount)], Quaternion.identity);
                    sp.name = sp.name.Substring(0, sp.name.Length - 7) + " breed 0";
                }
                else
                {
                    GameObject sp = Instantiate(CreaturesToSpawn[i], NavalObjectSpawningPossiblePositions[Random.Range(0, NavalObjectSpawningPossiblePositionsCount)], Quaternion.identity);
                    sp.name = sp.name.Substring(0, sp.name.Length - 7) + " breed 0";
                }
            }
        }
    }
    public void GenerateWaterSpots()
    {
        while (NavalObjectSpawningPossiblePositionsCount < NavalObjectSpawningPossiblePositions.Length)
        {
            Vector3Int RandomPos = new Vector3Int(Random.Range(0, xSize - 2), Random.Range(0, ySize - 2), 0) + WorldBottomBorder;
            RandomPos = new Vector3Int((int)RandomPos.x, (int)RandomPos.y, 0);
            if (gm.tc.ZeroTilemapLevel.GetTile<Tile>(RandomPos) != null)
            {
                NavalObjectSpawningPossiblePositions[NavalObjectSpawningPossiblePositionsCount] = RandomPos;
                NavalObjectSpawningPossiblePositionsCount++;
            }
        }
    }
    public void ActuallyPlaceAllTiles()
    {
        for (int y = 0; y < MapSize; y++)
        {
            for (int x = 0; x < MapSize; x++)
            {
                if (Map[x, y])
                {
                    tc.JustPlace(WorldBottomBorder + new Vector3Int(x, y, 0));
                }
            }
        }
    }
}
