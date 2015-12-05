using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour {

    public BoardData Data;
    public float TileWidth = 1;
    public TileTypeDataManager TileTypeDataManager;

	// Use this for initialization
	void Start () {
        Initialise();
	}

    public void Initialise() {
        TileTypeDataManager.Initialise();
        Generate(this.gameObject.transform.position);
    }

    public void Generate(Vector3 origin, bool newHeights = true) {
        Vector2 arraySize = GetBoardSize();
        int arrayWidth = (int)arraySize.x,
            arrayHeight = (int)arraySize.y;
           
        // get start position
        Vector3 centreOffset = new Vector3(TileWidth / 2, 0, TileWidth / 2),
                boardStart = origin - new Vector3((arrayWidth * TileWidth)/2, 0, (arrayHeight * TileWidth)/2) + centreOffset;
        for (int i = 0; i < arrayWidth; i++) {
            for (int j = 0; j < arrayHeight; j++) {
                TileData tile = GetTileAt(i, j);
                if (tile == null) {
                    continue;
                }
                tile.X = i;
                tile.Y = j;
                if (newHeights) {
                    tile.Height = GetRandomHeight(tile);
                }
                Vector3 position = new Vector3(i * TileWidth, tile.Height, j * TileWidth) + boardStart;
                tile.TileObject = (GameObject)Instantiate(GetTerrain(tile), position, Quaternion.identity);
                tile.TileObject.name = "Tile " + "[" + i + "," + j + "]";
                if (GetBuilding(tile) != null) {
                    GameObject buildingGO = (GameObject)Instantiate(GetBuilding(tile), position, Quaternion.identity);
                    // Set building name??
                    buildingGO.transform.parent = tile.TileObject.transform;
                }
                if (!CanTraverse(tile)) {
                    continue;
                }
                for (int x = i - 1; x <= i + 1; x++) {
                    for (int y = j - 1; y <= j + 1; y++) {
                        if (x == i && y == j) {
                            continue;
                        }
                        TileData tileAtXY = GetTileAt(x, y);
                        if (tileAtXY != null && CanTraverse(tileAtXY)) {
                            tile.AddConnectedTile(tileAtXY);
                        }
                    }
                }
            }
        }
    }

    public Vector2 GetBoardSize() {
        // We REALLY expect the board to be a square.
        //We will run into issues if the first column is longer than the others
        int arrayWidth = Data.Data.Length,
           arrayHeight = Data.Data[0].Data.Length;
        return new Vector2(arrayWidth, arrayHeight);
    }

    public GameObject GetTerrain(TileData t) {
        return TileTypeDataManager.GetTerrainData(t.Terrain).Prefab;
    }

    public GameObject GetBuilding(TileData t) {
        return TileTypeDataManager.GetBuildingData(t.Building).Prefab;
    }

    public TileData GetTileAt(int x, int y) {
        if (x < 0 || x >= Data.Data.Length || y < 0 || y >= Data.Data[x].Data.Length) {
            return null;
        }
        return Data.Data[x].Data[y];
    }

    public bool CanTraverse(TileData t) {
        bool terrain = TileTypeDataManager.GetTerrainData(t.Terrain).IsTraversable,
             building = TileTypeDataManager.GetBuildingData(t.Building).IsTraversable;
        return terrain && building;
    }

    public float GetRandomHeight(TileData t) {
        TerrainTypeData tD = TileTypeDataManager.GetTerrainData(t.Terrain);
        BuildingTypeData bD = TileTypeDataManager.GetBuildingData(t.Building);
        Vector2 range;
        Debug.Log(t.X + "," + t.Y);
        if (bD.SetHeight) {
            range = bD.Height;
        }
        else if (tD.SetHeight) {
            range = tD.Height;
        }
        else {
            range = new Vector2();
        }

        return Random.Range(range.x, range.y);
    }

}