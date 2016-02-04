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
    public Player _StormshapersPlayer;
    public Player _CurrentPlayer;
    public TurnManager _TurnManager;
	public GameStateHolder _GameStateHolder;

	//****TESTS ONLY****
	public CardList _StartCards;

	// Use this for initialization
	void Start() {
		//new game setup
		_Board.Initialise();
		_OverworldUI.Initialise();
		_BattlebeardPlayer.Initialise();

		//try get the battleboard start tile
		if (_Board._BBStartTile != null) {
            _BattlebeardPlayer.CommanderPosition = _Board._BBStartTile;
        }else{
            Debug.LogError("Battleboard start tile not set");
        }

        if(_Board._SSStartTile != null){
            _StormshapersPlayer.CommanderPosition = _Board._SSStartTile;
        }else{
            Debug.LogError("Stormshaper start tile not set");
        }

        _TurnManager.OnTurnStart += _TurnManager_OnTurnStart;
        _TurnManager.OnTurnEnd += _TurnManager_OnTurnEnd;
        _TurnManager.OnSwitchTurn += _TurnManager_OnSwitchTurn;

		//snap player to start position
        _OverworldUI.UpdateCommanderPosition();

        _OverworldUI._CommanderUI = _StormshapersPlayer.gameObject.GetComponent<CommanderUI>();

        _OverworldUI.Initialise();

        _OverworldUI.UpdateCommanderPosition();

        _OverworldUI._CommanderUI = _BattlebeardPlayer.gameObject.GetComponent<CommanderUI>();

		//event listeners
		_OverworldUI.OnCommanderMove += _OverworldUI_OnCommanderMove;
        _OverworldUI.OnPause += _OverworldUI_OnPause;
        _OverworldUI.OnUnPause += _OverworldUI_OnUnPause;
        _OverworldUI.OnPlayerUseCard += _OverworldUI_OnPlayerUseCard;
		_BattlebeardPlayer.OnCardAdded += _BattlebeardPlayer_OnCardAdded;
		_BattlebeardPlayer.OnCardRemoved += _BattlebeardPlayer_OnCardRemoved;
		_StormshapersPlayer.OnCardAdded += _StormshapersPlayer_OnCardAdded;
		_StormshapersPlayer.OnCardRemoved += _StormshapersPlayer_OnCardRemoved;

		//allow player movement for the start ****JUST FOR TESTING****
        //_OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(_BattlebeardPlayer.Type, _BattlebeardPlayer.CommanderPosition, 1));


		_BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.Scout);
		_BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.Scout);

		_CardSystem.OnEffectApplied += _CardSystem_OnEffectApplied;

		// TEST
        _CurrentPlayer = _BattlebeardPlayer;
        //set the player in focus
        _OverworldUI.SetPlayerFocus(_CurrentPlayer);

		//test by adding a scout card.
		_CurrentPlayer.SetCards (_StartCards);
        UseCard(_CurrentPlayer.Hand.GetCardOfType(CardType.Scout_Card));
        _TurnManager.StartTurn();
		_GameStateHolder._gameState = GameState.Overworld;
        
	}

    void _OverworldUI_OnPlayerUseCard(CardData card)
    {
        UseCard(card);
    }

	void _BattlebeardPlayer_OnCardAdded(CardData card)
	{
		//inform ui
		_OverworldUI.AddPlayerCard (PlayerType.Battlebeard, card);
	}

	void _BattlebeardPlayer_OnCardRemoved(CardData card)
	{
		//inform ui
		_OverworldUI.RemovePlayerCard (PlayerType.Battlebeard, card);
	}

	void _StormshapersPlayer_OnCardAdded(CardData card)
	{
		//inform ui
		_OverworldUI.AddPlayerCard (PlayerType.Stormshaper, card);
	}

	void _StormshapersPlayer_OnCardRemoved(CardData card)
	{
		//inform ui
		_OverworldUI.RemovePlayerCard (PlayerType.Stormshaper, card);
	}

	void _CardSystem_OnEffectApplied(CardData card, Player player) {
        if (card.Type == CardType.Scout_Card)
        {
            _OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(player.Type, player.CommanderPosition, card.Value));
		}
	}

	void UseCard(CardData card) {
		//use the card
		if (_CardSystem.CanUseCard (card, _GameStateHolder._gameState)) 
		{
			_CardSystem.ApplyEffect (card, _CurrentPlayer);
			_CurrentPlayer.RemoveCard(card);
		}
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
		if (_BattlebeardPlayer.IsScouting) {
			_BattlebeardPlayer.IsScouting = false;
			_TurnManager.EndTurn();
			StartCoroutine(SwitchPlayer());
			return;
		}
		switch (tile.Building) {
			case BuildingType.None:
                _TurnManager.EndTurn();
                StartCoroutine(SwitchPlayer());
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
        switch (_CurrentPlayer.tag) {
            case "CommanderBB":
                _CurrentPlayer = _StormshapersPlayer;
                _OverworldUI.SetPlayerFocus(_StormshapersPlayer);
                _TurnManager.StartTurn();
                break;
            case "CommanderSS":
                _CurrentPlayer = _BattlebeardPlayer;
                _OverworldUI.SetPlayerFocus(_BattlebeardPlayer);
                _TurnManager.StartTurn();
                break;
        }
    }

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Return)) {
			StartCoroutine(SwitchPlayer());
		}

		//Delete in final build. Used for testing, an example of how to call debug message class
		if (Input.GetKeyDown (KeyCode.Alpha8)) {
			DebugUI.getUI ().SetMessage ("Test", 22, Color.green);
		}
	}
}

