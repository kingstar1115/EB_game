using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Events;
using System;

public class OverworldManager : MonoBehaviour
{
	public OverworldUI _OverworldUI;
	public CardList _AvailableCaveCards;
	public CardSystem _CardSystem;
	public Board _Board;
	public Player _BattlebeardPlayer;
	public Player _StormshaperPlayer;
	public TurnManager _TurnManager;
	public GameStateHolder _GameStateHolder;
	public BattleData _BattleData;
	public string _BattleScene;
	public MusicTrack[] _OverworldMusic;

	public delegate void EndUnitSelectionAction(bool success, Unit u);


	//****TESTS ONLY****
	public CardList _StartCards;
	public CardList _StartCardsEditor;

	// Use this for initialization
	void Start() {
		//new game setup
		Fader.ScreenFader.StartFadeOverTime(Fader.FadeDir.FadeOut, SceneSnapshot.ScreenSnapshot.SnapScreenShot);
		bool newGame = _BattleData._EndState == BattleEndState.None;
		_Board.Initialise(!newGame);

		if (_BattleData._EndState == BattleEndState.None) {
			_BattlebeardPlayer.ResetArmy();
			_StormshaperPlayer.ResetArmy();
		}

		_BattlebeardPlayer.Initialise();
		_StormshaperPlayer.Initialise();

		if (_BattleData._EndState == BattleEndState.None) {
			//try get the battlebeard start tile
			if (_Board._BBStartTile != null) {
				_BattlebeardPlayer.CommanderPosition = _Board._BBStartTile;
			}
			else {
				Debug.LogError("Battlebeard start tile not set");
			}

			if (_Board._SSStartTile != null) {
				_StormshaperPlayer.CommanderPosition = _Board._SSStartTile;
			}
			else {
				Debug.LogError("Stormshaper start tile not set");
			}
		}

		//test by adding a scout card.

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
		_OverworldUI._ArmouryUI.OnPurchasedItem += _OverworldUI_ArmouryUI_OnPurchasedItem;
		_BattlebeardPlayer.Currency.OnChange += _OverworldUI._ResourceUI.UpdateResources;
		_BattlebeardPlayer.OnCardAdded += _BattlebeardPlayer_OnCardAdded;
		_BattlebeardPlayer.OnCardRemoved += _BattlebeardPlayer_OnCardRemoved;
		_BattlebeardPlayer.Currency.OnChange += _OverworldUI._ArmouryUI_OnCurrencyChanged;
		_StormshaperPlayer.Currency.OnChange += _OverworldUI._ResourceUI.UpdateResources;
		_StormshaperPlayer.OnCardAdded += _StormshaperPlayer_OnCardAdded;
		_StormshaperPlayer.OnCardRemoved += _StormshaperPlayer_OnCardRemoved;
		_StormshaperPlayer.Currency.OnChange += _OverworldUI._ArmouryUI_OnCurrencyChanged;

		_CardSystem.RequestUnitSelection +=_CardSystem_RequestUnitSelection;
		_CardSystem.RequestBattle += _CardSystem_RequestBattle;

		_BattlebeardPlayer.OnCastleProgress += _Board.SetCastleState;
		_StormshaperPlayer.OnCastleProgress += _Board.SetCastleState;
		

		_CardSystem.OnEffectApplied += _CardSystem_OnEffectApplied;

		_GameStateHolder._gameState = GameState.Overworld;

		Audio.AudioInstance.PlayMusic(_OverworldMusic, true);

		if (newGame) {

			//add apropriate start cards
			if (!Debug.isDebugBuild)
			{
				_BattlebeardPlayer.SetCards(_StartCards);
				_StormshaperPlayer.SetCards(_StartCards);
			}
			else
			{
				_BattlebeardPlayer.SetCards(_StartCardsEditor);
				_StormshaperPlayer.SetCards(_StartCardsEditor);
			}

			// add some start units
			_BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.Scout);
			_BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.Scout);
			_BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.Warrior);
			_BattlebeardPlayer.PlayerArmy.AddUnit(UnitType.Warrior);

			_StormshaperPlayer.PlayerArmy.AddUnit(UnitType.Scout);
			_StormshaperPlayer.PlayerArmy.AddUnit(UnitType.Scout);
			_StormshaperPlayer.PlayerArmy.AddUnit(UnitType.Cavalry);
			_StormshaperPlayer.PlayerArmy.AddUnit(UnitType.Cavalry);

			setPlayer(PlayerType.Battlebeard);
			

			_BattlebeardPlayer.CastleProgress = 0;
			_StormshaperPlayer.CastleProgress = 0;

			_TurnManager.StartTurn();
		}
		else {
			setPlayer(_BattleData._InitialPlayer.Type);

			_OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(_GameStateHolder._ActivePlayer, _GameStateHolder._ActivePlayer.CommanderPosition, 1));

			_BattlebeardPlayer.CastleProgress = _BattlebeardPlayer.CastleProgress;
			_StormshaperPlayer.CastleProgress = _StormshaperPlayer.CastleProgress;
			// run the battle end logic
			endBattle();
		}
	}

	void _OverworldUI_OnPlayerUseCard(CardData card)
	{
		UseCard(card);
	}

	void _BattlebeardPlayer_OnCardAdded(CardData card)
	{
		//inform ui
		onCardAdded(getPlayer(PlayerType.Battlebeard), card);
		_OverworldUI.AddPlayerCard (PlayerType.Battlebeard, card);
	}

	void _BattlebeardPlayer_OnCardRemoved(CardData card, int index)
	{
		//inform ui
		_OverworldUI.RemovePlayerCard (PlayerType.Battlebeard, card, index);
	}

	void _StormshaperPlayer_OnCardAdded(CardData card)
	{
		//inform ui

		onCardAdded(getPlayer(PlayerType.Stormshaper), card);
		_OverworldUI.AddPlayerCard (PlayerType.Stormshaper, card);
	}

	void _StormshaperPlayer_OnCardRemoved(CardData card, int index)
	{
		//inform ui
		_OverworldUI.RemovePlayerCard (PlayerType.Stormshaper, card, index);
	}

	void onCardAdded(Player p, CardData c) {
		switch (c.Type) {
			case CardType.Healing_Card:
				if (!p.HasGotCardHealing) {
					p.HasGotCardHealing = true;
					TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
						"Cards",
						"Healing cards can be used to heal your units at any time.\n" +
						"The number of units the can be healed is written on the card.",
					   false);
				}
				break;
			case CardType.Scout_Card:
				if(!p.HasGotCardScout) {
					p.HasGotCardScout = true;
					TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
						"Cards",
						"Scout cards allows you to make use of the scouts in your army.\n" +
						"Using this card allows you can move further in a turn without alerting anybody.\n" +
						"Having more scouts allows you to move further.",
					   false);
				}
				break;
			case CardType.Resource_Card:
				if(!p.HasGotCardResource) {
					p.HasGotCardResource = true;
					TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
						"Cards",
						"Resource cards give you resources so you can purchase things in the Armoury located in the centre of Nekark." + 
						"Your resources are shown in the top left of the screen.",
					   false);
				}
				break;
			case CardType.Battle_Card:
				if(!p.HasGotCardBattle) {
					p.HasGotCardBattle = true;
					TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
						"Cards",
						"Battle cards... Aargh! An enemy appeared out of nowhere!",
					   false);
				}
				break;
			case CardType.Tactic_Card:
				if(!p.HasGotCardTactic) {
					p.HasGotCardTactic = true;
					TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
						"Cards",
						"Tactic cards temporarily increase the strength of a unit in battle.\n" +
						"It only lasts for one attack, so use wisely.",
					   false);
				}
				break;
			case CardType.Alliance_Card:
				if(!p.HasGotCardAlliance) {
					p.HasGotCardAlliance = true;
					TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
						"Cards",
						"Alliance cards call forth a new unit to fight for you.",
					   false);
				}
				break;
			case CardType.Priority_Card:
				if(!p.HasGotCardPriority) {
					p.HasGotCardPriority = true;
					TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
						"Cards",
						"Priority cards will give you a speed advantage at the beginning of upcoming battles.",
					   false);
				}
				break;
			case CardType.Upgrade_Card:
				if(!p.HasGotCardPriority) {
					p.HasGotCardPriority = true;
					TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
						"Cards",
						"Resource cards increase the strength of a unit.\n" +
						"Be careful though, as it will wear off if the unit is knocked out!",
					   false);
				}
				break;
			default:
				break;

		}
	}

	void _CardSystem_OnEffectApplied(bool success, CardData card, Player player, Unit u) {
		ModalPanel p = ModalPanel.Instance();
		if (!success) {
			// we wont bother with this in the final but for now it hides the previous modal after it shows this one.
			p.ShowOK("Oh No!", "Card could not be used", null);
			return;
		}
		if (card.Type == CardType.Scout_Card) {
			int distance = Mathf.Clamp(player.PlayerArmy.GetActiveUnits(UnitType.Scout).Count + 1, 2, 4);
			player.IsScouting = true;
			_OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(player, player.CommanderPosition, distance));
		}
		if (_OverworldUI._HandUI.m_SelectedCardUI != null) {
			_GameStateHolder._ActivePlayer.RemoveCard(_OverworldUI._HandUI.m_SelectedCardUI._Index);
		}
		else {
			_GameStateHolder._ActivePlayer.RemoveCard(card);
		}
		
	}

	void UseCard(CardData card) {
		_CardSystem.UseCard(card, _GameStateHolder._ActivePlayer, _GameStateHolder._InactivePlayer, _GameStateHolder._gameState);
	}

	//This method is for when the user finishes using the armoury by Key or Button
	public void _ArmouryUI_EndTurn() {
		_OverworldUI._ArmouryUI.Hide();
		endTurn();
	}

	//Handles all armoury purchases
	public void _OverworldUI_ArmouryUI_OnPurchasedItem(PurchasableItem purchasedItem) {
		//We don't know item purchased at compile time, so use as keyword instead of casting
		PurchasableUnit purchasedUnit = purchasedItem as PurchasableUnit;
		PurchasableCard purchasedCard = purchasedItem as PurchasableCard;
		PurchasableCastlePiece purchasedCastlePiece = purchasedItem as PurchasableCastlePiece;

		//Give player the item and take money away
		if(purchasedUnit != null) {
			_GameStateHolder._ActivePlayer.PlayerArmy.AddUnit(purchasedUnit.UNITTYPE);
			_GameStateHolder._ActivePlayer.Currency.addPoints(-purchasedUnit.cost);
		}
		else if(purchasedCard != null) {
			_GameStateHolder._ActivePlayer.AddCard(purchasedCard.Card);
			_GameStateHolder._ActivePlayer.Currency.addPoints(-purchasedCard.cost);
		}
		else if(purchasedCastlePiece != null) {
			_GameStateHolder._ActivePlayer.CastleProgress++;
			_GameStateHolder._ActivePlayer.Currency.addPoints(-purchasedCastlePiece.cost);
		}
	}

	void _OverworldUI_OnCommanderForceMove(TileData tile) {
		_GameStateHolder._ActivePlayer.CommanderPosition = tile;
		//****JUST FOR TESTING**** set new reachable tiles
		_OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(_GameStateHolder._ActivePlayer, _GameStateHolder._ActivePlayer.CommanderPosition, 1));
		if (_BattleData._EndState == BattleEndState.Loss) {
			endTurn();
		}
	}

	void _OverworldUI_OnCommanderMove(TileData tile) {
		//set new position for the player (should depend on whose players turn it is)
		_GameStateHolder._ActivePlayer.CommanderPosition = tile;

		//****JUST FOR TESTING**** set new reachable tiles
		_OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(_GameStateHolder._ActivePlayer, _GameStateHolder._ActivePlayer.CommanderPosition, 1));

		//Handles events that happen when player lands on that tile
		HandleTileEvent(tile);
	}

	void HandleTileEvent(TileData tile) {
		_OverworldUI.Disable();
        _GameStateHolder._ActivePlayer.HasMoved = true;
		if (_GameStateHolder._ActivePlayer.IsScouting) {
			_GameStateHolder._ActivePlayer.IsScouting = false;
			endTurn ();
		} else {
			ModalPanel p = ModalPanel.Instance ();
			switch (tile.Building) {
			case BuildingType.Armoury:
				if (!_GameStateHolder._ActivePlayer.LandedArmoury) {
					_GameStateHolder._ActivePlayer.LandedArmoury = true;
					TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
					   "Armoury",
					   "Welcome to the Armoury!\n" +
						"You can use resources that you have earned to expand your army or purchase upgrades.",
					   false);
				}
				_OverworldUI._ArmouryUI.Show(_GameStateHolder._ActivePlayer);
				break;
			case BuildingType.Camp:
				if (!_GameStateHolder._ActivePlayer.LandedCamp) {
					_GameStateHolder._ActivePlayer.LandedCamp = true;
					TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
						"Camp",
						"You just landed on a camp. You can add it to your empire if you defeat the enemies lurking there.",
					   false);
				}
				if (tile.Owner != _GameStateHolder._ActivePlayer.Type) {
					if (tile.Owner == PlayerType.None) {
						// MONSTER BATTLE
						p.ShowOK ("Camp", "You see a camp and enter it. Monsters attack!", () => {
							startBattle (BattleType.Monster);
						});
					} else {
						
						if (tile.IsDefended()) {
							p.ShowOK("Camp", "You see a camp, there appears to be an enemy unit defending. The enemy attacks you!", () => {
								startBattle(BattleType.PvP);
							});
						} else {
							_Board.SetTileOwner(tile, _GameStateHolder._ActivePlayer.Type);
							Unit prisoner = _GameStateHolder._ActivePlayer.PlayerArmy.GetDefendingUnit(tile);
							if (prisoner != null) {
								_Board.UnsetTilePrisoner(tile);
								PromptForReturnPrisoner(_GameStateHolder._ActivePlayer, prisoner, tile, p, endTurn);
							} else if (_GameStateHolder._ActivePlayer.PlayerArmy.GetActiveUnits().Count > 1) {
								StartCoroutine(PromptForDefendingUnit("You enter the camp, it's been abandoned. Leave a unit to defend this camp?", _GameStateHolder._ActivePlayer, p, endTurn));
							} else {
								//TODO: Add some text here to give the player some info
								endTurn();
							}
						}
					}
					break;
				}
				if (tile.IsDefended ()) {
					Unit defendingUnit = _GameStateHolder._ActivePlayer.PlayerArmy.GetDefendingUnit (tile);
					p.ShowOKCancel("Take back unit", "Should this " + defendingUnit.Type + " stop defending this area?", () => 
						StartCoroutine(PromptToReplaceDefendingUnit (_GameStateHolder._ActivePlayer, defendingUnit, p, endTurn)),
						endTurn);
				} else {
					if (_GameStateHolder._ActivePlayer.PlayerArmy.GetActiveUnits().Count > 1) {
							StartCoroutine(PromptForDefendingUnit ("Your land is undefended. Would you like to leave a unit to defend this camp?", _GameStateHolder._ActivePlayer, p, endTurn));
					} else {
						endTurn ();
					}
				}
				break;
			case BuildingType.Cave:				
				if (tile.Owner != _GameStateHolder._ActivePlayer.Type) {
					if (tile.Owner != PlayerType.None && tile.IsDefended()) {
						p.ShowOK("Cave", "You enter the cave to explore. An enemy unit ambushes you!", () => {
							startBattle(BattleType.PvP);
						});
						return;
					}

					if(!_GameStateHolder._ActivePlayer.LandedCave) {
						_GameStateHolder._ActivePlayer.LandedCave = true;
						TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
							"Cave",
							"There's a decent stash of treasure in this cave.",
							true);
					}
					if(!_GameStateHolder._ActivePlayer.HasCaptured) {
						_GameStateHolder._ActivePlayer.HasCaptured = true;
						TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
							"Cave",
							"You captured the cave and made it part of your empire!",
							true);
					}
					CardData c =_CardSystem.GetRandomCard(_AvailableCaveCards.cards);
					_GameStateHolder._ActivePlayer.AddCard(c);
					_Board.SetTileOwner(tile, _GameStateHolder._ActivePlayer.Type);

					Unit prisoner = _GameStateHolder._ActivePlayer.PlayerArmy.GetDefendingUnit(tile);
					if (prisoner != null) {
						_Board.UnsetTilePrisoner(tile);
						PromptForReturnPrisoner(_GameStateHolder._ActivePlayer, prisoner, tile, p, endTurn);
					} else if (_GameStateHolder._ActivePlayer.PlayerArmy.GetActiveUnits().Count > 1) {
						StartCoroutine(PromptForDefendingUnit("You enter the cave, there's no sign of the enemy. Leave a unit to defend this cave?", _GameStateHolder._ActivePlayer, p, endTurn));
					} else {
						endTurn();
					}
				} else {
					if (tile.IsDefended ()) {
						Unit defendingUnit = _GameStateHolder._ActivePlayer.PlayerArmy.GetDefendingUnit (tile);
						p.ShowOKCancel("Take back unit", "Should this " + defendingUnit.Type + " stop defending this cave?", () => 
							StartCoroutine(PromptToReplaceDefendingUnit (_GameStateHolder._ActivePlayer, defendingUnit, p, endTurn)),
							endTurn);
					} else {
						if (_GameStateHolder._ActivePlayer.PlayerArmy.GetActiveUnits().Count > 1) {
								StartCoroutine(PromptForDefendingUnit ("Your land is undefended. Would you like to leave a unit to defend this cave?", _GameStateHolder._ActivePlayer, p, endTurn));
						} else {
							endTurn ();
						}
					}
				}
				break;
			case BuildingType.Fortress:
				bool ownsSurroundingTiles = tile.GetConnectedTiles().FindAll(t => t.Owner == _GameStateHolder._ActivePlayer.Type).Count >= 3;
				if (!_GameStateHolder._ActivePlayer.LandedFortress) {
					_GameStateHolder._ActivePlayer.LandedFortress = true;
					TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
						"Fortress",
						"This fortress seems eerie. It looks like there are many like it around Nekark.",
						true);
					if (!ownsSurroundingTiles) {
						TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
							"Fortress",
							"I suggest securing the surrounding area!",
							false);
					}
				}
				// Make sure that the fortress type matches the player type
				if (tile.Owner == _GameStateHolder._ActivePlayer.Type) {
					// make sure the player owns at least 3 surrounding tiles
					if (ownsSurroundingTiles) {
						// Battle lost immortal
						p.ShowOK("Fortress", "A bloody Lost Immortal just showed up innit blud!", () => {
							startBattle(BattleType.LostImmortal);
						});
					} else {
						p.ShowOK ("Fortress", "You felt a chill in the air, but nothing appeared.", endTurn);
					}
					break;
				}
				endTurn();
				break;
			case BuildingType.Inn:
				if(!_GameStateHolder._ActivePlayer.LandedInn) {
					_GameStateHolder._ActivePlayer.LandedInn = true;
					TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
						"Inn",
						"The Inn is a good place to heal your units.\n"+
						"Inns are a good place to rest and revive your units when they are knocked out.",
						true);
					TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
						"Inn",
						"Unfortunately, sometimes units cannot be healed and you will need to visit the inn another time.",
						false);
				}
				Audio.AudioInstance.PlaySFX(SoundEffect.Inn);
				if (_GameStateHolder._InactivePlayer.CastleProgress >= 4) {
						if(!_GameStateHolder._ActivePlayer.LandedInnUnavailable) {
							_GameStateHolder._ActivePlayer.LandedInnUnavailable = true;
							TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
								"Inn",
								"Oh no!" +
								"The inn has turned you away! The " + _GameStateHolder._InactivePlayer.Type.ToString() + "army is putting too much pressure on them!.\n" +
								"Looks like there's no rest for us anymore...",
								true);
						}
						p.ShowOK ("Oh No!", "The inn won't accept you!", endTurn);
					break;
				}

				var rnd = UnityEngine.Random.Range (0, 3);
				List<Unit> units;

				units = _GameStateHolder._ActivePlayer.PlayerArmy.GetRandomUnits (rnd, _GameStateHolder._ActivePlayer.PlayerArmy.GetKOUnits());

				foreach (var unit in units) {
					unit.Heal ();
				}

				string content, title;

				if (_GameStateHolder._ActivePlayer.PlayerArmy.GetKOUnits ().Count == 0) {
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

	void startBattle(BattleType type) {
		_BattleData._BattleType = type;
		_BattleData._InitialPlayer = _GameStateHolder._ActivePlayer;
		tearDownScene();
		StartCoroutine(SceneSwitcher.ChangeScene(_BattleScene));
	}



	void endBattle() {
		bool hasMessage = false;
		if (_BattleData._EndState == BattleEndState.Loss && !_GameStateHolder._ActivePlayer.HasLost) {
			_GameStateHolder._ActivePlayer.HasLost = true;
			TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
				"Battle",
				"We lost this fight, but there are many more battles to win!",
				true);
			hasMessage = true;
		}

		if(_BattleData._EndState == BattleEndState.Win && !_GameStateHolder._ActivePlayer.HasWon) {
			_GameStateHolder._ActivePlayer.HasWon = true;
			TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
				"Battle",
				"We won this fight and reaped the spoils, but there are many more battles to win!",
				true);
			hasMessage = true;
		}

		ModalPanel p = ModalPanel.Instance ();
		_OverworldUI.Disable ();
		TileData tile = _GameStateHolder._ActivePlayer.CommanderPosition;
		TileData otherPlayerTile = _GameStateHolder._InactivePlayer.CommanderPosition;

		// check for total KO
		if (_GameStateHolder._ActivePlayer.PlayerArmy.GetActiveUnits().Count == 0 && _GameStateHolder._InactivePlayer.CastleProgress != 4) {
			List<Unit> units = _GameStateHolder._ActivePlayer.PlayerArmy.GetKOUnits();
			// revive half the units;
			ReviveUnits(_GameStateHolder._ActivePlayer, units, (units.Count / 2)+1);
			//Reset position
			ResetCommanderPosition();
			return;
		}
		else if (isGameOverForPlayer(_GameStateHolder._ActivePlayer)) {
			EndGame(_GameStateHolder._InactivePlayer);
			return;
		}
		else if (isGameOverForPlayer(_GameStateHolder._InactivePlayer)){
			EndGame(_GameStateHolder._ActivePlayer);
			return;
		}

		if (_BattleData._BattleType == BattleType.Monster) {
			if (_BattleData._EndState == BattleEndState.Win) {
				if (tile.Building == BuildingType.Camp) {
					if(!_GameStateHolder._ActivePlayer.HasCaptured) {
						_GameStateHolder._ActivePlayer.HasCaptured = true;
						TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
							"Camp",
							"You captured the camp and made it part of your empire!",
							!hasMessage);
					}
					_Board.SetTileOwner(tile, _GameStateHolder._ActivePlayer.Type);
				}
				if (_GameStateHolder._ActivePlayer.PlayerArmy.GetActiveUnits().Count > 1) {
					StartCoroutine(PromptForDefendingUnit ("This tile now belongs to your kingdom. Leave a unit to defend your honour?", _GameStateHolder._ActivePlayer, p, endTurn));
				} else {
					endTurn();
				}
			} else {
				Debug.Log("lose monster");
				_OverworldUI.Enable();
				_OverworldUI.ForceMoveCommander(_GameStateHolder._ActivePlayer.PreviousCommanderPosition);
				//p.ShowOK("Battle", "You lost against the monster!", );
			}
		}
		else if (_BattleData._BattleType == BattleType.PvP) {
			if (_BattleData._EndState == BattleEndState.Win) {
				_Board.SetTileOwner(tile, _GameStateHolder._ActivePlayer.Type);

				// the unit defending is now a prisoner
				tile.Prisoner = true;
				Unit defender = _GameStateHolder._InactivePlayer.PlayerArmy.GetDefendingUnit (tile);
				defender.SetPrisoner (true);
				defender.SetDefending (false); //tbh should just do this in SetPrisoner()
				tile.SetDefender(null);
				_Board.UnsetTileDefence(tile);
				_Board.SetTilePrisoner(tile);

				Unit prisoner = _GameStateHolder._ActivePlayer.PlayerArmy.GetDefendingUnit(tile);
				
				if (tile.Building == BuildingType.Cave) {
					if(!_GameStateHolder._ActivePlayer.HasCaptured) {
						_GameStateHolder._ActivePlayer.HasCaptured = true;
						TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type,
							"Cave",
							"You captured the camp and made it part of your empire!",
							!hasMessage);
					}
					CardData c = _CardSystem.GetRandomCard(_AvailableCaveCards.cards);
					_GameStateHolder._ActivePlayer.AddCard(c);

					// if the active player had a prisoner on the tile 
					if (prisoner != null) {
						PromptForReturnPrisoner(_GameStateHolder._ActivePlayer, prisoner, tile, p, endTurn);
					} else if (_GameStateHolder._ActivePlayer.PlayerArmy.GetActiveUnits().Count > 1) {
						StartCoroutine(PromptForDefendingUnit ("This tile now belongs to your kingdom. Leave a unit to defend your honour?", _GameStateHolder._ActivePlayer, p, endTurn));
					} else {
						endTurn ();
					}

				}
				else {
					// if the active player had a prisoner on the tile 
					if (prisoner != null) {
						PromptForReturnPrisoner(_GameStateHolder._ActivePlayer, prisoner, tile, p, endTurn);
						return;
					} else if (_GameStateHolder._ActivePlayer.PlayerArmy.GetActiveUnits().Count > 1) {
						StartCoroutine(PromptForDefendingUnit ("This tile now belongs to your kingdom. Leave a unit to defend your honour?", _GameStateHolder._ActivePlayer, p, endTurn));
					} else {
						endTurn();
					}
				}
			} else {
				// the active player lost

				_OverworldUI.Enable();
				_OverworldUI.ForceMoveCommander(_GameStateHolder._ActivePlayer.PreviousCommanderPosition);
			}
		}
		else if (_BattleData._BattleType == BattleType.LostImmortal) {
			if (_BattleData._EndState == BattleEndState.Win && tile.Building == BuildingType.Fortress) {
				tile.Owner = PlayerType.None;
				_GameStateHolder._ActivePlayer.LostImmortalKillCount++;
				endTurn();
			}
			else {
				_OverworldUI.Enable();
				_OverworldUI.ForceMoveCommander(_GameStateHolder._ActivePlayer.PreviousCommanderPosition);
			}
		}
		else if (_BattleData._BattleType == BattleType.Card) {
			if (_BattleData._EndState == BattleEndState.Win) {
				endTurn();
			}
			else {
				_OverworldUI.Enable();
				_OverworldUI.ForceMoveCommander(_GameStateHolder._ActivePlayer.PreviousCommanderPosition);
			}
		}
	}

	//Won't need this function when we get rid of ModalPanel
	void Reset()
	{
		tearDownScene();
		StartCoroutine(SceneSwitcher.ChangeScene(Application.loadedLevelName));
	}

	void ReviveUnits(Player p, List<Unit> units, int num){
		units = p.PlayerArmy.GetRandomUnits(num, units);
		for (int i = 0; i < units.Count; i++){
			units[i].Heal();
		}
	}

	void ResetCommanderPosition()
	{
		// move commander to start tile;
		_OverworldUI.Enable();
		TileData startTile = _GameStateHolder._ActivePlayer.Type == PlayerType.Battlebeard ? _Board._BBStartTile : _Board._SSStartTile;
		_OverworldUI.ForceMoveCommander(startTile);
	}

	bool isGameOverForPlayer(Player p) {
		return p.PlayerArmy.GetActiveUnits().Count == 0 && getOtherPlayer(p.Type).CastleProgress == 4;
	}

	void EndGame(Player player)
	{
		ModalPanel p = ModalPanel.Instance();
		_OverworldUI.Disable();
		_BattleData._EndState = BattleEndState.None;
		StartCoroutine(SceneSwitcher.ChangeScene ("StartMenu"));
	}
	
	void _CardSystem_RequestBattle(CardData card, EndCardAction done)
	{
		BuildingType currentTile = _GameStateHolder._ActivePlayer.CommanderPosition.Building;
		BuildingType [] battleTiles = {
			BuildingType.Camp,
			BuildingType.Cave,
			BuildingType.Fortress
		};
		if (Array.IndexOf(battleTiles, currentTile) == -1) {
			done(false, card, _GameStateHolder._ActivePlayer, null);
			return;
		}

		done(true, card, _GameStateHolder._ActivePlayer, null);
		startBattle (BattleType.Card);

		if (Debug.isDebugBuild)		
			Debug.Log ("Battle card has started battle");
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
			Unit unit = _GameStateHolder._ActivePlayer.PlayerArmy.GetUnits(u)[i];
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

	IEnumerator PromptForDefendingUnit(string promptString, Player p, ModalPanel mp, UnityAction onDone) {
		yield return new WaitForSeconds(0.5f);
		mp.ShowOKCancel ("Tile Defence", promptString, () =>
			DefendingUnitSelection(_GameStateHolder._ActivePlayer, mp, onDone), 
			endTurn);
	}

	IEnumerator PromptToReplaceDefendingUnit(Player p, Unit u, ModalPanel mp, UnityAction onDone) {
		yield return new WaitForSeconds (0.5f);
		mp.ShowOKCancel("Replace defending unit", "This tile is currently defended by " + u.Type.ToString() + ". Would you like to send another unit in it's place?",
			() => RemoveDefendingUnit(p, u, () => DefendingUnitSelection(p, mp, onDone)),
			() => RemoveDefendingUnit(p, u, onDone));
	}

	void PromptForReturnPrisoner(Player p, Unit u, TileData t, ModalPanel mp, UnityAction onDone) {
		mp.ShowOKCancel("Unit reclaimed", "Should the unit go back to defending this tile?",
			() => {
				t.SetDefender(u);
				u.SetPosition(t);
				u.SetDefending(true);
				_Board.SetTileDefence(t);
				onDone();
			},
			() => StartCoroutine(PromptForDefendingUnit("Should another unit defend instead?", p, mp, onDone)));
	}

	void RemoveDefendingUnit(Player p, Unit u, UnityAction onDone) {
		TileData tile = p.CommanderPosition;
		TeardownDefendedTile (u, p, tile, onDone);
	}

	void DefendingUnitSelection(Player p, ModalPanel mp, UnityAction onDone) {
		//Only allow active units

		UnitSelection flags = UnitSelection.Active;

		flags = flags | UnitSelection.NotDefending;

		_OverworldUI.ShowUnitSelectionUI(flags);

		UIPlayerUnitTypeIndexCallback selectUnit = null;
		selectUnit = (PlayerType PlayerType, UnitType ut, int i) => {
			Unit unit = p.PlayerArmy.GetUnits(ut)[i];
			TileData tile = p.CommanderPosition;
			mp.ShowOKCancel("Confirm unit selection", "Leave behind " + unit.Type.ToString() + "?", 
				() => SetupDefendedTile(unit, p, tile, onDone, selectUnit),
				() => DefendingUnitSelection(p, mp, onDone));
		};
		_OverworldUI._ArmyUI.OnClickUnit += selectUnit;
	}

	void SetupDefendedTile(Unit u, Player p, TileData t, UnityAction onDone, UIPlayerUnitTypeIndexCallback remove) {
		t.SetDefender(u);
		u.SetPosition(t);
		u.SetDefending (true);
		_Board.SetTileDefence (t);
		_OverworldUI._ArmyUI.OnClickUnit -= remove;
		_OverworldUI.HideUnitSelectionUI();

		onDone();
	}

	void TeardownDefendedTile(Unit u, Player p, TileData t, UnityAction onDone) {
		t.SetDefender(null);
		u.SetPosition(null);
		u.SetDefending (false);
		_Board.UnsetTileDefence (t);

		onDone();
	}

	public void Pause() {
		_OverworldUI.Disable();
	}

	public void UnPause()
	{
		_OverworldUI.Enable();
	}

	void _OverworldUI_OnUnPause() {
		_OverworldUI._Paused = false;
	}

	void _OverworldUI_OnPause() {
		_OverworldUI._Paused = true;
	}

	// --- turns

	void _TurnManager_OnTurnStart() {
		_OverworldUI.Show();
		Player p = _GameStateHolder._ActivePlayer;
		_OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(p, p.CommanderPosition, 1));
		if (!p.HasMoved) {
			TutorialPanel.Instance().Tutor(p.Type,
				"Tutorial", 
				"Hello fellow " + p.Type.ToString() + ", and welcome to 'Empire, the Last Battle'!",
				true);
            TutorialPanel.Instance().Tutor(p.Type,
                "Tutorial",
                "I will be telling what you need to know in order to defeat the enemies that get in your way and retake the land of Nekark!",
                false);
			TutorialPanel.Instance().Tutor(p.Type, 
				"Tutorial", 
				"On left side of the screen, you can see some icons. These represent the units in your army.\n"+
                "There is an individual icon representing each type of unit.",
                false);
            TutorialPanel.Instance().Tutor(p.Type,
                "Tutorial",
                "The red bars to the right of them represent individual unit's health.\n" +
                 "Hovering your mouse over each icon will show the status of individual units of that type.",
                false);
            TutorialPanel.Instance().Tutor(p.Type,
                "Tutorial",
                "Throughout your skirmish you will come across items known as cards.\n" +
                "The cards you have collected are shown in the bottom right.\n" +
                "Click on a card, and press use to use a specific card.",
                false);
            TutorialPanel.Instance().Tutor(p.Type,
                "Tutorial",
                "You can admire the view by holding down the right mouse button and moving the mouse.",
                false);
            TutorialPanel.Instance().Tutor(p.Type,
                "Tutorial",
                "You can also move around the land by holding and dragging me to an adjacent tile on the board.",
                false);
            TutorialPanel.Instance().Tutor(p.Type,
                "Tutorial",
                "You will notice that only the accessible tiles get highlighted while you are moving me.",
                false);
            TutorialPanel.Instance().Tutor(p.Type,
                "Tutorial",
                "Now go forth!",
                false);
		}
	}

	void endTurn() {
		// here we use the cards that we got.
		// use all the resource cards
		CardData c = _CardSystem.GetRandomCard(_GameStateHolder._ActivePlayer.Hand.cards, CardType.Resource_Card);
		while(c != null) {
			_CardSystem.UseCard(c, _GameStateHolder._ActivePlayer, _GameStateHolder._InactivePlayer, _GameStateHolder._gameState);
			c = _CardSystem.GetRandomCard(_GameStateHolder._ActivePlayer.Hand.cards, CardType.Resource_Card);
		}

		// use battle card
		c = _CardSystem.GetRandomCard(_GameStateHolder._ActivePlayer.Hand.cards, CardType.Battle_Card);
		if (c != null) {
			_CardSystem.UseCard(c, _GameStateHolder._ActivePlayer, _GameStateHolder._InactivePlayer, _GameStateHolder._gameState);
		} else {
			_TurnManager.EndTurn();
		}
	}

	void _TurnManager_OnTurnEnd() {
		_OverworldUI.Hide();
		TutorialPanel.Instance().Hide();
		_OverworldUI.DisablePlayerMovement();
		_BattleData._EndState = BattleEndState.None;
		_OverworldUI.Disable();
		StartCoroutine(SwitchPlayer());
		_OverworldUI.Enable();
	}

	//This is temporary until we actually have things that happen after the move
	IEnumerator SwitchPlayer() {
		yield return new WaitForSeconds(1);
		_TurnManager.SwitchTurn();
	}

	void _TurnManager_OnSwitchTurn() {
		setPlayer(getOtherPlayer(_GameStateHolder._ActivePlayer.Type).Type);
		_TurnManager.StartTurn();
	}

	void setPlayer(PlayerType p) {
		_GameStateHolder._ActivePlayer = getPlayer(p);
		_GameStateHolder._InactivePlayer = getOtherPlayer(p);
		_OverworldUI.SetPlayer(_GameStateHolder._ActivePlayer);    
	}  

	Player getOtherPlayer(PlayerType p) {
		return p == PlayerType.Battlebeard ? _StormshaperPlayer : _BattlebeardPlayer;  
	}
  
	Player getPlayer(PlayerType p) {
		return p == PlayerType.Battlebeard ? _BattlebeardPlayer : _StormshaperPlayer;   
	}

	// Update is called once per frame
	void Update() {
        if (Debug.isDebugBuild)
        {
			if(Input.GetKeyDown(KeyCode.M)) {
				if(_GameStateHolder._ActivePlayer.PreviousCommanderPosition && _GameStateHolder._ActivePlayer.PreviousCommanderPosition != _GameStateHolder._ActivePlayer.CommanderPosition) {
					_OverworldUI.ForceMoveCommander(_GameStateHolder._ActivePlayer.PreviousCommanderPosition);
				}
			}

			if(Input.GetKeyDown(KeyCode.T)) {
				TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type, "title", "you should do this thing!", true);
				TutorialPanel.Instance().Tutor(_GameStateHolder._ActivePlayer.Type, "title 2", "page 2!", false);
			}

            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                DebugUI.getUI().SetMessage("Test", 22, Color.green);
            }

            if (Input.GetKey(KeyCode.Alpha9))
            {
                _GameStateHolder._ActivePlayer.Currency.addPoints(10);
            }

			if(Input.GetKey(KeyCode.Alpha0)) {
				_GameStateHolder._ActivePlayer.Currency.addPoints(-10);
			}

			if (Input.GetKeyDown(KeyCode.N))
            {
                // move back using up a turn
                if (_GameStateHolder._ActivePlayer.PreviousCommanderPosition && _GameStateHolder._ActivePlayer.PreviousCommanderPosition != _GameStateHolder._ActivePlayer.CommanderPosition)
                {
                    _OverworldUI.MoveCommander(_GameStateHolder._ActivePlayer.PreviousCommanderPosition);
                }
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                CardData c = _CardSystem.GetRandomCard();
				_GameStateHolder._ActivePlayer.AddCard(c);
            }

			if (Input.GetKeyDown(KeyCode.P))
			{
				//Test Pvp battle and loss
				_GameStateHolder._ActivePlayer.CastleProgress = 4;
				_GameStateHolder._InactivePlayer.CastleProgress = 4;
				_StormshaperPlayer.CommanderPosition = _Board.GetTileAt(0, 1);
			}

			if (Input.GetKeyDown(KeyCode.I)){
				foreach (var item in _GameStateHolder._ActivePlayer.PlayerArmy.GetActiveUnits()) {
					item.ReduceHP(int.MaxValue);
				}		
			}
		}
	}
	void tearDownScene() {
		_BattlebeardPlayer.RemoveListeners();
		_StormshaperPlayer.RemoveListeners();
		_OverworldUI.RemoveListeners();
		_CardSystem.RemoveListeners();
	}


}
