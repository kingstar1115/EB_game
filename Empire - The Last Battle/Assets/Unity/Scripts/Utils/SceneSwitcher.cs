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
        yield return new WaitForEndOfFrame();
        PreviousScene.screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        PreviousScene.screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        PreviousScene.screenshot.Apply();
        //SceneFaderUI.ScreenFader.StartFadeOverTime(SceneFaderUI.FadeDir.FadeIn);
        //yield return new WaitForSeconds(SceneFaderUI.ScreenFader.FadeTime);
        Application.LoadLevel(sceneName);
    }

    public static IEnumerator ChangeScene(int sceneBuildIndex)
    {
        yield return new WaitForEndOfFrame();
        PreviousScene.screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        PreviousScene.screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        PreviousScene.screenshot.Apply();
        //SceneFaderUI.ScreenFader.StartFadeOverTime(SceneFaderUI.FadeDir.FadeIn);
        //yield return new WaitForSeconds(SceneFaderUI.ScreenFader.FadeTime);
        Application.LoadLevel(sceneBuildIndex);
    }
}