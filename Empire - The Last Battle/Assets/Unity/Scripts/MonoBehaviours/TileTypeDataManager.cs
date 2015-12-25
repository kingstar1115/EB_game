using UnityEngine;
using System.Collections.Generic;

public class TileTypeDataManager : MonoBehaviour {
    Dictionary<TerrainType, TerrainTypeData> _terrainData;
    Dictionary<BuildingType, BuildingTypeData> _buildingData;
    public List<TileTypeDataBase> BaseData;

    public void Initialise() {
        _terrainData = new Dictionary<TerrainType, TerrainTypeData>();
        _buildingData = new Dictionary<BuildingType, BuildingTypeData>();
        foreach (TileTypeDataBase data in BaseData) {
            if (data.Tile == TileType.Terrain) {
                TerrainTypeData tData = (TerrainTypeData)data;
                _terrainData.Add(tData.Type, tData);
            }
            else {
                BuildingTypeData bData = (BuildingTypeData)data;
                _buildingData.Add(bData.Type, bData);
            }
        }
    }

    public TerrainTypeData GetTerrainData(TerrainType t) {
        TerrainTypeData data;
        _terrainData.TryGetValue(t, out data);
        return data;
    }

    public BuildingTypeData GetBuildingData(BuildingType t) {
        if (t == BuildingType.None) {
            return ScriptableObject.CreateInstance<BuildingTypeData>();
        }
        BuildingTypeData data;
        _buildingData.TryGetValue(t, out data);
        return data;
    }
}

public enum TileType {
    Terrain,
    Building
}


public enum TerrainType {
    CastleCorner00,
    CastleCorner01,
    CastleCorner10,
    CastleCorner11,
    Forest,
    Grass,
    Mountain,
    Savannah,
    Tundra,
}

public enum BuildingType {
    None,
    Armoury,
    Camp,
    CastleBattlebeard,
    CastleStormshaper,
    Cave,
    Fortress,
    Inn,
    StartTileBattlebeard,
    StartTileStormshaper
}
