using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TileData : ScriptableObject {
    public TerrainType Terrain;
    public BuildingType Building;
    public PlayerType Owner;
    public float Height;
	public bool Defended;

	private List<TileData>ConnectedTiles;
    public GameObject TileObject {
        get;
        set;
    }
    public int X {
        get;
        set;
    }
	public int Y {
        get;
        set;
    }

	public bool IsDefended() {
		return Defended;
	}
	public void AddConnectedTile(TileData t) {
		if (ConnectedTiles == null) {
			ConnectedTiles = new List<TileData>();
			ConnectedTiles.Add(t);
		}
		else if (!ConnectedTiles.Contains (t)) {
			ConnectedTiles.Add(t);
		}
	}

	public List<TileData> GetConnectedTiles() {
		if (ConnectedTiles == null) {
			return new List<TileData>();
		}
		return ConnectedTiles;
	}
}