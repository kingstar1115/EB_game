using UnityEngine;
using System.Collections.Generic;

public class Board : MonoBehaviour {

	public BoardData _Data;
	public float _TileWidth = 1;
	public TileTypeDataManager _TileTypeDataManager;
	public FlagManager _FlagManager;
	public TileData _BBStartTile;
	public TileData _SSStartTile;
    public string _MarkerCommanderBB_Tag;
    public string _MarkerCommanderSS_Tag;
	public GameObject[] _BattlebeardCastles;
	public GameObject[] _StormshaperCastles;

	int battlebeardCastleState = 0;
	int stormshaperCastleState = 0;

    public void Initialise() {
        _TileTypeDataManager.Initialise();
        _FlagManager.Initialise();
        Generate(this.gameObject.transform.position);
    }

    public void Generate(Vector3 origin, bool loadGame = false) {
        Vector2 arraySize = GetBoardSize();
        int arrayWidth = (int)arraySize.x,
            arrayHeight = (int)arraySize.y;

		List<PlayerType> lostImmortals = new List<PlayerType>() {
			PlayerType.Battlebeard, PlayerType.Battlebeard, PlayerType.Battlebeard, PlayerType.Battlebeard,
			PlayerType.Stormshaper, PlayerType.Stormshaper, PlayerType.Stormshaper, PlayerType.Stormshaper
		};
           
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
				if (_TileTypeDataManager.GetTerrainData(tile.Terrain).IsRotatable) {
					rotateRandom(tile.TileObject);
				}
                //grab the tile holder 
                TileHolder tileHolder = tile.TileObject.GetComponentInChildren<TileHolder>();
                if (tileHolder == null)
                    Debug.LogError("NO TILE HOLDER :O");

                //if there is a building
                if (GetBuilding(tile) != null) {
                    GameObject buildingGO = (GameObject)Instantiate(GetBuilding(tile), position, Quaternion.identity);
                    // Set building name??
                    buildingGO.transform.parent = tile.TileObject.transform;
					if (_TileTypeDataManager.GetBuildingData(tile.Building).IsRotatable) {
						rotateRandom(buildingGO);
					}
                    //add building commander markers to tile holder
                    tileHolder._MarkerCommanderBB = Utils.GetFirstChildWithTag(_MarkerCommanderBB_Tag, tile.TileObject);
                    tileHolder._MarkerCommanderSS = Utils.GetFirstChildWithTag(_MarkerCommanderSS_Tag, tile.TileObject);

					// if the building is a fortress then assign a lost immortal (we can use owner for this)
					if (!loadGame && tile.Building == BuildingType.Fortress) {
						int r = Random.Range(0, lostImmortals.Count);
						tile.Owner = lostImmortals[r];
						lostImmortals.RemoveAt(r);
					}
                }

				if (tile.Terrain == TerrainType.CastleCorner00) {
					if (tile.Building == BuildingType.CastleBattlebeard) {
						for (int c = 0; c < _BattlebeardCastles.Length; c++) {
							_BattlebeardCastles[c] = (GameObject)Instantiate(_BattlebeardCastles[c], position + new Vector3(_TileWidth/2, 0, _TileWidth/2), Quaternion.identity);
							_BattlebeardCastles[c].SetActive(false);
						}
					}
					if (tile.Building == BuildingType.CastleStormshaper) {
						for (int c = 0; c < _StormshaperCastles.Length; c++) {
							_StormshaperCastles[c] = (GameObject)Instantiate(_StormshaperCastles[c], position + new Vector3(_TileWidth / 2, 0, _TileWidth / 2), Quaternion.identity);
							_StormshaperCastles[c].SetActive(false);
						}
					}
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
                tileHolder._Tile = tile;
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

	void rotateRandom(GameObject obj) {
		int i = Random.Range (0, 3) * 90;
		obj.transform.Rotate (new Vector3 (0, i, 0));
	}

    public void SetTileOwner(TileData t, PlayerType p) {
        t.Owner = p;
        _FlagManager.SetFlagForTile(t);
    }

	// set the state of a specific castle. 0-4. 0 is no castle, 4 is fully built.
	public void SetCastleState(PlayerType p, int state) {
		if (p == PlayerType.Battlebeard) {
			if (battlebeardCastleState != -1) {
				_BattlebeardCastles[battlebeardCastleState].SetActive(false);
			}
			battlebeardCastleState = state -1;
			if (state != 0) {
				_BattlebeardCastles[battlebeardCastleState].SetActive(true);
			}
		}
		else {
			if (stormshaperCastleState != -1) {
				_StormshaperCastles[stormshaperCastleState].SetActive(false);
			}
			stormshaperCastleState = state - 1;
			if (state != 0) {
				_StormshaperCastles[stormshaperCastleState].SetActive(true);
			}
		}
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

	public HashSet<TileData> GetReachableTiles(Player p, TileData fromTile, int distance)
	{
		HashSet<TileData> foundTiles = new HashSet<TileData>();
		if (distance == 0 || fromTile == null) {
			return foundTiles;
		}
		foreach (TileData t in fromTile.GetConnectedTiles()) {
			if (t == p.CommanderPosition) {
				continue;
			}
			if ((p.Type == PlayerType.Battlebeard && t.Building != BuildingType.StartTileStormshaper) || (p.Type == PlayerType.Stormshaper && t.Building != BuildingType.StartTileBattlebeard))
			    foundTiles.Add(t);
			HashSet<TileData> tilesForT = GetReachableTiles(p, t, distance - 1);
            foreach (TileData tt in tilesForT) {
                //check that thetile is not the oponents start tile 
				if ((p.Type == PlayerType.Battlebeard && t.Building != BuildingType.StartTileStormshaper) || (p.Type == PlayerType.Stormshaper && t.Building != BuildingType.StartTileBattlebeard))
				    foundTiles.Add(tt);
			}
		}
		return foundTiles;
	}


}