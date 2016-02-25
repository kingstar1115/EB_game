using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameStateHolder : ScriptableObject
{
	public GameState _gameState;
	public Player _ActivePlayer;
	public Player _InactivePlayer;
}

public enum GameState
{
	Paused = 0x1,
	Overworld = 0x2,
	PreBattle = 0x4,
	Battle = 0x8,
}
