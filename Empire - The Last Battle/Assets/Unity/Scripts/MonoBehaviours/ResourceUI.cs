using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResourceUI : MonoBehaviour
{
	public Text ResourceText;
	public RawImage PlayerImage;

	Texture2D[] _textures = new Texture2D[2];

	void Start()
	{
		_textures[0] = Resources.Load<Texture2D>("Textures/Interface/BattlebeardSigilV1");
		_textures[1] = Resources.Load<Texture2D>("Textures/Interface/StormShapersSigilV1");
	}

	public void UpdateResources(Player currentPlayer)
	{
		ResourceText.text = currentPlayer.Currency.getPoints().ToString();
	}

	public void UpdatePlayerImage(Player currentPlayer)
	{
		if (currentPlayer.Type == PlayerType.Battlebeard)
		{
			//Change to player images once in
			PlayerImage.texture = _textures[0];
		}
		else if (currentPlayer.Type == PlayerType.Stormshaper)
		{
			PlayerImage.texture = _textures[1];
		}
		else
		{
			DebugUI.getUI().SetMessage("An Error has occurured in resourceUI; playerType cannot be none", 22, Color.red);
		}
	}
}
