using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class DebugUI : MonoBehaviour
{
	public float _duration;
	public List<Text> _text;

	public void SetMessage(string message, int fontSize, Color fontColour)
	{
		//Get text element that is most faded
		var itemMostFaded = _text.OrderByDescending(x => x.gameObject.GetComponent<CanvasGroup>().alpha).Last();
		var lastItem = _text.Last();

		foreach (var item in _text)
		{
			//If debug text already is showing text then move to next text object
			if (item.text.Length > 0)
			{
				//If on last item, and each text before is full then go ahead and assign variables to most faded text element
				if (item.Equals(lastItem))
				{
					itemMostFaded.text = message;
					itemMostFaded.fontSize = fontSize;
					itemMostFaded.color = fontColour;
				}
				continue;
			}

			item.text = message;
			item.fontSize = fontSize;
			item.color = fontColour;

			//variables have been assigned to new text so return from loop
			//to make sure we don't assign more than 1 text object
			break;
		}
	}
}