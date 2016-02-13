using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneFaderUI : MonoBehaviour
{
	public enum FadeDir
	{
		FadeIn,
		FadeOut
	}

	public RawImage FadeInOutTexture;
	[Tooltip("Time to fade in seconds")]
	public float FadeTime;

	private float _alpha = 0.0f;
	private const float _startAlpha = 0f;
	private const float _endAlpha = 1.0f;

	static SceneFaderUI _screenFader;
	public static SceneFaderUI ScreenFader
	{
		get
		{
			return _screenFader;
		}
	}

	void Awake()
	{
		_screenFader = this;
	}

	void OnGUI()
	{
		FadeInOutTexture.color = new Color(FadeInOutTexture.color.r, FadeInOutTexture.color.g, FadeInOutTexture.color.b, _alpha);
	}

	public void StartFadeOverTime(FadeDir fadeDir)
	{
		StartCoroutine(Fade(fadeDir));
	}

	IEnumerator Fade(FadeDir fadeDir)
	{
		for (float t = 0f; t < FadeTime; t += Time.deltaTime)
		{
			float normalizedTime = t / FadeTime;
			//right here, you can now use normalizedTime as the third parameter in any Lerp from start to end
			if (fadeDir == FadeDir.FadeIn)
				_alpha = Mathf.Lerp(_startAlpha, _endAlpha, normalizedTime);
			else
				_alpha = Mathf.Lerp(_endAlpha, _startAlpha, normalizedTime);

			yield return null;
		}

		if (fadeDir == FadeDir.FadeIn)
			_alpha = _endAlpha; //without this, the value will end at something like 0.9992367
		else
			_alpha = _startAlpha;
	}

	void OnLevelWasLoaded()
	{
		_alpha = _endAlpha;
		StartFadeOverTime((FadeDir.FadeOut));
	}
}