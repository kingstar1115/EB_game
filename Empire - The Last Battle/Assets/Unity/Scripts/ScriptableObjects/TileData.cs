using UnityEngine;
using System.Collections.Generic;

public class TileData : ScriptableObject {
    public TerrainType Terrain;
    public BuildingType Building;
    public List<TileData> Adjacent;
    public Vector2 HeightBetween;
}
