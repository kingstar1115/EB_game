using UnityEngine;
using System.Collections;

public class TileTypeData : ScriptableObject {
    public GameObject[] Buildings;
    public GameObject[] Terrains;    
    public TerrainType[] NonTraversableTerrain;
    public Vector2[] TerrainHeights; 
}

public enum TerrainType {
    Basic,
    Castle
}

public enum BuildingType {
    None,
    Basic
}
