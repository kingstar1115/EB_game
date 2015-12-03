using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TileData : ScriptableObject {
    public TerrainType Terrain;
    public BuildingType Building;

	[System.NonSerialized]
	public List<TileData> ConnectedTiles;
	public int X;
	public int Y;


	public PlayerType Owner = PlayerType.Neutral;
	public float Height;

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