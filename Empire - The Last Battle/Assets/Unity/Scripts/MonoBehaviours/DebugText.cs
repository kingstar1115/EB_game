using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
	private DebugUI _debugUI;
	private Text _text;
	private Color _baseColor;
	private float _time;

	void Start()
	{
		_debugUI = GameObject.Find("DebugUI").GetComponent<DebugUI>();
		_text = gameObject.GetComponent<Text>();
		_baseColor = _text.color;
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
		_text.material.color = _baseColor;
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

		Color myColor = _text.material.color;
		float ratio = _time / _debugUI._duration;
		myColor.a = Mathf.Lerp(1, 0, ratio);
		_text.material.color = myColor;
	}
}
