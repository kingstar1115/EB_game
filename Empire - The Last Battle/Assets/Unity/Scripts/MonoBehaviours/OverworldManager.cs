using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class OverworldManager : MonoBehaviour
{
	public OverworldUI _OverworldUI;
	public CardList _AvailableCaveCards;
	public CardSystem _CardSystem;
	public Board _Board;
	public Player _BattlebeardPlayer;
    public Player _StormshaperPlayer;
    public Player _CurrentPlayer;
    public TurnManager _TurnManager;

	// Use this for initialization
	void Start() {
		//new game setup
		_Board.Initialise();
		_BattlebeardPlayer.Initialise();
		_StormshaperPlayer.Initialise();
		_BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.Scout);
		_BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.Scout);
		_BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.Catapult);
		_BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.Archer);
		_BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.AxeThrower);
		_BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.Ballista);
		_BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.Cavalry);
		_BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.Pikemen);
		_BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.Scout);
		_BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.Warrior);
		_OverworldUI.Initialise(_BattlebeardPlayer, _StormshaperPlayer);

		//try get the battleboard start tile
		if (_Board._BBStartTile != null) {
            _BattlebeardPlayer.CommanderPosition = _Board._BBStartTile;
        }else{
            Debug.LogError("Battlebeard start tile not set");
        }

        if(_Board._SSStartTile != null){
			_StormshaperPlayer.CommanderPosition = _Board._SSStartTile;
        }else{
            Debug.LogError("Stormshaper start tile not set");
        }

        _TurnManager.OnTurnStart += _TurnManager_OnTurnStart;
        _TurnManager.OnTurnEnd += _TurnManager_OnTurnEnd;
        _TurnManager.OnSwitchTurn += _TurnManager_OnSwitchTurn;

		//snap player to start position
        _OverworldUI.UpdateCommanderPosition();

        _OverworldUI._CommanderUI = _StormshaperPlayer.gameObject.GetComponent<CommanderUI>();

		_OverworldUI.Initialise(_BattlebeardPlayer, _StormshaperPlayer);

		_OverworldUI.UpdateCommanderPosition();

        _OverworldUI._CommanderUI = _BattlebeardPlayer.gameObject.GetComponent<CommanderUI>();

		//event listeners
		_OverworldUI.OnCommanderMove += _OverworldUI_OnCommanderMove;
        _OverworldUI.OnPause += _OverworldUI_OnPause;
        _OverworldUI.OnUnPause += _OverworldUI_OnUnPause;

		//allow player movement for the start ****JUST FOR TESTING****
        //_OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(_BattlebeardPlayer.Type, _BattlebeardPlayer.CommanderPosition, 1));

		_CurrentPlayer = _BattlebeardPlayer;

		_CardSystem.OnEffectApplied += _CardSystem_OnEffectApplied;

		_TurnManager.StartTurn();

		// TEST
		UseCard(_AvailableCaveCards.cards[0]);

        
	}

	void _CardSystem_OnEffectApplied(CardData card, Player player) {
        if (card.Type == CardType.Scout_Card) {
			Debug.Log(card.Value);
            _OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(player.Type, player.CommanderPosition, card.Value));
		}
	}

	void UseCard(CardData card) {
		_CardSystem.ApplyEffect(card, _CurrentPlayer);
	}

    void _OverworldUI_OnUnPause()
    {
        _OverworldUI._Paused = false;
    }

    void _OverworldUI_OnPause()
    {
        _OverworldUI._Paused = true;
    }

	void _OverworldUI_OnCommanderMove(TileData tile) {
		//set new position for the player (should depend on whose players turn it is)
		_CurrentPlayer.CommanderPosition = tile;

		//****JUST FOR TESTING**** set new reachable tiles
		_OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(_CurrentPlayer.Type, _CurrentPlayer.CommanderPosition, 1));

		//Handles events that happen when player lands on that tile
		HandleTileEvent(tile);
	}
    //This is temporary until we actually have things that happen after the move
    IEnumerator SwitchPlayer() {
        yield return new WaitForSeconds(2);
        _TurnManager.SwitchTurn();
    }

	void HandleTileEvent(TileData tile) {
		if (_CurrentPlayer.IsScouting) {
			_CurrentPlayer.IsScouting = false;
		} else {
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
					HealTroops(_CurrentPlayer);
					break;
				case BuildingType.StartTileBattlebeard:
					break;
				case BuildingType.StartTileStormshaper:
					break;
				default:
					break;
			}
		}

		_TurnManager.EndTurn();
		StartCoroutine(SwitchPlayer());
	}

	public void HealTroops(Player player) {
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

	public CardData GenerateRandomCard(List<CardData> availableCards) {
		//Generate a random card (Warning: This is weighted heavily towards resource cards because
		//there are more of them in the enum, change this later?)
		short randomCardIndex = (short)UnityEngine.Random.Range(0, availableCards.Count - 1);
		CardData card = availableCards [randomCardIndex];

		return card;
	}

    public void Pause()
    {
        _OverworldUI.Disable();
    }

    public void UnPause()
    {
        _OverworldUI.Enable();
    }

    void _TurnManager_OnTurnStart() {
        _OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(_CurrentPlayer.Type, _CurrentPlayer.CommanderPosition, 1));
    }

    void _TurnManager_OnTurnEnd() {
        _OverworldUI.DisablePlayerMovement();
    }

    void _TurnManager_OnSwitchTurn() {
        switch (_CurrentPlayer.Type) {
            case PlayerType.Battlebeard:
                _CurrentPlayer = _StormshaperPlayer;
                _OverworldUI._CommanderUI = _StormshaperPlayer.gameObject.GetComponent<CommanderUI>();
                _OverworldUI.SwitchFocus(_CurrentPlayer.transform);
                _TurnManager.StartTurn();
                break;
            case PlayerType.Stormshaper:
                _CurrentPlayer = _BattlebeardPlayer;
                _OverworldUI._CommanderUI = _BattlebeardPlayer.gameObject.GetComponent<CommanderUI>();
                _OverworldUI.SwitchFocus(_CurrentPlayer.transform);
                _TurnManager.StartTurn();
                break;
        }
    }

	// Update is called once per frame
	void Update() {

	}

}
