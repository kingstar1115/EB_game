using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;

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
	public Audio _Audio;
	public TileHolder tile;
	public GameStateHolder _GameStateHolder;
	//****TESTS ONLY****
	public CardList _StartCards;

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

        //test by adding a scout card.
        _BattlebeardPlayer.SetCards(_StartCards);
        _BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.Scout);
        _BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.Scout);
        _BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.Scout);
        _StormshaperPlayer.SetCards(_StartCards);
        _StormshaperPlayer.PlayerArmy.AddUnit(UnitType.Scout);
        _StormshaperPlayer.PlayerArmy.AddUnit(UnitType.Scout);
        _StormshaperPlayer.PlayerArmy.AddUnit(UnitType.Scout);

		_OverworldUI.Initialise(_BattlebeardPlayer, _StormshaperPlayer);

        _TurnManager.OnTurnStart += _TurnManager_OnTurnStart;
        _TurnManager.OnTurnEnd += _TurnManager_OnTurnEnd;
        _TurnManager.OnSwitchTurn += _TurnManager_OnSwitchTurn;


		//event listeners
		_OverworldUI.OnCommanderMove += _OverworldUI_OnCommanderMove;
		_OverworldUI.OnCommanderForceMove += _OverworldUI_OnCommanderForceMove;
        _OverworldUI.OnPause += _OverworldUI_OnPause;
        _OverworldUI.OnUnPause += _OverworldUI_OnUnPause;
        _OverworldUI.OnPlayerUseCard += _OverworldUI_OnPlayerUseCard;
		_BattlebeardPlayer.Currency.OnChange += _OverworldUI._ResourceUI.UpdateResources;
		_BattlebeardPlayer.OnCardAdded += _BattlebeardPlayer_OnCardAdded;
		_BattlebeardPlayer.OnCardRemoved += _BattlebeardPlayer_OnCardRemoved;
		_StormshaperPlayer.Currency.OnChange += _OverworldUI._ResourceUI.UpdateResources;
		_StormshaperPlayer.OnCardAdded += _StormshapersPlayer_OnCardAdded;
		_StormshaperPlayer.OnCardRemoved += _StormshapersPlayer_OnCardRemoved;

		_CardSystem.RequestUnitSelection +=_CardSystem_RequestUnitSelection;

		_BattlebeardPlayer.OnCastleProgress += _Board.SetCastleState;
		_StormshaperPlayer.OnCastleProgress += _Board.SetCastleState;
		_BattlebeardPlayer.CastleProgress = 4;
		_StormshaperPlayer.CastleProgress = 4;
		setPlayer(PlayerType.Battlebeard);

		_CardSystem.OnEffectApplied += _CardSystem_OnEffectApplied;

		_GameStateHolder._gameState = GameState.Overworld;

        _TurnManager.StartTurn();    
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

	// get rid of this shit later
	void cardNotUsedModal () {
		ModalPanel.Instance().ShowOK("Oh No!", "Card could not be used", null);
	}

	void _CardSystem_OnEffectApplied(bool success, CardData card, Player player, Unit u) {
		ModalPanel p = ModalPanel.Instance();
		if (!success) {
			// we wont bother with this in the final but for now it hides the previous modal after it shows this one.
			Invoke("cardNotUsedModal", 1);
			return;
		}
		if (card.Type == CardType.Scout_Card) {
            _OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(player.Type, player.CommanderPosition, card.Value));
		}
		ModalPanel.Instance().ShowOK("Yeah!", "Used " + card.Type, null);
		_CurrentPlayer.RemoveCard(card);
	}

	void UseCard(CardData card) {

		ModalPanel p = ModalPanel.Instance();
		p.ShowOK("Card", "Using " + card.Type, () => {
			_CardSystem.UseCard(card, _CurrentPlayer, _InactivePlayer);
		});


		//use the card
		//if (_CardSystem.CanUseCard (card, _GameStateHolder._gameState))
		//{
		//	_CardSystem.ApplyEffect (card, _CurrentPlayer);
		//}

	}

    void _OverworldUI_OnUnPause()
    {
        _OverworldUI._Paused = false;
    }

    void _OverworldUI_OnPause()
    {
        _OverworldUI._Paused = true;
    }

	void _OverworldUI_OnCommanderForceMove(TileData tile) {
		//****JUST FOR TESTING**** set new reachable tiles
		_OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(_CurrentPlayer.Type, _CurrentPlayer.CommanderPosition, 1));
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
        _OverworldUI.Disable();
		if (_CurrentPlayer.IsScouting) {
			_CurrentPlayer.IsScouting = false;
			endTurn ();
		} else {
			ModalPanel p = ModalPanel.Instance ();
			switch (tile.Building) {
			case BuildingType.Armoury:
				p.ShowOK ("Armoury", "You landed on the Armoury.", endTurn);
				break;
			case BuildingType.Camp:
				if (tile.Owner != _CurrentPlayer.Type) {
					if (tile.Owner == PlayerType.None) {
						// MONSTER BATTLE
					} else {
						// PVP BATTLE
					}

					// WIN
					_Board.SetTileOwner (tile, _CurrentPlayer.Type);
				}
				p.ShowOK ("Camp", "You landed on a camp.", endTurn);
				break;
			case BuildingType.Cave:
				if (tile.Owner != _CurrentPlayer.Type) {
					if (tile.Owner != PlayerType.None) {
						//BATTLE
					} else {
						// WIN ANYWAY
					}

					// WIN
					CardData c = GenerateRandomCard (_AvailableCaveCards.cards);
					_CurrentPlayer.AddCard(c);
					_Board.SetTileOwner (tile, _CurrentPlayer.Type);
					p.ShowOK ("Card Recieved!", "You recieved a " + c.Type + " card.", endTurn);
				} else {
					endTurn ();
				}
				break;
			case BuildingType.Fortress:
					// Make sure that the fortress type matches the player type
				if (tile.Owner == _CurrentPlayer.Type) {
					// make sure the player owns at least 3 surrounding tiles
					if (tile.GetConnectedTiles ().FindAll (t => t.Owner == _CurrentPlayer.Type).Count >= 3) {
						// Battle lost immortal
						// on victory ->
						tile.Owner = PlayerType.None;
						_CurrentPlayer.LostImmortalKillCount++;
						_CurrentPlayer.CastleProgress++;
						p.ShowOK ("Fortress", "A bloody Lost Immortal just showed up innit blud!", endTurn);
					} else {
						p.ShowOK ("Fortress", "You felt a chill in the air, but nothing appeared.", endTurn);
					}
					break;
				}
				p.ShowOK ("Fortress", "Nothing appeared.", endTurn);
					// end turn
				break;
			case BuildingType.Inn:
				if (_InactivePlayer.CastleProgress >= 4) {
					p.ShowOK ("Oh No!", "The inn won't accept you!", endTurn);
					break;
				}

				var rnd = UnityEngine.Random.Range (0, 3);
				List<Unit> units;

				units = _CurrentPlayer.PlayerArmy.GetRandomUnits (rnd, true);

				foreach (var unit in units) {
					unit.Heal ();
				}

				string content, title;

				if (_CurrentPlayer.PlayerArmy.GetKOUnits ().Count == 0) {
					title = "The Inn Welcomes You";
					content = "You are well rested.";
				} else {
					title = units.Count + " Units Healed";
					content = rnd == 0 ?
						"Their wounds were too great. Looks like they'll need some more time." :
						"Your army is well rested.";
				}
				p.ShowOK (title, content, endTurn);
				break;
			default:
				endTurn ();
				break;
			}
		}
	}

	void endTurn() {
		_TurnManager.EndTurn ();
		StartCoroutine (SwitchPlayer ());
	}

	public CardData GenerateRandomCard(List<CardData> availableCards) {
		//Generate a random card
		List<CardType> uniqueTypes = availableCards.Select(x => x.Type).Distinct().ToList();
		int randomTypeIndex = Random.Range(0, uniqueTypes.Count - 1);
		List<CardData> cardsOfType = availableCards.FindAll(x => x.Type == uniqueTypes[randomTypeIndex]);
		int randomCardIndex = (short)Random.Range(0, cardsOfType.Count - 1);
		return cardsOfType[randomCardIndex];
	}

	void _CardSystem_RequestUnitSelection(CardData c, int numSelection, Player p, CardAction action, EndCardAction done) {
		// assume P is going to be the current player

		UnitSelection flags = UnitSelection.None;
		if (c.Type == CardType.Healing_Card) {
			flags = flags | UnitSelection.Inactive;
		}
		if (c.Type == CardType.Upgrade_Card) {
			flags = flags | UnitSelection.Active;
			flags = flags | UnitSelection.NotUpgraded;
		}
		if (c.Type == CardType.Tactic_Card) {
			flags = flags | UnitSelection.Active;
			flags = flags | UnitSelection.NotTempUpgraded;
		}

		_OverworldUI.ShowUnitSelectionUI(flags);

		int selectedUnits = 0;
		UIPlayerUnitTypeIndexCallback selectUnit = null;
		selectUnit = (PlayerType pt, UnitType u, int i) => {
			Unit unit = _CurrentPlayer.PlayerArmy.GetUnits(u)[i];
			selectedUnits++;
			// perform the action each time something is selected. This will only effect healing.
			// we don't want the player to be stuck with no units to select
			action(c, p, unit);
			// we reached the total?
			if (selectedUnits == numSelection) {
				// don't listen for namy more and hide the UI
				_OverworldUI._ArmyUI.OnClickUnit -= selectUnit;
				_OverworldUI.HideUnitSelectionUI();
				done(true, c, p, unit);
			}

		};
		_OverworldUI._ArmyUI.OnClickUnit += selectUnit;
	}


    public void Pause() {
        _OverworldUI.Disable();
    }

    public void UnPause()
    {
        _OverworldUI.Enable();
    }

    void _TurnManager_OnTurnStart() {
		_OverworldUI.Show();
        _OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(_CurrentPlayer.Type, _CurrentPlayer.CommanderPosition, 1));
    }

    void _TurnManager_OnTurnEnd() {
		_OverworldUI.Hide();
        _OverworldUI.DisablePlayerMovement();
    }

	void setPlayer(PlayerType p) {
		_CurrentPlayer = p == PlayerType.Battlebeard ? _BattlebeardPlayer : _StormshaperPlayer;
		_InactivePlayer = p == PlayerType.Battlebeard ? _StormshaperPlayer : _BattlebeardPlayer;
		_OverworldUI.SetPlayer(_CurrentPlayer);
	}


    void _TurnManager_OnSwitchTurn() {
		setPlayer (_CurrentPlayer.Type == PlayerType.Battlebeard ? PlayerType.Stormshaper : PlayerType.Battlebeard);
		_TurnManager.StartTurn ();
    }

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Return)) {
			StartCoroutine(SwitchPlayer());
		}

		if (Input.GetKeyDown (KeyCode.M)) {
			if (_CurrentPlayer.PreviousCommanderPosition && _CurrentPlayer.PreviousCommanderPosition != _CurrentPlayer.CommanderPosition) {
				_OverworldUI.ForceMoveCommander(_CurrentPlayer.PreviousCommanderPosition);
			}
		}

        if (Debug.isDebugBuild)
        {
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                _Audio.PlayLooped(SoundsEnum.Catoonz);
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                _Audio.PlayOnce(SoundsEnum.Catoonz);
            }

            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                DebugUI.getUI().SetMessage("Test", 22, Color.green);
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                _CurrentPlayer.Currency.addPoints(10);
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                // move back using up a turn
                if (_CurrentPlayer.PreviousCommanderPosition && _CurrentPlayer.PreviousCommanderPosition != _CurrentPlayer.CommanderPosition)
                {
                    _OverworldUI.MoveCommander(_CurrentPlayer.PreviousCommanderPosition);
                }
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                CardData c = GenerateRandomCard(_AvailableCaveCards.cards);
                _CurrentPlayer.AddCard(c);
            }
            //Delete in final build. Used for testing, an example of how to call debug message class
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                DebugUI.getUI().SetMessage("Test", 22, Color.green);
            }
        }
	}
}
