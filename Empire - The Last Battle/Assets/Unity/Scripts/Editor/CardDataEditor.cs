using UnityEditor;

public class CardDataEditor : Editor
{

    [MenuItem("Assets/Create/ELB/CardData")]
    public static void CreateCardData()
    {
        //get a save location
        string path = EditorUtility.SaveFilePanel("Create CardData",
                                                  "Assets/Unity/ScriptableObjects/Cards/", "default.asset", "asset");

        if (path == "")
            return;

        //get the location relative to the project
        path = FileUtil.GetProjectRelativePath(path);

        CreateCardDataAt(path);
    }

    public static CardData CreateCardDataAt(string path)
    {
        CardData cardData = CreateInstance<CardData>();
        AssetDatabase.CreateAsset(cardData, path);
        AssetDatabase.SaveAssets();
        return cardData;
    }
}
