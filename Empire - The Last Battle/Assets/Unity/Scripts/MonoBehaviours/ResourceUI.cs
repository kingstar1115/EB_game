using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResourceUI : MonoBehaviour
{
	public Transform InTransform;
	public Transform OutTransform;
	public Text ResourceText;
	public Image SSImage, BBImage;
	public GameObject Images;
	bool showing;

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

	public void Show(){
		showing = true;
		Images.GetComponent<LerpPosition>().LerpTo(new Vector3(Images.transform.position.x, InTransform.position.y));
	}

	public void Hide(){
		showing = false;
		Images.GetComponent<LerpPosition>().LerpTo(new Vector3(Images.transform.position.x, OutTransform.position.y));
	}
}
