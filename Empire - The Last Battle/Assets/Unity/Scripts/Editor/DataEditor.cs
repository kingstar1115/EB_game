using UnityEditor;
using UnityEngine;

public class DataEditor : Editor {
    public static void CreateScriptableObject(string type, string subPath = "") {
        //get a save location
        string path = EditorUtility.SaveFilePanel("Create " + type,
                                                  "Assets/Unity/ScriptableObjects/" + subPath, "new" + type + ".asset", "asset");
        if(path == "")
            return;

        //get the location relative to the project
        path = FileUtil.GetProjectRelativePath(path);
        ScriptableObject data = CreateInstance(type);
        AssetDatabase.CreateAsset(data, path);
        AssetDatabase.SaveAssets();
    }
}
