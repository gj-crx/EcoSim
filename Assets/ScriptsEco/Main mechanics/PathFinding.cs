using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinding : MonoBehaviour
{
    public Tilemap WaterLevelTilemap;
    public Tilemap GroundLevelTilemap;
    public Tile WaterTile;

    private Vector2Int[,] TileGroups = new Vector2Int[2000, 2];
    private sbyte[,] MapDistance = new sbyte[500, 500];
    private bool[,] MarkedTiles = new bool[500, 500];
    private int countTiles = 0;
    private int CurrentGroup = 0;

    public Vector3 TestTarget;
    public bool TestOrder = false;
    public int CurrentDistance = 0;
    public Vector3[] Way;

    private bool inited = false;
    private Tilemap CurrentUnwalkableTilemap;
    void Start()
    {
        if (inited == false) Init();
    }
    public void Init()
    {
        inited = true;
        GM gm = GameObject.Find("GM").GetComponent<GM>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TestOrder)
        {
            TestOrder = false;
            CalculateWay(TestTarget, gameObject.transform.position);
        }
    }
    /// <summary>
    /// LandType 0 - land, 1 - marine, 2 - amphibious
    /// </summary>
    /// <param name="Target"></param>
    /// <param name="FromWhere"></param>
    /// <param name="LandType = sosi"></param>
    /// <returns></returns>
    public bool CalculateWay(Vector3 Target, Vector3 FromWhere, int LandType = 0)
    {
        //проверка на использование объекта до инициализации
        if (inited == false) Init();
        //инициализация
        if (LandType == 0) CurrentUnwalkableTilemap = WaterLevelTilemap;
        if (LandType == 1) CurrentUnwalkableTilemap = GroundLevelTilemap;
        bool Founded = false;
        MarkedTiles = new bool[1000, 1000];
        TileGroups = new Vector2Int[2500, 2];
        MapDistance = new sbyte[1000, 1000];
        CurrentGroup = 0;
        //округление таргета
        if (Target.x - (int)Target.x > 0.65f) Target = new Vector3((int)Target.x + 1, Target.y);
        if (Target.y - (int)Target.y > 0.65f) Target = new Vector3(Target.x, (int)Target.y + 1);
        //округление стартовой позиции
        if (FromWhere.x - (int)FromWhere.x < 0.25f)
        {
            FromWhere = new Vector3((int)FromWhere.x - 1, FromWhere.y);
        }
        if (FromWhere.y - (int)FromWhere.y < 0.25f)
        {
            FromWhere = new Vector3(FromWhere.x, (int)FromWhere.y - 1);
        }
        Vector2Int RealTarget = new Vector2Int((int)Target.x, (int)Target.y);
        //ввод стартовых данных
        TileGroups[0, 0] = new Vector2Int((int)FromWhere.x, (int)FromWhere.y);
        countTiles = 1;

        //75 циклов, каждый раз ку
        for (int Distance = 0; Distance < 75; Distance++)
        {
            int CurrentTiles = countTiles;
            countTiles = 0;
            for (int i = 0; i < CurrentTiles; i++)
            {
                MapDistance[TileGroups[i, CurrentGroup].x, TileGroups[i, CurrentGroup].y] = (sbyte)Distance;
                if (TileGroups[i, CurrentGroup] == RealTarget)
                { //путь найден
                    CurrentDistance = Distance;
                    Founded = true;
                    break;
                }
                else
                {
                    GetNeibs(TileGroups[i, CurrentGroup], LandType);
                }
            }
            if (Founded) break;
            CurrentGroup = 1 - CurrentGroup;
        }
        if (Founded)
        {
            //обратный поиск пути
            Vector2Int CurrentPath = RealTarget;
            Way = new Vector3[CurrentDistance + 1];
            for (int i = CurrentDistance; i > 0; i--)
            {
                Way[i] = new Vector3(CurrentPath.x + 0.5f, CurrentPath.y + 0.5f);
                CurrentPath = BackWayGetNeib(CurrentPath);
            }
            Way[0] = FromWhere;
        }
        return Founded;
    }
    public void GetNeibs(Vector2Int pos, int LandType = 0)
    { //находит все соседние клетки с заданной позицией, которые еще не были помечены и через которые можно пройти
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                Vector2Int t = pos + new Vector2Int(x, y);
                if (LandType == 0)
                {
                    if (t.x > 0 && t.y > 0)
                        if (CurrentUnwalkableTilemap.GetTile<Tile>(new Vector3Int(t.x, t.y, 0)) != WaterTile &&
                                CurrentUnwalkableTilemap.GetTile<Tile>(new Vector3Int(t.x, t.y, 0)) != null)
                        {
                            if (MarkedTiles[t.x, t.y] == false && (t.x != 0 || t.y != 0))
                            {
                                TileGroups[countTiles, 1 - CurrentGroup] = t;
                                countTiles++;
                                MarkedTiles[t.x, t.y] = true;
                            }
                        }
                }
                if (LandType == 1)
                {
                    if (t.x > 0 && t.y > 0)
                        if (CurrentUnwalkableTilemap.GetTile<Tile>(new Vector3Int(t.x, t.y, 0)) == WaterTile &&
                                CurrentUnwalkableTilemap.GetTile<Tile>(new Vector3Int(t.x, t.y, 0)) != null)
                        {
                            if (MarkedTiles[t.x, t.y] == false && (t.x != 0 || t.y != 0))
                            {
                                TileGroups[countTiles, 1 - CurrentGroup] = t;
                                countTiles++;
                                MarkedTiles[t.x, t.y] = true;
                            }
                        }
                }
                if (LandType == 2)
                {
                    if (t.x > 0 && t.y > 0)
                        if (CurrentUnwalkableTilemap.GetTile<Tile>(new Vector3Int(t.x, t.y, 0)) != null)
                        {
                            if (MarkedTiles[t.x, t.y] == false && (t.x != 0 || t.y != 0))
                            {
                                TileGroups[countTiles, 1 - CurrentGroup] = t;
                                countTiles++;
                                MarkedTiles[t.x, t.y] = true;
                            }
                        }
                }
            }
        }
    }
    public Vector2Int BackWayGetNeib(Vector2Int pos)
    { //
        int minimum = 300;
        Vector2Int ForReturn = Vector2Int.zero;
        for (int y = 0; y < 10; y++)
        {
            if (y > 1) y = -1;
            for (int x = 0; x < 10; x++)
            {
                if (x > 1) x = -1;
                Vector2Int t = pos + new Vector2Int(x, y);
                if (MarkedTiles[t.x, t.y] && (MapDistance[t.x, t.y] < minimum && MapDistance[t.x, t.y] > 0))
                {
                    minimum = MapDistance[t.x, t.y];
                    ForReturn = t;
                }
                if (x == -1) x = 10;
            }
            if (y == -1) y = 10;
        }
        return ForReturn;
    }
}
