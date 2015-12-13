using UnityEngine;
using UnityEditor;
using System.Collections;

public class TerrainTypeDataEditor : Editor
{
	[MenuItem("Assets/Create/ELB/Terrain/TerrainTypeData")]
    public static void CreateBuilding()
    {
        //get a save location
        string path = EditorUtility.SaveFilePanel("Create The New terrainTypeData",
                                                  "Assets/Unity/ScriptableObjects/Board/Terrain", "default.asset", "asset");

        if (path == "")
            return;

        //get the location relative to the project
        path = FileUtil.GetProjectRelativePath(path);

        CreateTerrainTypeDataAt(path);
    }

    public static TerrainTypeData CreateTerrainTypeDataAt(string path)
    {
        TerrainTypeData terrainTypeData = CreateInstance<TerrainTypeData>();
        AssetDatabase.CreateAsset(terrainTypeData, path);
        AssetDatabase.SaveAssets();
        return terrainTypeData;
    }
}
