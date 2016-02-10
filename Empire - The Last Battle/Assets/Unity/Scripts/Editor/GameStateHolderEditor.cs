using UnityEngine;
using UnityEditor;
using System.Collections;

public class GameStateHolderEditor : Editor 
{
	[MenuItem("Assets/Create/ELB/GameStateHolder")]
	public static void CreateBuilding()
	{
		//get a save location
		string path = EditorUtility.SaveFilePanel("Create The New GameStateHolder",
		                                          "Assets/Unity/ScriptableObjects/GameState", "GameStateHolder.asset", "asset");
		
		if (path == "")
			return;
		
		//get the location relative to the project
		path = FileUtil.GetProjectRelativePath(path);
		
		CreateGameStateHolderAt(path);
	}
	
	public static GameStateHolder CreateGameStateHolderAt(string path)
	{
		GameStateHolder gsHolder = CreateInstance<GameStateHolder>();
		AssetDatabase.CreateAsset(gsHolder, path);
		AssetDatabase.SaveAssets();
		return gsHolder;
	}
}
