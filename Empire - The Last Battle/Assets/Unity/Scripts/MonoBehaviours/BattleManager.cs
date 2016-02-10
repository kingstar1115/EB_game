using UnityEngine;
using System.Collections;

public class BattleManager : MonoBehaviour {

	public GameStateHolder _GameStateHolder;
	public BattleData _BattleData;
	public BattleUnitsManager _BattleUnitsManager;
	public string _OverworldScene;

	// Use this for initialization
	void Start () {
		
		// set up the scene
		_setupScene();
		// set up the units and stuff

		Debug.Log(_BattleData._BattleType);

		if (_BattleData._BattleType == BattleType.LostImmortal) {
			_setupLostImmortalBattle();
		}
		else if (_BattleData._BattleType == BattleType.Monster) {
			_setupMonsterBattle();
		}
		else if (_BattleData._BattleType == BattleType.PvP) {
			_setupPvPBattle();
		}

		// set the state to pre battle
		_GameStateHolder._gameState = GameState.PreBattle;

		// start pre battle stuff

		_startBattle();
	}


	void _startBattle() {
		// set state to battle
		_GameStateHolder._gameState = GameState.Battle;

	}

	void _setupScene() {
		TerrainType t = _BattleData._InitialPlayer.CommanderPosition.Terrain;
		BuildingType b = _BattleData._InitialPlayer.CommanderPosition.Building;
		Debug.Log(t.ToString() + ' ' + b.ToString());
	}

	void _setupMonsterBattle() {
		Debug.Log("Battle Monster");
		foreach (UnitType t in _GameStateHolder._ActivePlayer.PlayerArmy.GetActiveUnitTypes()) {
			_BattleUnitsManager.SetUnit(t, _GameStateHolder._ActivePlayer.Type);
		}
		// generate a monster based on the player 
		_BattleUnitsManager.SetOpposition(Monster.Minotaur);
	}

	void _setupLostImmortalBattle() {
		Debug.Log("Battle Lost Immortal");
		foreach (UnitType t in _GameStateHolder._ActivePlayer.PlayerArmy.GetActiveUnitTypes()) {
			_BattleUnitsManager.SetUnit(t, _GameStateHolder._ActivePlayer.Type);
		}
		// generate a lost immortal based on the beaten lost immortals (or random)
		_BattleUnitsManager.SetOpposition(LostImmortal.bb1);
	}

	void _setupPvPBattle() {

		// yeah.. for this we have to get the unit selection UI out for both players

		// for now it's like this

		_BattleUnitsManager.SetOpposition(UnitType.Archer, _GameStateHolder._InactivePlayer.Type);
		_BattleUnitsManager.SetActiveUnit(UnitType.Archer, _GameStateHolder._ActivePlayer.Type);
		Debug.Log("Battle Player");
	}

	public void WinBattle() {
		_BattleData._EndState = BattleEndState.Win;
		_endBattle();
	}

	public void LoseBattle() {
		_BattleData._EndState = BattleEndState.Loss;
		_endBattle();
	}

	void _endBattle() {
		Application.LoadLevel(_OverworldScene);
	}


	// Update is called once per frame
	void Update () {
	
	}
}
