using UnityEngine;
using System.Collections;

public class BuildingTypeData : TileTypeDataBase {
    public BuildingType Type;
    public BuildingTypeData() {
        Type = BuildingType.None;
        Tile = TileType.Building;
    }
}