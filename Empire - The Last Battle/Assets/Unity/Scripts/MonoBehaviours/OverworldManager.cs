using UnityEngine;
using System.Collections;

public class OverworldManager : MonoBehaviour
{
	public OverworldUI _OverworldUI;
	public CardSystem _CardSystem;
	public Board _Board;
	public Player _Player1;

	// Use this for initialization
	void Start () 
	{
		//new game setup
		_Board.Initialise();
		_OverworldUI.Initialise();

		//try get the player start tile
		if (_Board._P1StartTile != null)
			_Player1.CommanderPosition =_Board._P1StartTile;
        else
            Debug.LogError("Player1 start tile not set");
        
        //snap player to start position
        _OverworldUI.UpdateCommanderPosition();

        //event listeners
        _OverworldUI.OnCommanderMove += _OverworldUI_OnCommanderMove;

		//allow player movement for the start ****JUST FOR TESTING****
		_OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(_Player1.CommanderPosition, 1));
	}

    void _OverworldUI_OnCommanderMove(TileData tile)
    {
        //set new position for the player (should depend on whose players turn it is)
        _Player1.CommanderPosition = tile;

        //****JUST FOR TESTING**** set new reachable tiles
        _OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(_Player1.CommanderPosition, 1));
    }
	
	// Update is called once per frame
	void Update () {
	
	}


}
