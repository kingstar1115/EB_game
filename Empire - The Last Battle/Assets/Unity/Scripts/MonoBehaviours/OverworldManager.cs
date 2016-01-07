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
	public Player _InactivePlayer;
    public TurnManager _TurnManager;

	// Use this for initialization
	void Start() {
		//new game setup
		_Board.Initialise();
		_BattlebeardPlayer.Initialise();
		_StormshaperPlayer.Initialise();

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

		_OverworldUI.Initialise(_BattlebeardPlayer, _StormshaperPlayer);

		_TurnManager.OnTurnStart += _TurnManager_OnTurnStart;
        _TurnManager.OnTurnEnd += _TurnManager_OnTurnEnd;
        _TurnManager.OnSwitchTurn += _TurnManager_OnSwitchTurn;

		//event listeners
		_OverworldUI.OnCommanderMove += _OverworldUI_OnCommanderMove;
        _OverworldUI.OnPause += _OverworldUI_OnPause;
        _OverworldUI.OnUnPause += _OverworldUI_OnUnPause;

		setPlayer(PlayerType.Battlebeard);

		_CardSystem.OnEffectApplied += _CardSystem_OnEffectApplied;
        _TurnManager.StartTurn();
	}

	void _CardSystem_OnEffectApplied(CardData card, Player player) {
        if (card.Type == CardType.Scout_Card) {
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
        yield return new WaitForSeconds(1);
        _TurnManager.SwitchTurn();
		_OverworldUI.Enable();
    }

	void HandleTileEvent(TileData tile) {		
		if (_CurrentPlayer.IsScouting) {
			_CurrentPlayer.IsScouting = false;
			endTurn();
		} else {
			_OverworldUI.Disable();
			ModalPanel p = ModalPanel.Instance();
			switch (tile.Building) {
				case BuildingType.Armoury:
					p.ShowOK("Armoury", "You landed on the Armoury.", endTurn);
					break;
				case BuildingType.Camp:
					if (tile.Owner != _CurrentPlayer.Type) {
						if (tile.Owner == PlayerType.None) {
							// MONSTER BATTLE
						} else {
							// PVP BATTLE
						}

						// WIN
						_Board.SetTileOwner(tile, _CurrentPlayer.Type);
					}
					p.ShowOK("Camp", "You landed on a camp.", endTurn);
					break;
				case BuildingType.Cave:
					if (tile.Owner != _CurrentPlayer.Type) {
						if (tile.Owner != PlayerType.None) {
							//BATTLE
						} else {
							// WIN ANYWAY
						}

						// WIN
						CardData c = GenerateRandomCard(_AvailableCaveCards.cards);
						_CurrentPlayer.Hand.cards.Add(c);
						_Board.SetTileOwner(tile, _CurrentPlayer.Type);
						p.ShowOK("Card Recieved!", "You recieved a " + c.Type + " card.", endTurn);
					}
					break;
				case BuildingType.Fortress:
					p.ShowOK("Fortress", "You landed on a fortress.", endTurn);
					break;
				case BuildingType.Inn:
					if (_InactivePlayer.CastleProgress >= 4) {
						p.ShowOK("Oh No!", "The inn won't accept you!", endTurn);
						break;
					}

					var rnd = UnityEngine.Random.Range(0, 3);
					List<Unit> units;

					units = _CurrentPlayer.PlayerArmy.GetRandomUnits(rnd, true);

					foreach (var unit in units) {
						unit.Heal();
					}

					string content, title;

					if (_CurrentPlayer.PlayerArmy.GetUnits().Count == 0) {
						title = "The Inn Welcomes You";
						content = "You are well rested.";
					} else {
						title = units.Count + " Units Healed";
						content = rnd == 0 ? 
						"Their wounds were too great. Looks like they'll need some more time." :
						"Your army is well rested.";
					}
					p.ShowOK(title, content, endTurn);
					break;
				default:
					endTurn();
					break;
			}
		}	
		
	}

	void endTurn() {	
		_TurnManager.EndTurn();
		StartCoroutine(SwitchPlayer());
	}

	public CardData GenerateRandomCard(List<CardData> availableCards) {
		//Generate a random card (Warning: This is weighted heavily towards resource cards because
		//there are more of them in the enum, change this later?)
		short randomCardIndex = (short)Random.Range(0, availableCards.Count - 1);
		return availableCards[randomCardIndex];
	}

    public void Pause()
    {
        _OverworldUI.Disable();
		_OverworldUI.Hide();
    }

    public void UnPause()
    {
        _OverworldUI.Enable();
		_OverworldUI.Show();
    }

    void _TurnManager_OnTurnStart() {
        _OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(_CurrentPlayer.Type, _CurrentPlayer.CommanderPosition, 1));


		// test add unit each turn
		int rand = Random.Range(0, 8);
		_CurrentPlayer.PlayerArmy.AddUnit((UnitType)rand);

		// chance of knocking out a random unit each turn

		int c = _CurrentPlayer.PlayerArmy.GetUnits().Count;
		rand = Random.Range(0, (int)c * 5);
		if (rand < c) {
			_CurrentPlayer.PlayerArmy.GetUnits()[rand].ReduceHP(100);
		}
	}

    void _TurnManager_OnTurnEnd() {
        _OverworldUI.DisablePlayerMovement();
    }

	void setPlayer(PlayerType p) {
		_CurrentPlayer = p == PlayerType.Battlebeard ? _BattlebeardPlayer : _StormshaperPlayer;
		_InactivePlayer = p == PlayerType.Battlebeard ? _StormshaperPlayer : _BattlebeardPlayer;
		_OverworldUI.SetPlayer(_CurrentPlayer);
	}

    void _TurnManager_OnSwitchTurn() {
		setPlayer(_CurrentPlayer.Type == PlayerType.Battlebeard ? 
			PlayerType.Stormshaper : PlayerType.Battlebeard);
		_TurnManager.StartTurn();
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Return)) {
			StartCoroutine(SwitchPlayer());
		}
	}

}
