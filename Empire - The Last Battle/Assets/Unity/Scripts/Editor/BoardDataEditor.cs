using UnityEngine;
using UnityEditor;
using System.Collections;

public class BoardDataEditor : Editor
{

    [MenuItem("Assets/Create/ELB/BoardData")]
    public static void CreateBuilding()
    {
        //get a save location
        string path = EditorUtility.SaveFilePanel("Create The BoardData",
                                                  "Assets/Unity/ScriptableObjects/Board/", "defualt.asset", "asset");

        if (path == "")
            return;

        //get the location relative to the project
        path = FileUtil.GetProjectRelativePath(path);

        CreateBoardDataAt(path);
    }

    public static BoardData CreateBoardDataAt(string path)
    {
        BoardData boardData = CreateInstance<BoardData>();
        AssetDatabase.CreateAsset(boardData, path);
        AssetDatabase.SaveAssets();
        return boardData;
    }
}
