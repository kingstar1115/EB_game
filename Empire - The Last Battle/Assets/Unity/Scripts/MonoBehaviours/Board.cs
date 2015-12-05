using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Board : MonoBehaviour {

    public BoardData Data;
    public float TileWidth = 1;
    public TileTypeData TileTypeData;
    private List<Tile> _tiles;
    private Dictionary<TileData, Tile> _tileLookup;

    public event System.Action OnGenerate = delegate { };


	// Use this for initialization
	void Start () {
        _tiles = new List<Tile>();
        _tileLookup = new Dictionary<TileData, Tile>();
        Generate(this.gameObject.transform.position);
	}

    public void Generate(Vector3 origin, bool loadHeights = false) {
        Vector2 arraySize = GetBoardSize();
        int arrayWidth = (int)arraySize.x,
            arrayHeight = (int)arraySize.y;
           
        // get start position
        Vector3 centreOffset = new Vector3(TileWidth / 2, 0, TileWidth / 2),
                boardStart = origin - new Vector3((arrayWidth * TileWidth)/2, 0, (arrayHeight * TileWidth)/2) + centreOffset;
        for (int i = 0; i < arrayWidth; i++) {
            for (int j = 0; j < arrayHeight; j++) {
                TileData tile = GetTileDataAt(i, j);
                GameObject tileGO;
                if (tile == null) {
                    continue;
                }
                float height = GetTileHeight(tile, !loadHeights);
                if (tile.Height != height) {
                    tile.Height = height;
                }
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
                    _tileLookup.Add(tile, t);
                }
                t.TileObject = tileGO;

                if(!CanTraverse(t.TileData)) {
                    continue;
                }

                for (int x = i - 1; x <= i + 1; x++) {
                    for (int y = j - 1; y <= j + 1; y++) {
                        if (x == i && y == j) {
                            continue;
                        }
                        Tile tileAtXY = GetTileAt(x, y);
                        if (tileAtXY == null) {
                            TileData td = GetTileDataAt(x, y);
                            if (td != null) {
                                tileAtXY = new Tile();
                                tileAtXY.X = x;
                                tileAtXY.Y = y;
                                tileAtXY.TileData = td;
                                _tiles.Add(tileAtXY);
                                _tileLookup.Add(td, tileAtXY);
                            }
                        }
                        if (tileAtXY != null && CanTraverse(tileAtXY.TileData)) {
                            if (t.ConnectedTiles == null) {
                                t.ConnectedTiles = new List<Tile>();
                            }
                            t.ConnectedTiles.Add(tileAtXY);
                        }
                    }
                }
            }
        }
        OnGenerate();
    }

    public Vector2 GetBoardSize() {
        // We REALLY expect the board to be a square.
        //We will run into issues if the first column is longer than the others
        int arrayWidth = Data.Data.Length,
           arrayHeight = Data.Data[0].Data.Length;
        return new Vector2(arrayWidth, arrayHeight);
    }

    public GameObject GetTerrain(TerrainType tt) {
        if (TileTypeData.Terrains.Length < (int)tt) {
            Debug.LogError("TerrainType is out of sync with the terrain GameObjects!");
        }
        return TileTypeData.Terrains[(int)tt];
    }

    public GameObject GetBuilding(BuildingType bt) {
        if (TileTypeData.Buildings.Length < (int)bt) {
            Debug.LogError("BuildingType is out of sync with the building GameObjects!");
        }
        return TileTypeData.Buildings[(int)bt];
    }

    public TileData GetTileDataAt(int x, int y) {
        if (x < 0 || x >= Data.Data.Length || y < 0 || y >= Data.Data[x].Data.Length) {
            return null;
        }
        return Data.Data[x].Data[y];
    }

    public Tile GetTileAt(int x, int y) {
        TileData td = GetTileDataAt(x,y);
        if (td == null) {
            return null;
        }
        Tile t;
        _tileLookup.TryGetValue(td, out t);
        return t;
    }

    public bool CanTraverse(TileData t) {
        if (t == null) {
            return false;
        }
        return System.Array.IndexOf(TileTypeData.NonTraversableTerrain, t.Terrain) == -1;
    }

    public float GetTileHeight(TileData t, bool random = true) {
        if (random) {
            Vector2 r = TileTypeData.TerrainHeights[(int)t.Terrain];
            return Random.Range(r.x, r.y);
        }
        else {
            return t.Height;
        }
    }

}

public class Tile {
    public GameObject TileObject;
    public TileData TileData;
    public List<Tile> ConnectedTiles;
    public int X;
    public int Y;
}