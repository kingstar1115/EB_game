using UnityEditor;

public class PlayerCardsDataEditor : Editor
{

    [MenuItem("Assets/Create/ELB/PlayerCardsData")]
    public static void CreatePlayerCards()
    {
        //get a save location
        string path = EditorUtility.SaveFilePanel("Create The New PlayerCards",
                                                  "Assets/Unity/ScriptableObjects/Cards/", "default.asset", "asset");

        if (path == "")
            return;

        //get the location relative to the project
        path = FileUtil.GetProjectRelativePath(path);

        CreatePlayerCardsDataAt(path);
    }

    public static PlayerCards CreatePlayerCardsDataAt(string path)
    {
        PlayerCards playerCards = CreateInstance<PlayerCards>();
        AssetDatabase.CreateAsset(playerCards, path);
        AssetDatabase.SaveAssets();
        return playerCards;
    }
}
