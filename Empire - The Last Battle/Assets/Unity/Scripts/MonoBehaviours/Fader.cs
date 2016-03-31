using UnityEngine;
using System.Collections;
using System;

public class Fader : MonoBehaviour 
{
	public enum FadeDir
	{
		FadeIn,
		FadeOut
	}

	[Tooltip("Time to fade in seconds")]
	public float FadeTime;
	public bool FadeInInstantly = false;
	public bool FadeOutInstantly = false;
	public float TimeToStartFade;
	private CanvasGroup _canvasGroup;
	private float _alpha = 0.0f;
	private const float _startAlpha = 0f;
	private const float _endAlpha = 1f;
	float _currentTime;
	bool _fading = false;
	FadeDir _fadeDir;
	
	static Fader _screenFader;
	public static Fader ScreenFader
	{
		get
		{
			return _screenFader;
		}
	}
	
	void Awake()
	{
		_screenFader = this;
		_canvasGroup = GetComponent<CanvasGroup> ();
	}

	void Start()
	{
		if (FadeInInstantly)
			Invoke ("StartFadeOverTime", TimeToStartFade);
	}

	public void StartFadeOverTime()
	{
		_fading = true;
		_currentTime = 0;
	}

	public void StartFadeOverTime(FadeDir dir, Action action)
	{
		action ();

		_fading = true;
		_fadeDir = dir;
		_currentTime = 0;
	}

	private void Fade()
	{	
		if (!_fading)
			return;

		_currentTime += Time.deltaTime;
		float normalizedTime = _currentTime / FadeTime;
		//right here, you can now use normalizedTime as the third parameter in any Lerp from start to end
		if (_fadeDir == FadeDir.FadeIn)
			_alpha = Mathf.Lerp(_startAlpha, _endAlpha, normalizedTime);
		else
			_alpha = Mathf.Lerp(_endAlpha, _startAlpha, normalizedTime);
		
		if (_alpha >= 1 && _fadeDir == FadeDir.FadeIn) {
			_alpha = _endAlpha;
			_fading = false;
			_currentTime = 0;

			if (FadeOutInstantly)
				StartFadeOverTime(FadeDir.FadeOut, () => { });
		}
		//without this, the value will end at something like 0.9992367
		else if (_alpha <= 0 && _fadeDir == FadeDir.FadeOut) {
			_alpha = _startAlpha;
			_fading = false;
			_currentTime = 0;
		}
		
		_canvasGroup.alpha = _alpha;
	}
	
	void Update() 
	{
		Fade ();
	}
}
