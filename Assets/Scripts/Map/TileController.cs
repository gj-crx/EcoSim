using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileController : MonoBehaviour
{
    public Tilemap tl;
    public Tilemap ZeroTilemapLevel;
    public Tile[] Tiles = new Tile[9];
    public Tile WaterTile;

    public bool[,] BuildingTiles;
    public sbyte[,] ResourceSources;
    

    private Vector3Int AnalyzedTilePos;
    public Gen1 g1;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public bool WalkableTile(int x, int y)
    {
        Tile t = tl.GetTile<Tile>(AnalyzedTilePos + new Vector3Int(x, y, 0));
        if (t == null) return false;
        for (int i = 0; i < Tiles.Length; i++)
        {
            if (Tiles[i] == t) return true;
        }
        return false;
    }
    public bool WalkableTile(Vector3Int pos)
    {
        Tile t = tl.GetTile<Tile>(pos);
        if (t == null) return false;
        for (int i = 0; i < Tiles.Length; i++)
        {
            if (Tiles[i] == t) return true;
        }
        return false;
    }
    public bool WalkableTile(Vector3 position)
    {
        Vector3Int pos = new Vector3Int((int)position.x, (int)position.y, (int)position.z);
        Tile t = tl.GetTile<Tile>(pos);
        if (t == null) return false;
        for (int i = 0; i < Tiles.Length; i++)
        {
            if (Tiles[i] == t) return true;
        }
        return false;
    }
    public int AnalyzeTile(Vector3Int TilePos)
    {
        AnalyzedTilePos = TilePos;
        //первая половина
        if (WalkableTile(0, 1) && WalkableTile(1, 0) && WalkableTile(0, -1) && WalkableTile(-1, 0))
        { //если все четыре соседних блока - проходимы, но вода все равно где-то рядом
            if (!WalkableTile(-1, 1) && !WalkableTile(1, -1))
            {
                return 13;
            }
            if (!WalkableTile(1, -1) && !WalkableTile(-1, 1))
            {
                return 14;
            }
            if (!WalkableTile(1, -1))
            {
                return 9;
            }
            if (!WalkableTile(-1, -1))
            {
                return 10;
            }
            if (!WalkableTile(-1, 1))
            {
                return 11;
            }
            if (!WalkableTile(1, 1))
            {
                return 12;
            }
        }
        //вторая половина
        if (!WalkableTile(0, 1) && WalkableTile(1, 0) && WalkableTile(0, -1) && !WalkableTile(-1, 0))
        {
            return 0;
        }
        if (!WalkableTile(0, 1) && !WalkableTile(1, 0) && WalkableTile(0, -1) && WalkableTile(-1, 0))
        {
            return 2;
        }
        if (WalkableTile(0, 1) && !WalkableTile(1, 0) && !WalkableTile(0, -1) && WalkableTile(-1, 0))
        {
            return 4;
        }
        if (WalkableTile(0, 1) && WalkableTile(1, 0) && !WalkableTile(0, -1) && !WalkableTile(-1, 0))
        {
            return 6;
        }

        if (!WalkableTile(0, 1) && WalkableTile(1, 0) && WalkableTile(0, -1) && WalkableTile(-1, 0))
        {
            return 1;
        }
        if (WalkableTile(0, 1) && !WalkableTile(1, 0) && WalkableTile(0, -1) && WalkableTile(-1, 0))
        {
            return 3;
        }
        if (WalkableTile(0, 1) && WalkableTile(1, 0) && !WalkableTile(0, -1) && WalkableTile(-1, 0))
        {
            return 5;
        }
        if (WalkableTile(0, 1) && WalkableTile(1, 0) && WalkableTile(0, -1) && !WalkableTile(-1, 0))
        {
            return 7;
        }
        return 8;
    }
    public void PlaceTile(Vector3Int pos)
    {
        int TileID = AnalyzeTile(pos);
        ZeroTilemapLevel.SetTile(pos, null);
        tl.SetTile(pos, Tiles[TileID]);
        if (TileID == 8)
        {
            g1.GenerateResourcesOnTile(pos);
        }
    }
    public void JustPlace(Vector3Int pos)
    {
        ZeroTilemapLevel.SetTile(pos, null);
        tl.SetTile(pos, Tiles[8]);
    }
    public void ClearUselessTiles(Vector3Int TilePosition)
    {
        AnalyzedTilePos = TilePosition;
        if (WalkableTile(0, 0) == false) return;
        //здесь будут несколько паттернов, удаляемых вручную для избежания багов генерации
        //int Count = BoolToInt(WalkableTile(0, 1), WalkableTile(1, 0), WalkableTile(0, -1), WalkableTile(-1, 0));
        int Count = 0;
        if (WalkableTile(0, 1)) Count++;
        if (WalkableTile(0, -1)) Count++;
        if (WalkableTile(1, 0)) Count++;
        if (WalkableTile(-1, 0)) Count++;
        if (Count <= 1)
        {
            tl.SetTile(TilePosition, null);
            ZeroTilemapLevel.SetTile(TilePosition, WaterTile);
          //  Debug.Log("Тайл убран " + TilePosition.x + " " + TilePosition.y);
            if (WalkableTile(0, 1)) ClearUselessTiles(TilePosition + new Vector3Int(0, 1, 0));
            if (WalkableTile(0, -1)) ClearUselessTiles(TilePosition + new Vector3Int(0, -1, 0));
            if (WalkableTile(1, 0)) ClearUselessTiles(TilePosition + new Vector3Int(1, 0, 0));
            if (WalkableTile(-1, 0)) ClearUselessTiles(TilePosition + new Vector3Int(-1, 0, 0));
            return;
        }
        if (WalkableTile(-1, 1) && WalkableTile(0, 1) && !WalkableTile(-1, 1) && WalkableTile(-1, 0) && WalkableTile(1, 0) && 
            !WalkableTile(-1, -1) && !WalkableTile(0, -1) && !WalkableTile(1, -1))
        {
            tl.SetTile(TilePosition, null);
            ZeroTilemapLevel.SetTile(TilePosition, WaterTile);
            return;
        }

        if (AnalyzeTile(TilePosition) == 8)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (WalkableTile(x, y) == false)
                    {
                        tl.SetTile(TilePosition, null);
                        ZeroTilemapLevel.SetTile(TilePosition, WaterTile);
                    }
                }
            }
        }
    }
    public int BoolToInt(params bool[] array)
    {
        int count = 0;
        foreach (bool b in array)
        {
            if (b) count++;
        }
        return count;
    }
}
