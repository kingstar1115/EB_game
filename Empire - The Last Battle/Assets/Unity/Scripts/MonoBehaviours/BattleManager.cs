using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class BattleManager : MonoBehaviour {

    public delegate void BattleAction(iBattleable battleAbleObject);
    public static event BattleAction OnBattleAbleUpdate = delegate { };
    public delegate void BattleIntAction(iBattleable battleAbleObject, int val);
    public static event BattleIntAction OnBattleAbleTakeDamage = delegate { };

	public GameStateHolder _GameStateHolder;
	public BattleData _BattleData;
	public CardSystem _CardSystem;
	public BattleUnitPositionManager _BattleUnitPositionManager;
	public Player _StormshaperPlayer;
	public Player _BattlebeardPlayer;
	public string _OverworldScene;
	public BattlerType activePlayer;
	public MonsterManager _MonsterManager;

	public CardList LostImmortalCards;

	List<Unit> _instigatorBattlers;
	iBattleable _oppositionBattler;
	iBattleable _oppositionReserveA;
	iBattleable _oppositionReserveB;
	iBattleable _oppositionReserveC;

	// Use this for initialization
	void Start () {
		Fader.ScreenFader.StartFadeOverTime(Fader.FadeDir.FadeOut, SceneSnapshot.ScreenSnapshot.SnapScreenShot);
		// set up the players
		_StormshaperPlayer = _GameStateHolder._ActivePlayer.Type == PlayerType.Stormshaper ? _GameStateHolder._ActivePlayer : _GameStateHolder._InactivePlayer;
		_BattlebeardPlayer = _GameStateHolder._ActivePlayer.Type == PlayerType.Battlebeard ? _GameStateHolder._ActivePlayer : _GameStateHolder._InactivePlayer;
		// set up the units and stuff

		activePlayer = BattlerType.None;

		// init the ui
		_BattleUnitPositionManager.Initialise(_BattlebeardPlayer, _StormshaperPlayer);

		Debug.Log(_BattleData._BattleType);

		_instigatorBattlers = new List<Unit>();
		_GameStateHolder._ActivePlayer.OnUpdateUnit += OnUpdateUnit;
		_GameStateHolder._ActivePlayer.OnAddUnit += OnAddUnit;
		// add player event listeners
		_GameStateHolder._ActivePlayer.Initialise();

        //ui event listeners
        _BattleUnitPositionManager.OnPause += _BattleUnitPositionManager_OnPause;
        _BattleUnitPositionManager.OnUnPause += _BattleUnitPositionManager_OnUnPause;
		_BattleUnitPositionManager.OnPlayerUseCard += UseCard;

		_BattlebeardPlayer.OnCardAdded += _BattlebeardPlayer_OnCardAdded;
		_BattlebeardPlayer.OnCardRemoved += _BattlebeardPlayer_OnCardRemoved;
		_StormshaperPlayer.OnCardAdded += _StormshaperPlayer_OnCardAdded;
		_StormshaperPlayer.OnCardRemoved += _StormshaperPlayer_OnCardRemoved;

		// init the card system
		_CardSystem.RequestUnitSelection += _CardSystem_RequestUnitSelection;
		_CardSystem.OnEffectApplied += _CardSystem_OnEffectApplied;


		if (_BattleData._BattleType == BattleType.LostImmortal) {
			_setupLostImmortalBattle();
			// start pre battle stuff
			StartCoroutine(_preBattle());
		}
		else if (_BattleData._BattleType == BattleType.Monster || _BattleData._BattleType == BattleType.Card) {
			_setupMonsterBattle();
			// start pre battle stuff
			StartCoroutine(_preBattle());
		}
		else if (_BattleData._BattleType == BattleType.PvP) {
			_setupPvPBattle();
			// pre battle starts inside the setup because we wait for unit selection (blargh)
		}		
	}

    void _BattleUnitPositionManager_OnUnPause()
    {
        _BattleUnitPositionManager._Paused = false;
    }

    void _BattleUnitPositionManager_OnPause()
    {
        _BattleUnitPositionManager._Paused = true;
    }

	void removeListeners() {
		_GameStateHolder._ActivePlayer.RemoveListeners();
		_GameStateHolder._InactivePlayer.RemoveListeners();
		OnBattleAbleUpdate = delegate {};
		OnBattleAbleTakeDamage = delegate {};
		_BattleUnitPositionManager.RemoveListeners();
	}

	private void OnAddUnit(Player p, Unit u) {
		_instigatorBattlers.Add(u);
		_BattleUnitPositionManager.InitUnitUI(u.Type, p.Type);
		
	}

	private void OnUpdateUnit(Player p, Unit u) {
		//if there is a KO
		if (u.IsKO()){
			//play the sound effect for KO unit
			Audio.AudioInstance.PlaySFX(SoundEffect.Dead);

			//if the battle is pvp
			if(_BattleData._BattleType == BattleType.PvP)
			{
				//remove the correct unit 
				if(_BattleData._InitialPlayer == p)
				{
					//remove active player
					_BattleUnitPositionManager.RemoveInstigatorPlayer();
					_instigatorBattlers.Remove(u);
				}
				else
				{
					_BattleUnitPositionManager.RemoveOpposition();
				}
			}
			else {
				if (!p.PlayerArmy.GetActiveUnitTypes().Contains(u.Type)) {
					_BattleUnitPositionManager.RemoveUnit(u.Type);
				}
				_instigatorBattlers.Remove(u);
			}
		}
		else if (!_instigatorBattlers.Contains(u)) {
			OnAddUnit(p, u);
		}

		OnBattleAbleUpdate (u);
	}

	void setActiveUnits() {
		_instigatorBattlers = _GameStateHolder._ActivePlayer.PlayerArmy.GetActiveUnits();
	}

	IEnumerator _preBattle() {
		yield return new WaitForSeconds(2);
		// set the state to pre battle
		_GameStateHolder._gameState = GameState.PreBattle;

		if (_BattleData._BattleType == BattleType.PvP) {
			calculateFirstTurnPvP();
		}
		else {
			calculateFirstTurnNonPvP();
		}

		_startBattle();
	}

	void calculateFirstTurnPvP() {
		// calculate who goes first
		CardData activeCard = _GameStateHolder._ActivePlayer.GetCardOfType(CardType.Priority_Card),
				 inactiveCard = _GameStateHolder._InactivePlayer.GetCardOfType(CardType.Priority_Card);

		if (activeCard != null ^ inactiveCard != null) {
			// one player has a priority card, which one?
			if (inactiveCard != null) {
				activePlayer = BattlerType.Opposition;
				_GameStateHolder._InactivePlayer.RemoveCard(inactiveCard);
				Debug.Log(_GameStateHolder._InactivePlayer.GetType() + " used priority card to go first");
			} else {
				activePlayer = BattlerType.Instigator;
				_GameStateHolder._ActivePlayer.RemoveCard(activeCard);
				Debug.Log(_GameStateHolder._ActivePlayer.GetType() + " used priority card to go first");
			}
		}
		else {
			calculateFirstTurnNonPvP();
		}
	}

	float getPercentage(float minVal, float maxVal) {
		float min = Mathf.Min(maxVal / 2, minVal);
		float max = Mathf.Max(maxVal + maxVal / 2);
		return ((minVal - min) / (max - min));
	}

	void calculateFirstTurnNonPvP() {
		// calculate speeds

		CardData activeCard = _GameStateHolder._ActivePlayer.GetCardOfType(CardType.Priority_Card);
		if (activeCard != null) {
			_GameStateHolder._ActivePlayer.RemoveCard(activeCard);
			activePlayer = BattlerType.Instigator;
		} else {
			float maxSpeedInstigator = _instigatorBattlers.Max(x => x.GetSpeed());
			float maxSpeedOpposition = _oppositionBattler.GetSpeed();

			// if now get the chance that the instigator will go first
			float chance = 0;
			if(maxSpeedInstigator >= maxSpeedOpposition) {
				chance = 1 - getPercentage(maxSpeedOpposition, maxSpeedInstigator);
			}
			else if(maxSpeedOpposition > maxSpeedInstigator) {
				chance = getPercentage(maxSpeedInstigator, maxSpeedOpposition);
			}
			if(new System.Random().NextDouble() < chance) {
				activePlayer = BattlerType.Instigator;
			}
			else {
				activePlayer = BattlerType.Opposition;
			}
		}
		Debug.Log(activePlayer + " goes first");
	}

	void switchPlayer() {
		activePlayer = (activePlayer == BattlerType.Instigator) ? BattlerType.Opposition : BattlerType.Instigator;
		startTurn();
	}

	void startTurn() {
		_BattleUnitPositionManager.ChangeCamera();
		if (_BattleData._BattleType == BattleType.PvP || activePlayer == BattlerType.Instigator) {
			startTurnPlayer();
		}
		else {
			startTurnMonster();
		}
	}

	void startTurnPlayer() {
		// show player UI
        _BattleUnitPositionManager.SetPlayer(GetActivePlayer());
	}

	void startTurnMonster() {
		// monster will attack!

		// AI goes here.

		// just attack the first one for now
		Debug.Log("Enemy attacks!");
		Attack(activePlayer, _instigatorBattlers[Random.Range(0, _instigatorBattlers.Count)]);
		Audio.AudioInstance.PlaySFX(SoundEffect.Roar1);
		StartCoroutine(endTurn());
	}

   IEnumerator endTurn() {
	   if (!(activePlayer == BattlerType.Opposition && _BattleData._BattleType != BattleType.PvP)) {
		   // do end turn stuff
		   _BattleUnitPositionManager._ArmyUI.Hide();
		   _BattleUnitPositionManager._HandUI.Hide();
		   _BattleUnitPositionManager._ButtonsUI.Hide();
	   }
		yield return new WaitForSeconds(2);
		
		if (_instigatorBattlers.Count() == 0) {
			_endBattle(BattleEndState.Loss);
		} else if (_oppositionBattler.IsKO())  {

			if(_BattleData._BattleType == BattleType.LostImmortal) {
				if(_oppositionReserveB != null) {
					_BattleUnitPositionManager.RemoveOpposition();
					_BattleUnitPositionManager.SetReserveBAsOpposition();
					_oppositionBattler = _oppositionReserveB;
					_oppositionReserveB = null;
					switchPlayer();
				}
				else if(_oppositionReserveC != null) {
					_BattleUnitPositionManager.RemoveOpposition();
					_BattleUnitPositionManager.SetReserveCAsOpposition();
					_oppositionBattler = _oppositionReserveC;
					_oppositionReserveC = null;
					switchPlayer();
				}
				else {
					_BattleUnitPositionManager.RemoveOpposition();
					_endBattle(BattleEndState.Win);
				}
			} else {
				_BattleUnitPositionManager.RemoveOpposition();
				_endBattle(BattleEndState.Win);
			}
			
		} else {
			switchPlayer();
		}
	}

	


	Player getOtherPlayer(PlayerType p) {
		return p == PlayerType.Battlebeard ? _StormshaperPlayer : _BattlebeardPlayer;
	}

	Player getPlayer(PlayerType p) {
		return p == PlayerType.Battlebeard ? _BattlebeardPlayer : _StormshaperPlayer;
	}


	void _startBattle() {
		// set state to battle
		_GameStateHolder._gameState = GameState.Battle;

		Audio.AudioInstance.PlaySFX(SoundEffect.Charge);

		startTurn();

	}

	void _setupMonsterBattle() {
		Debug.Log("Battle Monster");
		setActiveArmy();

		setOpposition(getMonster());
		Audio.AudioInstance.PlayMusic(MusicTrack.Dungeon);
	}

	void _setupLostImmortalBattle() {
		Debug.Log("Battle Lost Immortal");
		setActiveArmy();

		// generate a lost immortal based on the beaten lost immortals
		MonsterType[] monsters = getLostImmortalMonsters();
		setOpposition(monsters[0]);
		setOppositionB(monsters[1]);
		setOppositionC(getLostImmortal());

		// do some stuff to move reserve A into the active position (needs to be animated)
		//_BattleUnitPositionManager.SetReserveAAsOpposition();
		//_oppositionBattler = _oppositionReserveA;
		//_oppositionReserveA = null;

		Audio.AudioInstance.PlayMusic(MusicTrack.Dungeon);
	}

	MonsterType getLostImmortal() {
		int count = _GameStateHolder._ActivePlayer.LostImmortalKillCount;
		// ss immortals have an index of 11 - 14, bb is 15 - 18
		int startIndex = _GameStateHolder._ActivePlayer.Type == PlayerType.Stormshaper ? 11 : 15;
		return (MonsterType)startIndex + count;
	}

	MonsterType getMonster() {
		float rand = Random.value;
		MonsterType monster;
		switch (_GameStateHolder._ActivePlayer.CastleProgress) {
			case 0:
				// cyclops/minotaur/(hard)wyrm
				// 45/45/10
				if (rand <= .45) { monster = MonsterType.Cyclops; }
				else if (rand <= .85) { monster = MonsterType.Minotaur; }
				else { monster = MonsterType.Wyrm; }
				break;
			case 1:
				// (easy)minotaur/fire/wyrm/(hard)wyvern
				// 5/45/45/5
				if (rand <= .05) { monster = MonsterType.Minotaur; }
				else if (rand <= .5) { monster = MonsterType.FireElemental; }
				else if (rand <= .95) { monster = MonsterType.Wyrm; }
				else { monster = MonsterType.Wyvern; }
				break;
			case 2:
				// water/wyvern/(hard)dragon
				// 45/45/10
				if(rand <= .45) { monster = MonsterType.WaterElemental; }
				else if(rand <= .85) { monster = MonsterType.Wyvern; }
				else { monster = MonsterType.Dragon; }
				break;
			case 3:
				// (easy)wyvern/sasquatch/earth/dragon/(hard)hydra
				// 5/30/30/30/5
				if(rand <= .05) { monster = MonsterType.Wyvern; }
				else if(rand <= .35) { monster = MonsterType.Sasquatch; }
				else if(rand <= .65) { monster = MonsterType.EarthElemental; }
				else if(rand <= .95) { monster = MonsterType.Dragon; }
				else { monster = MonsterType.Hydra; }
				break;
			case 4:
				// (easy)sasquatch/air/hydra
				// 10/45/45
				if(rand <= .1) { monster = MonsterType.Sasquatch; }
				else if(rand <= .55) { monster = MonsterType.AirElemental; }
				else { monster = MonsterType.Hydra; }
				break;
			default:
				monster = MonsterType.Cyclops;
				break;
		}
		return monster;
		;
	}

	MonsterType[] getLostImmortalMonsters() {
		int rand = Random.Range(0,2);
		MonsterType[] monsters;
		switch(_GameStateHolder._ActivePlayer.LostImmortalKillCount) {
			case 0:
				monsters = new MonsterType[] { MonsterType.Wyrm, MonsterType.FireElemental };
				break;
			case 1:
				monsters = new MonsterType[] { MonsterType.Wyvern, MonsterType.WaterElemental };
				break;
			case 2:
				monsters = new MonsterType[] { MonsterType.Dragon, MonsterType.EarthElemental };
				break;
			case 3:
				monsters = new MonsterType[] { MonsterType.Hydra, MonsterType.AirElemental };
				break;
			default:
				monsters = new MonsterType[] { MonsterType.Hydra, MonsterType.AirElemental };
				break;
		}

		if(rand == 0) {
			monsters.Reverse();
		}
		return monsters;
	}

	void _setupPvPBattle() {
		Audio.AudioInstance.PlayMusic(MusicTrack.Dungeon);
		TileData t = _BattleData._InitialPlayer.CommanderPosition;
		setOpposition(t.Defender);
		ModalPanel.Instance().ShowOK("Battle Start", "An enemy " + t.Defender.Type + " was waiting for you.\n" +
			"You will need to choose who to send out!", () => {
				_BattleUnitPositionManager._ArmyUI.SwitchPlayer(_GameStateHolder._ActivePlayer.Type);
				_selectUnit(_GameStateHolder._ActivePlayer, (ut, i) => {
					Unit u = _GameStateHolder._ActivePlayer.PlayerArmy.GetUnits(ut)[i];
					setActiveUnit(u);
					Debug.Log("Battle Player");
					// start pre battle stuff
					StartCoroutine(_preBattle());
				});
			});
	}

	void _selectUnit(Player p, UIUnitTypeIndexCallback next) {
		_BattleUnitPositionManager.ShowUnitSelectionUI(UnitSelection.Active);
		UIPlayerUnitTypeIndexCallback selectUnit = null;
		selectUnit = (PlayerType PlayerType, UnitType ut, int i) => {
			_BattleUnitPositionManager._ArmyUI.OnClickUnit -= selectUnit;
			_BattleUnitPositionManager.HideUnitSelectionUI();
			next(ut, i);
		};
		_BattleUnitPositionManager._ArmyUI.OnClickUnit += selectUnit;
	}

	bool attacking = false;
	public void AttackButton() {
		if (_BattleData._BattleType != BattleType.PvP && activePlayer != BattlerType.Instigator ) {
			return;
		}
		attacking = true;
		iBattleable target = _oppositionBattler;
		if (_BattleData._BattleType == BattleType.PvP && activePlayer != BattlerType.Instigator) {
			target = _instigatorBattlers[0];
		}

		Attack(activePlayer, target);
		Audio.AudioInstance.PlaySFX(SoundEffect.Hit1);
		StartCoroutine(endTurn());
	}

	public int Attack(BattlerType t, iBattleable target) {
		int totalDamage = 0;
		if (t == BattlerType.Instigator) {
			totalDamage = _instigatorBattlers.Sum(x => x.GetStrength());
		}
		else {
			totalDamage = _oppositionBattler.GetStrength();
		}
		target.ReduceHP(totalDamage);
		OnBattleAbleTakeDamage (target, totalDamage);
		OnBattleAbleUpdate (target);
		Debug.Log(t + " attacked " + target.GetType() + " for " + totalDamage + " damage.");
		Debug.Log(target.GetCurrentHP() + " hp left (" + target.GetHPPercentage()*100 + "%)");
		return totalDamage;
	}

	public void Run() {
		_endBattle(BattleEndState.Loss);
	}

	void _endBattle(BattleEndState state) {

		_BattleData._EndState = state;

		if (_BattleData._BattleType == BattleType.PvP) {
			if (state != BattleEndState.Loss) {
				_BattleUnitPositionManager.ShowChest(BattlerType.Opposition);
			} else {
				_BattleUnitPositionManager.ShowChest(BattlerType.Instigator);
			}
			PlayerType p = GetPlayerTypeByBattler(activePlayer);
			ModalPanel.Instance().ShowOK(p.ToString() + " Won", "You reap the spoils and take the enemy prisoner!", () => {
				Player player = p == _GameStateHolder._ActivePlayer.Type ? _GameStateHolder._ActivePlayer : _GameStateHolder._InactivePlayer;
				_reapSpoils(player);
			});
			_BattleUnitPositionManager.ChangeToMainCamera();
			_BattleUnitPositionManager.Hide();
			return;
		}

		if (state != BattleEndState.Loss) {
			_BattleUnitPositionManager.ShowChest(BattlerType.Opposition);
			_BattleUnitPositionManager.ChangeToMainCamera();
			_BattleUnitPositionManager.Hide();
			ModalPanel.Instance().ShowOK("You Won", "You reap the spoils of the battle!", () => {
				_reapSpoils(_GameStateHolder._ActivePlayer);
			});	
		} else {
			_actuallyEndBattle();
		}
	}

	void _reapSpoils(Player p) {
		List<CardData> cards = new List<CardData>();
		if (_BattleData._BattleType != BattleType.LostImmortal) {
			// get resource card
			CardData randomcard = _CardSystem.GetRandomCard(CardType.Resource_Card);
			cards.Add(randomcard);
		} else {
			// we should add a thing here stopping you getting more battle cards (but probably won't)
			CardData randomcard = _CardSystem.GetRandomCard(LostImmortalCards.cards);
			cards.Add(randomcard);
			randomcard = _CardSystem.GetRandomCard(LostImmortalCards.cards);
			cards.Add(randomcard);
			randomcard = _CardSystem.GetRandomCard(LostImmortalCards.cards);
			cards.Add(randomcard);
			randomcard = _CardSystem.GetRandomCard(LostImmortalCards.cards);
			cards.Add(randomcard);
		}
		StartCoroutine(showCards(cards, _actuallyEndBattle, p));
	}
	IEnumerator showCards(List<CardData> cards, System.Action action, Player p) {
		if (cards != null && cards.Count > 0) {
			CardData card = cards[0];
			cards.RemoveAt(0);
			yield return new WaitForSeconds(0.1f);
			_BattleUnitPositionManager.ShowSpoils(card);
			yield return new WaitForSeconds(1f);
			_BattleUnitPositionManager.ShowSpoils(card);
			p.AddCard(card);
			ModalPanel.Instance().ShowOK("Spoils", "You found a " + card.Name + " card", () => {
				_BattleUnitPositionManager.HideSpoils();
				StartCoroutine(showCards(cards, action, p));
			});
		} else {
			action();
		}
	}

	void _actuallyEndBattle() {
		_tearDownScene();

		//heal all non-ko troops
		foreach (Unit u in _GameStateHolder._ActivePlayer.PlayerArmy.GetActiveUnits()) {
			u.Heal();
		}

		foreach (Unit u in _GameStateHolder._InactivePlayer.PlayerArmy.GetActiveUnits()) {
			u.Heal();
		}

		StartCoroutine(SceneSwitcher.ChangeScene(_OverworldScene));
	}

	void _tearDownScene() {
		removeListeners();
	}


	void setOppositionA(MonsterType t) {
        _oppositionReserveA = _MonsterManager.NewMonster(t);
        _BattleUnitPositionManager.SetReserveOppositionA(t, _oppositionReserveA);
	}

    void setOppositionB(MonsterType t){
        _oppositionReserveB = _MonsterManager.NewMonster(t);
        _BattleUnitPositionManager.SetReserveOppositionB(t, _oppositionReserveB);
	}

    void setOppositionC(MonsterType t){
        _oppositionReserveC = _MonsterManager.NewMonster(t);
        _BattleUnitPositionManager.SetReserveOppositionC(t, _oppositionReserveC);
	}

	void setOpposition(MonsterType t) {
        _oppositionBattler = _MonsterManager.NewMonster(t);
        _BattleUnitPositionManager.SetOpposition(t, _oppositionBattler);
		
	}

	void setOpposition(Unit u) {
		_BattleUnitPositionManager.SetOpposition(u.Type, _GameStateHolder._InactivePlayer.Type, u);
		_oppositionBattler = u;
	}

	void setActiveUnit(Unit u) {
		_BattleUnitPositionManager.SetActiveUnit(u, _GameStateHolder._ActivePlayer.Type);
		_instigatorBattlers.Add(u);
	}

	void setActiveArmy() {
		//wanna get all the units here to check if active, if new type and pass in unit to add unit
		List<UnitType> types = new List<UnitType> ();
		foreach (var unit in _GameStateHolder._ActivePlayer.PlayerArmy.GetUnits()) {

			if(!unit.IsDefending() && !unit.IsKO()) {
				//log unit
				_instigatorBattlers.Add(unit);

				//set up ui 
				if(!types.Contains(unit.Type)) {
					//init for new type
					_BattleUnitPositionManager.InitUnitUI(unit.Type, _GameStateHolder._ActivePlayer.Type);

					//log type
					types.Add(unit.Type);
				}

				//add individual unit ui 
				_BattleUnitPositionManager.AddUnitToUI(unit);
			}
		}
	}

	public PlayerType GetPlayerTypeByBattler(BattlerType bType)
	{
		if (bType == BattlerType.Instigator) {
			return _GameStateHolder._ActivePlayer.Type;
		} else if (_BattleData._BattleType == BattleType.PvP){
			return _GameStateHolder._InactivePlayer.Type;
		}

		return PlayerType.None;
	}

	public PlayerType GetActivePlayerType()
	{
		return GetPlayerTypeByBattler(activePlayer);
	}

	public Player GetActivePlayer()
	{
		if (activePlayer == BattlerType.Instigator) {
			return _GameStateHolder._ActivePlayer;
		} else if (_BattleData._BattleType == BattleType.PvP){
			return _GameStateHolder._InactivePlayer;
		}
		
		return null;
	}

	// Update is called once per frame
	void Update () {
	
	}

	void onCardAdded(Player p, CardData c) {
		switch(c.Type) {
			case CardType.Healing_Card:
				if(!p.HasGotCardHealing) {
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
		if(!success) {
			// we wont bother with this in the final but for now it hides the previous modal after it shows this one.
			p.ShowOK("Oh No!", "Card could not be used", null);
			return;
		}
		
		if(_BattleUnitPositionManager._HandUI.m_SelectedCardUI != null) {
			_GameStateHolder._ActivePlayer.RemoveCard(_BattleUnitPositionManager._HandUI.m_SelectedCardUI._Index);
		}
		else {
			_GameStateHolder._ActivePlayer.RemoveCard(card);
		}

	}

	void _BattlebeardPlayer_OnCardAdded(CardData card) {
		//inform ui
		onCardAdded(getPlayer(PlayerType.Battlebeard), card);
		_BattleUnitPositionManager.AddPlayerCard(PlayerType.Battlebeard, card);
	}

	void _BattlebeardPlayer_OnCardRemoved(CardData card, int index) {
		//inform ui
		_BattleUnitPositionManager.RemovePlayerCard(PlayerType.Battlebeard, card, index);
	}

	void _StormshaperPlayer_OnCardAdded(CardData card) {
		//inform ui

		onCardAdded(getPlayer(PlayerType.Stormshaper), card);
		_BattleUnitPositionManager.AddPlayerCard(PlayerType.Stormshaper, card);
	}

	void _StormshaperPlayer_OnCardRemoved(CardData card, int index) {
		//inform ui
		_BattleUnitPositionManager.RemovePlayerCard(PlayerType.Stormshaper, card, index);
	}

	void _CardSystem_RequestUnitSelection(CardData c, int numSelection, Player p, CardAction action, EndCardAction done) {
		// assume P is going to be the current player

		UnitSelection flags = UnitSelection.None;
		if(c.Type == CardType.Healing_Card) {
			flags = flags | UnitSelection.Inactive;
		}
		if(c.Type == CardType.Upgrade_Card) {
			flags = flags | UnitSelection.Active;
			flags = flags | UnitSelection.NotUpgraded;
		}
		if(c.Type == CardType.Tactic_Card) {
			flags = flags | UnitSelection.Active;
			flags = flags | UnitSelection.NotTempUpgraded;
		}

		_BattleUnitPositionManager.ShowUnitSelectionUI(flags);

		int selectedUnits = 0;
		UIPlayerUnitTypeIndexCallback selectUnit = null;
		selectUnit = (PlayerType pt, UnitType u, int i) => {
			Unit unit = _GameStateHolder._ActivePlayer.PlayerArmy.GetUnits(u)[i];
			selectedUnits++;
			// perform the action each time something is selected. This will only effect healing.
			// we don't want the player to be stuck with no units to select
			action(c, p, unit);
			// we reached the total?
			if(selectedUnits == numSelection) {
				// don't listen for namy more and hide the UI
				_BattleUnitPositionManager._ArmyUI.OnClickUnit -= selectUnit;
				_BattleUnitPositionManager.HideUnitSelectionUI();
				done(true, c, p, unit);
			}

		};
		_BattleUnitPositionManager._ArmyUI.OnClickUnit += selectUnit;
	}

	void UseCard(CardData card) {
		_CardSystem.UseCard(card, _GameStateHolder._ActivePlayer, _GameStateHolder._InactivePlayer, _GameStateHolder._gameState);
	}

}

public enum BattlerType {
	None,
	Instigator,
	Opposition
}