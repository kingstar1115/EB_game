using UnityEngine;
using System.Collections.Generic;

public class Board : MonoBehaviour {

    public BoardData _Data;
    public float _TileWidth = 1;
    public TileTypeDataManager _TileTypeDataManager;
    public FlagManager _FlagManager;
	public TileData _BBStartTile;
	public TileData _SSStartTile;

    public void Initialise() {
        _TileTypeDataManager.Initialise();
        _FlagManager.Initialise();
        Generate(this.gameObject.transform.position);
    }

    public void Generate(Vector3 origin, bool loadGame = false) {
        Vector2 arraySize = GetBoardSize();
        int arrayWidth = (int)arraySize.x,
            arrayHeight = (int)arraySize.y;
           
        // get start position
        Vector3 centreOffset = new Vector3(_TileWidth / 2, 0, _TileWidth / 2),
                boardStart = origin - new Vector3((arrayWidth * _TileWidth) / 2, 0, (arrayHeight * _TileWidth) / 2) + centreOffset;
        for (int i = 0; i < arrayWidth; i++) {
            for (int j = 0; j < arrayHeight; j++) {
                TileData tile = GetTileAt(i, j);
                if (tile == null) {
                    continue;
                }
                tile.X = i;
                tile.Y = j;
                if (!loadGame) {
                    tile.Height = GetRandomHeight(tile);
                }
                Vector3 position = new Vector3(i * _TileWidth, tile.Height, j * _TileWidth) + boardStart;
                tile.TileObject = (GameObject)Instantiate(GetTerrain(tile), position, Quaternion.identity);
                tile.TileObject.name = "Tile " + "[" + i + "," + j + "]";
                if (GetBuilding(tile) != null) {
                    GameObject buildingGO = (GameObject)Instantiate(GetBuilding(tile), position, Quaternion.identity);
                    // Set building name??
                    buildingGO.transform.parent = tile.TileObject.transform;
                }
                // set owner flag
                _FlagManager.SetFlagForTile(tile);

				//if the tile is a start tile set them up
				if(tile.Building == BuildingType.StartTileBattlebeard) {
					_BBStartTile = tile;
				}
				else if(tile.Building == BuildingType.StartTileStormshaper) {
					_SSStartTile = tile;	 
				}
				
                // add tile reference to game object
                tile.TileObject.GetComponentInChildren<TileHolder>()._Tile = tile;
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

    public void SetTileOwner(TileData t, PlayerType p) {
        t.Owner = p;
        _FlagManager.SetFlagForTile(t);
    }

    public Vector2 GetBoardSize() {
        // We REALLY expect the board to be a square.
        //We will run into issues if the first column is longer than the others
        int arrayWidth = _Data.Data.Length,
           arrayHeight = _Data.Data[0].Data.Length;
        return new Vector2(arrayWidth, arrayHeight);
    }

    public GameObject GetTerrain(TileData t) {
        return _TileTypeDataManager.GetTerrainData(t.Terrain).Prefab;
    }

    public GameObject GetBuilding(TileData t) {
        return _TileTypeDataManager.GetBuildingData(t.Building).Prefab;
    }

    public TileData GetTileAt(int x, int y) {
        if (x < 0 || x >= _Data.Data.Length || y < 0 || y >= _Data.Data[x].Data.Length) {
            return null;
        }
        return _Data.Data[x].Data[y];
    }

    public bool CanTraverse(TileData t) {
        bool terrain = _TileTypeDataManager.GetTerrainData(t.Terrain).IsTraversable,
             building = _TileTypeDataManager.GetBuildingData(t.Building).IsTraversable;
        return terrain && building;
    }

    public float GetRandomHeight(TileData t) {
        TerrainTypeData tD = _TileTypeDataManager.GetTerrainData(t.Terrain);
        BuildingTypeData bD = _TileTypeDataManager.GetBuildingData(t.Building);
        Vector2 range;
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



	public HashSet<TileData> GetReachableTiles(TileData fromTile, int distance)
	{
		HashSet<TileData> foundTiles = new HashSet<TileData>();
		if (distance == 0 || fromTile == null) {
			return foundTiles;
		}
		foreach (TileData t in fromTile.GetConnectedTiles()) {
			foundTiles.Add(t);
            HashSet<TileData> tilesForT = GetReachableTiles(t, distance - 1);
            foreach (TileData tt in tilesForT) {
				foundTiles.Add(tt);
			}
		}
		return foundTiles;
	}


}