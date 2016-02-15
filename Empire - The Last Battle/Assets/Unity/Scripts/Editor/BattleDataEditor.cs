using UnityEditor;

public class BattleDataEditor : Editor
{

    [MenuItem("Assets/Create/ELB/Battle/BattleData")]
	public static void CreateBattleData()
    {
        //get a save location
		string path = EditorUtility.SaveFilePanel("Create BattleData",
                                                  "Assets/Unity/ScriptableObjects/Battle/", "newBattleData.asset", "asset");

        if (path == "")
            return;

        //get the location relative to the project
        path = FileUtil.GetProjectRelativePath(path);

		CreateBattleDataAt(path);
    }

	public static BattleData CreateBattleDataAt(string path)
    {
		BattleData battleData = CreateInstance<BattleData>();
		AssetDatabase.CreateAsset(battleData, path);
        AssetDatabase.SaveAssets();
		return battleData;
    }
}
