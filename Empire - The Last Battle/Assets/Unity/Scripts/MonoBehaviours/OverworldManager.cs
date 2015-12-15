using System.Collections.Generic;
using UnityEngine;

public class OverworldManager : MonoBehaviour
{
	public OverworldUI _OverworldUI;
	public CardData _AvailableCaveCards;
	public CardSystem _CardSystem;
	public Board _Board;
	public Player _BattlebeardPlayer;

	// Use this for initialization
	void Start() {
		//new game setup
		_Board.Initialise();
		_OverworldUI.Initialise();
		_BattlebeardPlayer.Initialise();

		//try get the battleboard start tile
		if (_Board._BBStartTile != null)
			_BattlebeardPlayer.CommanderPosition = _Board._BBStartTile;
		else
			Debug.LogError("Battlebeard start tile not set");

		//snap player to start position
		_OverworldUI.UpdateCommanderPosition();

		//event listeners
		_OverworldUI.OnCommanderMove += _OverworldUI_OnCommanderMove;

		//allow player movement for the start ****JUST FOR TESTING****
		//_OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(_BattlebeardPlayer.CommanderPosition, 1));


		_BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.Scout);
		_BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.Scout);

		_CardSystem.OnEffectApplied += _CardSystem_OnEffectApplied;
		// TEST
		UseCard(Cards.Scout_Card_1);
	}

	void _CardSystem_OnEffectApplied(Cards card) {
		if (card == Cards.Scout_Card_1) {
			int availableScouts = _BattlebeardPlayer.PlayerArmy.GetUnits(UnitType.Scout).Count;
			if (availableScouts > 0) {
				Mathf.Clamp(availableScouts++, 2, 4);
				// Make sure these are reset the next turn!
				_BattlebeardPlayer.IsScouting = true;
				_OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(_BattlebeardPlayer.CommanderPosition, availableScouts));
			}
		}
	}

	void UseCard(Cards card) {
		_CardSystem.ApplyEffect(card, _BattlebeardPlayer);
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
		if (_BattlebeardPlayer.IsScouting) {
			_BattlebeardPlayer.IsScouting = false;
			return;
		}
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
                HealTroops(_BattlebeardPlayer);
				break;
			case BuildingType.StartTileBattlebeard:
				break;
			case BuildingType.StartTileStormshaper:
				break;
			default:
				break;
		}
	}

	public void HealTroops(Player player)
	{
		//Change magic number to function once more castle code is in
		if (player.CastleProgress >= 4)
			return;

		var rnd = UnityEngine.Random.Range(0, 3);
		List<Unit> units;

		units = player.PlayerArmy.GetRandomUnits(rnd, true);

		foreach (var unit in units)
		{
			unit.Heal();
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
