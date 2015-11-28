using UnityEngine;
using UnityEditor;
using System.Collections;

public class TileDataEditor : Editor
{
    [MenuItem("Assets/Create/ELB/TileData")]
    public static void CreateBuilding()
    {
        //get a save location
        string path = EditorUtility.SaveFilePanel("Create The New TileData",
                                                  "Assets/Unity/ScriptableObjects/Board/Tiles/", "defualt.asset", "asset");

        if (path == "")
            return;

        //get the location relative to the project
        path = FileUtil.GetProjectRelativePath(path);

        CreateTileDataAt(path);
    }

    public static TileData CreateTileDataAt(string path)
    {
        TileData tileData = CreateInstance<TileData>();
        AssetDatabase.CreateAsset(tileData, path);
        AssetDatabase.SaveAssets();
        return tileData;
    }
}


