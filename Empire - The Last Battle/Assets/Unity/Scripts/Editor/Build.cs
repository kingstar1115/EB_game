// C# example
using UnityEditor;
class MyEditorScript
{
	static void PerformBuild ()
	{
		string[] scenes = { "Assets/Unity/Scenes/Overworld.unity", "Assets/Unity/Scenes/BattleScene.unity", "Assets/Unity/Scenes/StartMenu.unity" };
		BuildPipeline.BuildPlayer (scenes, "WebPlayerBuild", BuildTarget.WebPlayer, BuildOptions.None);
	}
}