using UnityEditor;

public class ArmyEditor : Editor
{

    [MenuItem("Assets/Create/ELB/Player/Army")]
    public static void CreateArmy()
    {
        //get a save location
        string path = EditorUtility.SaveFilePanel("Create Army",
                                                  "Assets/Unity/ScriptableObjects/Player/", "newArmy.asset", "asset");

        if (path == "")
            return;

        //get the location relative to the project
        path = FileUtil.GetProjectRelativePath(path);

        CreateArmyAt(path);
    }

	public static Army CreateArmyAt(string path)
    {
		Army army = CreateInstance<Army>();
        AssetDatabase.CreateAsset(army, path);
        AssetDatabase.SaveAssets();
        return army;
    }
}
