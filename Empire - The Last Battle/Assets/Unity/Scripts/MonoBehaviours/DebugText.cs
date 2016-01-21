using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text), typeof(CanvasGroup))]
public class DebugText : MonoBehaviour
{
	private DebugUI _debugUI;
	private CanvasGroup _canvasGroup;
	private Text _text;
	private float _baseAlpha;
	private float _time;

	void Start()
	{
		_debugUI = GameObject.Find("DebugUI").GetComponent<DebugUI>();
		_text = gameObject.GetComponent<Text>();
		_canvasGroup = gameObject.GetComponent<CanvasGroup> ();
		_baseAlpha = 1f;
	}

	void Update()
	{
		_time += 0.01f;

		//If text has some text then start fading
		if (_text.text.Length > 0)
			Fade();
	}

	void Reset()
	{
		_text.text = "";
		_canvasGroup.alpha = _baseAlpha;
		_time = 0f;
	}

	void Fade()
	{
		//If text has fully faded out then reset text variables to default 
		if (_time > _debugUI._duration)
		{
			Reset();
			return;
		}

		float ratio = _time / _debugUI._duration;
		_canvasGroup.alpha = Mathf.Lerp(1, 0, ratio);
	}
}
