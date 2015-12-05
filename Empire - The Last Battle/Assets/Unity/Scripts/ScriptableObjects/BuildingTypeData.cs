using UnityEngine;
using System.Collections;

public class BuildingTypeData : TileTypeDataBase {
    public BuildingType Type;
    public BuildingTypeData() {
        Type = BuildingType.None;
        TileType = global::TileType.Building;
    }
}