using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text), typeof(CanvasGroup))]
public class DebugText : MonoBehaviour
{
	public float BaseAlpha 
	{
		get 
		{
			return _baseAlpha;
		}
	}
	public float Time { get; set;}

	private float _baseAlpha;
	private DebugUI _debugUI;
	private CanvasGroup _canvasGroup;
	private Text _text;

	void Start()
	{
		_debugUI = GameObject.Find("DebugUI").GetComponent<DebugUI>();
		_text = gameObject.GetComponent<Text>();
		_canvasGroup = gameObject.GetComponent<CanvasGroup> ();
		_baseAlpha = 1f;
	}

	void Update()
	{
		//If text has some text then start countdown and fading
		if (_text.text.Length > 0) {
			Time += 0.01f;
			Fade();
		}
	}

	void Reset()
	{
		_text.text = "";
		_canvasGroup.alpha = _baseAlpha;
		Time = 0f;
		this.gameObject.SetActive(false);
	}

	void Fade()
	{
		//If text has fully faded out then reset text variables to default 
		if (Time > _debugUI._duration)
		{
			Reset();
			return;
		}

		float ratio = Time / _debugUI._duration;
		_canvasGroup.alpha = Mathf.Lerp(1, 0, ratio);
	}
}
