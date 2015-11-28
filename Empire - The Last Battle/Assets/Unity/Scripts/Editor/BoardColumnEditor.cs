using UnityEngine;
using UnityEditor;
using System.Collections;

public class BoardColumnEditor : Editor
{
    [MenuItem("Assets/Create/ELB/BoardColumn")]
    public static void CreateBuilding()
    {
        //get a save location
        string path = EditorUtility.SaveFilePanel("Create The New BoardColumn",
                                                  "Assets/Unity/ScriptableObjects/Board/BoardColumns/", "defualt.asset", "asset");

        if (path == "")
            return;

        //get the location relative to the project
        path = FileUtil.GetProjectRelativePath(path);

        CreateBoardColumnAt(path);
    }

    public static BoardColumn CreateBoardColumnAt(string path)
    {
        BoardColumn boardColumn = CreateInstance<BoardColumn>();
        AssetDatabase.CreateAsset(boardColumn, path);
        AssetDatabase.SaveAssets();
        return boardColumn;
    }
}
