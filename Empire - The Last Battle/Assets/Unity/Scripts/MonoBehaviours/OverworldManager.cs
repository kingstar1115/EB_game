using System.Collections.Generic;
using UnityEngine;

public class OverworldManager : MonoBehaviour
{
	public OverworldUI _OverworldUI;
	public CardData _AvailableCaveCards;
	public Board _Board;
	public Player _BattlebeardPlayer;
    public Inn _Inn;

	// Use this for initialization
	void Start() {
		//new game setup
		_Board.Initialise();
		_OverworldUI.Initialise();

		//try get the battleboard start tile
		if (_Board._BBStartTile != null)
			_BattlebeardPlayer.CommanderPosition = _Board._BBStartTile;
		else
			Debug.LogError("Battleboard start tile not set");

		//snap player to start position
		_OverworldUI.UpdateCommanderPosition();

		//event listeners
		_OverworldUI.OnCommanderMove += _OverworldUI_OnCommanderMove;

		//allow player movement for the start ****JUST FOR TESTING****
		_OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(_BattlebeardPlayer.CommanderPosition, 1));
	}

	void _OverworldUI_OnCommanderMove(TileData tile) {
		//set new position for the player (should depend on whose players turn it is)
		_BattlebeardPlayer.CommanderPosition = tile;

		//****JUST FOR TESTING**** set new reachable tiles
		_OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(_BattlebeardPlayer.CommanderPosition, 1));

		//Handles events that happen when player lands on that tile
		HandleTileEvent(tile);
	}

	void HandleTileEvent(TileData tile) {
		switch (tile.Building) {
			case BuildingType.None:
				break;
			case BuildingType.Armoury:
				break;
			case BuildingType.Camp:
				break;
			case BuildingType.CastleBattlebeard:
				break;
			case BuildingType.CastleStormshaper:
				break;
			case BuildingType.Cave:
				GenerateRandomCard(_AvailableCaveCards.cards);
				break;
			case BuildingType.Fortress:
				break;
			case BuildingType.Inn:
				//Needs changing to current player once both players are in this class
                _Inn.HealTroops(_BattlebeardPlayer);
				break;
			case BuildingType.StartTileBattlebeard:
				break;
			case BuildingType.StartTileStormshaper:
				break;
			default:
				break;
		}
	}

	public Cards GenerateRandomCard(List<Cards> availableCards) {
		//Generate a random card (Warning: This is weighted heavily towards resource cards because 
		//there are more of them in the enum, change this later?)
		short randomCardIndex = (short)UnityEngine.Random.Range(0, availableCards.Count - 1);
		var card = (Cards)randomCardIndex;
		return card;
	}

	// Update is called once per frame
	void Update() {

	}

}
