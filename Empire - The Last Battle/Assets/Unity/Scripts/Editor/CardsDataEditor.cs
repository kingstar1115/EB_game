using UnityEditor;

public class CardsDataEditor : Editor
{

    [MenuItem("Assets/Create/ELB/CardsData")]
    public static void CreatePlayerCards()
    {
        //get a save location
        string path = EditorUtility.SaveFilePanel("Create The New Cards",
                                                  "Assets/Unity/ScriptableObjects/Cards/", "default.asset", "asset");

        if (path == "")
            return;

        //get the location relative to the project
        path = FileUtil.GetProjectRelativePath(path);

        CreatePlayerCardsDataAt(path);
    }

    public static CardsData CreatePlayerCardsDataAt(string path)
    {
        CardsData playerCards = CreateInstance<CardsData>();
        AssetDatabase.CreateAsset(playerCards, path);
        AssetDatabase.SaveAssets();
        return playerCards;
    }
}
