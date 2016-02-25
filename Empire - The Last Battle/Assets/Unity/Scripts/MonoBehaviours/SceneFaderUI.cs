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

	public CanvasGroup Canvas;
	[Tooltip("Time to fade in seconds")]
	public float FadeTime;

	private float _alpha = 0.0f;
	private const float _startAlpha = 0f;
	private const float _endAlpha = 1.0f;
	float currentTime;
	bool fading = false;
	FadeDir fadeDir;

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


	public void StartFadeOverTime(FadeDir dir)
	{
		_alpha = Canvas.alpha;
		fading = true;
		currentTime = 0;
		fadeDir = dir;
	}

	void Update() {

		if (!fading)
			return;

		currentTime += Time.deltaTime;
		float normalizedTime = currentTime / FadeTime;
		//right here, you can now use normalizedTime as the third parameter in any Lerp from start to end
		if (fadeDir == FadeDir.FadeIn)
			_alpha = Mathf.Lerp(_startAlpha, _endAlpha, normalizedTime);
		else
			_alpha = Mathf.Lerp(_endAlpha, _startAlpha, normalizedTime);

		if (_alpha >= 1 && fadeDir == FadeDir.FadeIn) {
			_alpha = _endAlpha;
			fading = false;
			currentTime = 0;
		}
		//without this, the value will end at something like 0.9992367
		else if (_alpha <= 0 && fadeDir == FadeDir.FadeOut) {
			_alpha = _startAlpha;
			fading = false;
			currentTime = 0;
		}

		Canvas.alpha = _alpha;
	}
}