using UnityEngine;
using UnityEditor;

public class UnitTypeEditor : Editor 
{

	[MenuItem("Assets/Create/ELB/Units/UnitTypeData")]
	public static void CreateUnit()
	{
		//get a save location
		string path = EditorUtility.SaveFilePanel("Create The New Asset",
		                                          "Assets/Unity/ScriptableObjects/Units/Types", "default.asset", "asset");
		
		if (path == "")
			return;
		
		//get the location relative to the project
		path = FileUtil.GetProjectRelativePath(path);
		
		CreateUnitTypeDataAt(path);
	}
	
	public static UnitTypeData CreateUnitTypeDataAt(string path)
	{
		UnitTypeData UnitType = CreateInstance<UnitTypeData>();
		AssetDatabase.CreateAsset(UnitType, path);
		AssetDatabase.SaveAssets();
		return UnitType;
	}
}
