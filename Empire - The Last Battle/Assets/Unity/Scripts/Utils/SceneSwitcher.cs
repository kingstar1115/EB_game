using UnityEngine;
using System.Collections;

public static class SceneSwitcher
{
	/// <summary>
	/// Use StartCoroutine(ChangeScene()) to call, not just method name.
	/// </summary>
	/// <returns></returns>
	public static IEnumerator ChangeScene(string sceneName)
	{
		SceneFaderUI.ScreenFader.StartFadeOverTime(SceneFaderUI.FadeDir.FadeIn);
		yield return new WaitForSeconds(SceneFaderUI.ScreenFader.FadeTime);
		Application.LoadLevel(sceneName);
	}

	public static IEnumerator ChangeScene(int sceneBuildIndex)
	{
		SceneFaderUI.ScreenFader.StartFadeOverTime(SceneFaderUI.FadeDir.FadeIn);
		yield return new WaitForSeconds(SceneFaderUI.ScreenFader.FadeTime);
		Application.LoadLevel(sceneBuildIndex);
	}
}