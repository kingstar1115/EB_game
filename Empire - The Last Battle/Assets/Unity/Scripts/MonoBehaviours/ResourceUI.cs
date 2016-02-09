using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResourceUI : MonoBehaviour
{
	public Text ResourceText;
	public Image PlayerImage;
	public OverworldManager OverWorld;

	void Update ()
	{
		//If debug build then lets test UI
		if (Debug.isDebugBuild)
		{
			if (Input.GetKeyDown(KeyCode.Alpha7))
			{
				OverWorld._CurrentPlayer.Currency.addPoints(10);
			}
			else if(Input.GetKeyDown(KeyCode.Alpha6))
			{
				OverWorld._CurrentPlayer.Currency.addPoints(-20);
            }
		}
	}

	void OnGUI()
	{
		ResourceText.text = OverWorld._CurrentPlayer.Currency.getPoints().ToString();
		//Need to change player image as well when it's put in
	}
}
