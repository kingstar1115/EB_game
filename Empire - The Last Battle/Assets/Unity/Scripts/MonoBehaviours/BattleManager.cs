using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class BattleManager : MonoBehaviour {

	public GameStateHolder _GameStateHolder;
	public BattleData _BattleData;
	public BattleUnitPositionManager _BattleUnitPositionManager;
	public Player _StormshaperPlayer;
	public Player _BattlebeardPlayer;
	public string _OverworldScene;
	public BattlerType activePlayer;
	public MonsterManager _MonsterManager;

	List<Unit> _instigatorBattlers;
	iBattleable _oppositionBattler;
	iBattleable _oppositionReserveA;
	iBattleable _oppositionReserveB;
	iBattleable _oppositionReserveC;

	// Use this for initialization
	void Start () {
		SceneFaderUI.ScreenFader.StartFadeOverTime(SceneFaderUI.FadeDir.FadeOut);
		// set up the players
		_StormshaperPlayer = _GameStateHolder._ActivePlayer.Type == PlayerType.Stormshaper ? _GameStateHolder._ActivePlayer : _GameStateHolder._InactivePlayer;
		_BattlebeardPlayer = _GameStateHolder._ActivePlayer.Type == PlayerType.Battlebeard ? _GameStateHolder._ActivePlayer : _GameStateHolder._InactivePlayer;
		// set up the units and stuff

		Debug.Log(_BattleData._BattleType);

		_instigatorBattlers = new List<Unit>();
		_GameStateHolder._ActivePlayer.OnUpdateUnit += OnUpdateUnit;
		_GameStateHolder._ActivePlayer.OnAddUnit += OnAddUnit;
		// add player event listeners
		_GameStateHolder._ActivePlayer.Initialise();

		if (_BattleData._BattleType == BattleType.LostImmortal) {
			_setupLostImmortalBattle();
		}
		else if (_BattleData._BattleType == BattleType.Monster || _BattleData._BattleType == BattleType.Card) {
			_setupMonsterBattle();
		}
		else if (_BattleData._BattleType == BattleType.PvP) {
			_setupPvPBattle();
		}

		// set the state to pre battle
		_GameStateHolder._gameState = GameState.PreBattle;

		// start pre battle stuff
		StartCoroutine(_preBattle());
	}

	void removeListeners() {
		_GameStateHolder._ActivePlayer.PlayerArmy.RemoveListeners();
		_GameStateHolder._InactivePlayer.PlayerArmy.RemoveListeners();
	}

	private void OnAddUnit(Player p, Unit u) {
		_instigatorBattlers.Add(u);
		_BattleUnitPositionManager.SetUnit(u.Type, p.Type);
		
	}

	private void OnUpdateUnit(Player p, Unit u) {
		if (u.IsKO() && !p.PlayerArmy.GetActiveUnitTypes().Contains(u.Type)) {
			_BattleUnitPositionManager.RemoveUnit(u.Type);
			_instigatorBattlers.Remove(u);
		}
		else if (!_instigatorBattlers.Contains(u)) {
			OnAddUnit(p, u);
		}
	}

	void setActiveUnits() {
		_instigatorBattlers = _GameStateHolder._ActivePlayer.PlayerArmy.GetActiveUnits();
	}

	IEnumerator _preBattle() {
		yield return new WaitForSeconds(2);
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

		if(activeCard != null ^ inactiveCard != null) {
			// one player has a priority card, which one?
			if(inactiveCard != null) {
				_GameStateHolder._InactivePlayer.RemoveCard(inactiveCard);
				activePlayer = BattlerType.Opposition;
				Debug.Log(_GameStateHolder._InactivePlayer.GetType() + " used priority card to go first");
			} else {
				_GameStateHolder._ActivePlayer.RemoveCard(activeCard);
				Debug.Log(_GameStateHolder._ActivePlayer.GetType() + " used priority card to go first");
				activePlayer = BattlerType.Instigator;
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
		Debug.Log(activePlayer + " goes first");
	}

	void switchPlayer() {
		activePlayer = activePlayer == BattlerType.Instigator ? BattlerType.Opposition : BattlerType.Instigator;
		startTurn();
	}

	void startTurn() {
		if (_BattleData._BattleType == BattleType.PvP || activePlayer == BattlerType.Instigator) {
			startTurnPlayer();
		}
		else {
			startTurnMonster();
		}
	}

	void startTurnPlayer() {
		// show player UI
	}

	void startTurnMonster() {
		// monster will attack!

		// AI goes here.

		// just attack the first one for now
		Debug.Log("Enemy attacks!");
		Attack(activePlayer, _instigatorBattlers[0]);
		StartCoroutine(endTurn());
	}

   IEnumerator endTurn() {
		yield return new WaitForSeconds(1);
		// do end turn stuff

		if (_instigatorBattlers.Count() == 0) {
			LoseBattle();
		} else if (_oppositionBattler.IsKO())  {

			// there's some lost immortal switcharoo here but for now it's just one.
			_BattleUnitPositionManager.RemoveOpposition();
			WinBattle();
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


		startTurn();
		// 


	}

	void _setupMonsterBattle() {
		Debug.Log("Battle Monster");
		setActiveArmy();
		// generate a monster based on the player 

		setOpposition(MonsterType.Minotaur);
	}

	void _setupLostImmortalBattle() {
		Debug.Log("Battle Lost Immortal");
		setActiveArmy();
		// generate a lost immortal based on the beaten lost immortals (or random)
		setOpposition(MonsterType.LostImmortalBb1);
		setOppositionA(MonsterType.Cyclops);
		setOppositionB(MonsterType.Minotaur);
	}

	void _setupPvPBattle() {

		// yeah.. for this we have to get the unit selection UI out for both players

		// for now it's like this
		setOpposition(_GameStateHolder._InactivePlayer.PlayerArmy.GetActiveUnits()[0]);
		setActiveUnit(_GameStateHolder._ActivePlayer.PlayerArmy.GetActiveUnits()[0]);
		Debug.Log("Battle Player");
	}

	public void AttackButton() {
		Attack(activePlayer, _oppositionBattler);
		StartCoroutine(endTurn());
	}

	public int Attack(BattlerType t, iBattleable target) {
		int totalDamage = 0;
		if(t == BattlerType.Instigator) {
			totalDamage = _instigatorBattlers.Sum(x => x.GetStrength());
		}
		else {
			totalDamage = _oppositionBattler.GetStrength();
		}
		target.ReduceHP(totalDamage);
		Debug.Log(t + " attaked " + target.GetType() + " for " + totalDamage + " damage.");
		Debug.Log(target.GetCurrentHP() + " hp left (" + target.GetHPPercentage()*100 + "%)");
		return totalDamage;
	}

	public void WinBattle() {
		_BattleData._EndState = BattleEndState.Win;
		_endBattle();
	}

	public void LoseBattle() {
		_BattleData._EndState = BattleEndState.Loss;

		// temporary
		foreach (Unit u in _GameStateHolder._ActivePlayer.PlayerArmy.GetActiveUnits()) {
			u.ReduceHP(1000);
		}

		_endBattle();
	}

	void _endBattle() {

		//heal all non-ko troops
		foreach (Unit u in _GameStateHolder._ActivePlayer.PlayerArmy.GetActiveUnits()) {
			u.Heal();
		}

		foreach (Unit u in _GameStateHolder._InactivePlayer.PlayerArmy.GetActiveUnits()) {
			u.Heal();
		}


		_tearDownScene();

		StartCoroutine(SceneSwitcher.ChangeScene(_OverworldScene));
	}

	void _tearDownScene() {
		removeListeners();
	}


	void setOppositionA(MonsterType t) {
		_BattleUnitPositionManager.SetReserveOppositionA(t);
		_oppositionBattler = _MonsterManager.NewMonster(t);
	}

	void setOppositionB(MonsterType t) {
		_BattleUnitPositionManager.SetReserveOppositionB(t);
		_oppositionBattler = _MonsterManager.NewMonster(t);
	}

	void setOpposition(MonsterType t) {
		_BattleUnitPositionManager.SetOpposition(t);
		_oppositionBattler = _MonsterManager.NewMonster(t);
	}

	void setOpposition(Unit u) {
		_BattleUnitPositionManager.SetOpposition(u.Type, _GameStateHolder._InactivePlayer.Type);
		_oppositionBattler = u;
	}

	void setActiveUnit(Unit u) {
		_BattleUnitPositionManager.SetActiveUnit(u.Type, _GameStateHolder._ActivePlayer.Type);
		_instigatorBattlers.Add(u);
	}

	void setActiveArmy() {
		foreach(UnitType t in _GameStateHolder._ActivePlayer.PlayerArmy.GetActiveUnitTypes()) {
			_BattleUnitPositionManager.SetUnit(t, _GameStateHolder._ActivePlayer.Type);
		}
		_instigatorBattlers = _GameStateHolder._ActivePlayer.PlayerArmy.GetActiveUnits();
	}

	// Update is called once per frame
	void Update () {
	
	}
}

public enum BattlerType {
	Instigator,
	Opposition
}