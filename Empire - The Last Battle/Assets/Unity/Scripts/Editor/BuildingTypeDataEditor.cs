using UnityEngine;
using UnityEditor;
using System.Collections;

public class BuildingTypeDataEditor : Editor
{
    [MenuItem("Assets/Create/ELB/buildingTypeData")]
    public static void CreateBuilding()
    {
        //get a save location
        string path = EditorUtility.SaveFilePanel("Create The New buildingTypeData",
                                                  "Assets/Unity/ScriptableObjects/Board/", "default.asset", "asset");

        if (path == "")
            return;

        //get the location relative to the project
        path = FileUtil.GetProjectRelativePath(path);

        CreateBuildingTypeDataAt(path);
    }

    public static BuildingTypeData CreateBuildingTypeDataAt(string path)
    {
        BuildingTypeData buildingTypeData = CreateInstance<BuildingTypeData>();
        AssetDatabase.CreateAsset(buildingTypeData, path);
        AssetDatabase.SaveAssets();
        return buildingTypeData;
    }
}
