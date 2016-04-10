// C# example
using UnityEditor;
class MyEditorScript
{
	static void PerformBuild ()
	{
		string[] scenes = { 
			"Assets/Unity/Scenes/StartMenu.unity" ,
			"Assets/Unity/Scenes/Overworld.unity", 
			"Assets/Unity/Scenes/BattleScene.unity"
		};
		BuildPipeline.BuildPlayer (scenes, "../Build", BuildTarget.WebPlayer, BuildOptions.None);
	}
}