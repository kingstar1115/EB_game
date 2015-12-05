using UnityEngine;
using System.Collections;

public class TileTypeDataBase : ScriptableObject {
    public bool SetHeight;
    public Vector2 Height;
    public bool IsTraversable = true;
    public TileType Tile;
    public GameObject Prefab;
}
