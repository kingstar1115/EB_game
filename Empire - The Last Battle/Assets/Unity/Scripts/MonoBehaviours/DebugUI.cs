using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class DebugUI : MonoBehaviour
{
	public float _duration;
	public Pool TextPool;
	static DebugUI ui;

	void Awake() {
		ui = this;
	}

	public static DebugUI getUI() {
		return ui;
	}

	public void SetMessage(string message, int fontSize, Color fontColour)
	{
		GameObject newTextObject = TextPool.GetPooledObject();
		Text newText = newTextObject.GetComponent<Text>();
		newTextObject.transform.SetParent(this.transform);
		newText.text = message;
		newText.fontSize = fontSize;
		newText.color = fontColour;
		newTextObject.SetActive(true);
	}
}