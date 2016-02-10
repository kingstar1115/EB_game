using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResourceUI : MonoBehaviour
{
	public Text ResourceText;
	public Image SSImage, BBImage;

	public void UpdateResources(int val)
	{
		ResourceText.text = val.ToString();
	}

	public void UpdatePlayerImage(Player currentPlayer)
	{
		if (currentPlayer.Type == PlayerType.Battlebeard)
		{
			//Change to player images once in
			BBImage.enabled = true;
			SSImage.enabled = false;
		}
		else if (currentPlayer.Type == PlayerType.Stormshaper)
		{
			BBImage.enabled = false;
			SSImage.enabled = true;
		}
		else
		{
			DebugUI.getUI().SetMessage("An Error has occurured in resourceUI; playerType cannot be none", 22, Color.red);
		}
	}
}
