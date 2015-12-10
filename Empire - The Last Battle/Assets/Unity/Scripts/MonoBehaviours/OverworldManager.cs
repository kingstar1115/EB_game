using UnityEngine;
using System.Collections;

public class OverworldManager : MonoBehaviour
{
	public OverworldUI _OverworldUI;
	public CardSystem _CardSystem;
	public Board _Board;
	public Player _BattlebeardPlayer;

	// Use this for initialization
	void Start () 
	{
		//new game setup
		_Board.Initialise();
		_OverworldUI.Initialise();

		//try get the battleboard start tile
		if (_Board._BBStartTile != null)
			_BattlebeardPlayer.CommanderPosition =_Board._BBStartTile;
        else
            Debug.LogError("Battleboard start tile not set");
        
        //snap player to start position
        _OverworldUI.UpdateCommanderPosition();

        //event listeners
        _OverworldUI.OnCommanderMove += _OverworldUI_OnCommanderMove;

		//allow player movement for the start ****JUST FOR TESTING****
		_OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(_BattlebeardPlayer.CommanderPosition, 1));
	}

    void _OverworldUI_OnCommanderMove(TileData tile)
    {
        //set new position for the player (should depend on whose players turn it is)
        _BattlebeardPlayer.CommanderPosition = tile;

        //****JUST FOR TESTING**** set new reachable tiles
        _OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(_BattlebeardPlayer.CommanderPosition, 1));
    }
	
	// Update is called once per frame
	void Update () {
	
	}


}
