using UnityEngine;
using UnityEditor;

public class UnitBaseDataEditor : Editor
{
    [MenuItem("Assets/Create/ELB/Units/UnitBaseData")]
    public static void CreateUnit()
    {
        //get a save location
        string path = EditorUtility.SaveFilePanel("Create The New Asset",
                                                  "Assets/Unity/ScriptableObjects/Units/", "default.asset", "asset");

        if (path == "")
            return;

        //get the location relative to the project
        path = FileUtil.GetProjectRelativePath(path);

        CreateUnitBaseDataAt(path);
    }

    public static UnitBaseData CreateUnitBaseDataAt(string path)
    {
        UnitBaseData UnitBase = CreateInstance<UnitBaseData>();
        AssetDatabase.CreateAsset(UnitBase, path);
        AssetDatabase.SaveAssets();
        return UnitBase;
    }
}