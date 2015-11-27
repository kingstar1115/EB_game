using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Board : MonoBehaviour {

    public Vector3 Origin;
    public BoardData Data;
    public float TileWidth = 1;
    public GameObject[] Terrains;
    public GameObject[] Buildings;
    public List<Tile> _tiles;



	// Use this for initialization
	void Start () {
        _tiles = new List<Tile>();
        Generate(Origin);
	}

    public void Generate(Vector3 origin) {
        int arrayWidth = Data.Data.Length,
            arrayHeight = Data.Data[0].Data.Length;

        // get start position
        Vector3 centreOffset = new Vector3(TileWidth / 2, 0, TileWidth / 2),
                boardStart = origin - new Vector3((arrayWidth * TileWidth)/2, 0, (arrayHeight * TileWidth)/2) + centreOffset;
        for (int i = 0; i < arrayWidth; i++) {
            for (int j = 0; j < arrayHeight; j++) {
                TileData tile = GetTileDataAt(i, j);
                GameObject tileGO;
                if (tile != null) {
                    float height = Random.Range(tile.HeightBetween.x, tile.HeightBetween.y);
                    Vector3 position = new Vector3(i * TileWidth, height, j * TileWidth) + boardStart;
                    tileGO = (GameObject)Instantiate(GetTerrain(tile.Terrain), position, Quaternion.identity);
                    tileGO.name = "Tile " + "[" + i + "," + j + "]";
                    if (tile.Building != BuildingType.None) {
                        GameObject buildingGO = (GameObject)Instantiate(GetBuilding(tile.Building), position, Quaternion.identity);
                        // Set building name??
                        buildingGO.transform.parent = tileGO.transform;
                    }
                    // Build graph
                    Tile t = GetTileAt(i, j);
                    if (t == null) {
                        t = new Tile();
                        t.X = i;
                        t.Y = j;
                        t.TileData = tile;
                        _tiles.Add(t);
                    }
                    else {
                        t.TileObject = tileGO;
                    }

                    if(CanTraverse(t.TileData)) {
                        for (int x = i - 1; x <= i + 1; x++) {
                            for (int y = j - 1; y <= j + 1; y++) {
                                Tile tileAtXY = GetTileAt(x, y);
                                if (tileAtXY == null) {
                                    tileAtXY = new Tile();
                                    tileAtXY.X = x;
                                    tileAtXY.Y = y;
                                    tileAtXY.TileData = GetTileDataAt(x, y);
                                    _tiles.Add(tileAtXY);
                                }
                                if (CanTraverse(tileAtXY.TileData)) {
                                    t.ConnectedTiles.Add(tileAtXY);
                                }
                            }
                        }
                    }
                }
                

            }
        }
    }

    private void buildGraph(int x, int y) {

    }

    public GameObject GetTerrain(TerrainType tt) {
        if (Terrains.Length < (int)tt) {
            Debug.LogError("TerrainType is out of sync with the terrain GameObjects!");
        }
        return Terrains[(int)tt];
    }

    public GameObject GetBuilding(BuildingType bt) {
        if (Terrains.Length < (int)bt) {
            Debug.LogError("BuildingType is out of sync with the building GameObjects!");
        }
        return Buildings[(int)bt];
    }

    public TileData GetTileDataAt(int x, int y) {
        if (x < 0 || x >= Data.Data.Length || y < 0 || y >= Data.Data[x].Data.Length) {
            return null;
        }
        return Data.Data[x].Data[y];
    }

    public Tile GetTileAt(int x, int y) {
        return _tiles.Find(ti => ti.X == x && ti.Y == y);
    }

    public bool CanTraverse(TileData t) {
        if (t == null) {
            return false;
        }
        // List NON traversable TerrainTypes here.
        BuildingType[] nonTrav = { BuildingType.Castle };
        return ~System.Array.IndexOf(nonTrav, t.Building) != 0;

    }

}

public enum TerrainType {
    Basic
}

public enum BuildingType {
    None,
    Basic,
    Castle
}

public class Tile {
    public GameObject TileObject;
    public TileData TileData;
    public List<Tile> ConnectedTiles;
    public int X;
    public int Y;
}