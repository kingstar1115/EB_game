using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameStateHolder : ScriptableObject
{
	public GameState _gameState;
}

public enum GameState
{
	Paused = 0x0,
	Overworld = 0x1,
	PreBattle = 0x2,
	Battle = 0x4,
}
