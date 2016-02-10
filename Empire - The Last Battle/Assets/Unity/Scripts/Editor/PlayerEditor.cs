using UnityEditor;

public class PlayerEditor : Editor
{

    [MenuItem("Assets/Create/ELB/Player/Player")]
    public static void CreatePlayer()
    {
        //get a save location
        string path = EditorUtility.SaveFilePanel("Create Player",
                                                  "Assets/Unity/ScriptableObjects/Player/", "newPlayer.asset", "asset");

        if (path == "")
            return;

        //get the location relative to the project
        path = FileUtil.GetProjectRelativePath(path);

        CreatePlayerAt(path);
    }

	public static Player CreatePlayerAt(string path)
    {
		Player player = CreateInstance<Player>();
        AssetDatabase.CreateAsset(player, path);
        AssetDatabase.SaveAssets();
        return player;
    }
}
