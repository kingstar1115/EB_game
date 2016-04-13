using UnityEngine;
using System.Collections;

public class BattleData : ScriptableObject {
	// Player that started the battle
	public Player _InitialPlayer;
	public BattleType _BattleType;
	public BattleEndState _EndState;
	public Unit _LostUnit;
}

public enum BattleType {
	Monster,
	PvP,
	LostImmortal,
	Card
}

public enum BattleEndState {
	None,
	Flee,
	Win,
	Loss,
	Draw
}