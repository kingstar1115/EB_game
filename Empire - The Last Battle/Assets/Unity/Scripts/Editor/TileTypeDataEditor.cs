using UnityEngine;
using UnityEditor;
using System.Collections;

public class TileTypeDataEditor : Editor
{
    [MenuItem("Assets/Create/ELB/TileTypeData")]
    public static void CreateBuilding()
    {
        //get a save location
        string path = EditorUtility.SaveFilePanel("Create The New TileTypeData",
                                                  "Assets/Unity/ScriptableObjects/Board/", "defualt.asset", "asset");

        if (path == "")
            return;

        //get the location relative to the project
        path = FileUtil.GetProjectRelativePath(path);

        CreateTileTypeDataAt(path);
    }

    public static TileTypeData CreateTileTypeDataAt(string path)
    {
        TileTypeData tileTypeData = CreateInstance<TileTypeData>();
        AssetDatabase.CreateAsset(tileTypeData, path);
        AssetDatabase.SaveAssets();
        return tileTypeData;
    }
}
