using UnityEngine;
using System.Collections;

public class OverworldManager : MonoBehaviour
{
	public OverworldUI _OverworldUI;
	public CardSystem _CardSystem;
	public Board _Board;
	public Player _Player1;
	public int _Player1StartX;
	public int _Player1StartY;

	// Use this for initialization
	void Start () 
	{
		//new game setup
		_Board.Initialise();
		_OverworldUI.Initialise();

		//try get the player start tile
		TileData startTile1 = _Board.GetTileAt (_Player1StartX, _Player1StartY);
        if (startTile1 != null)
        {
            _Player1.CommanderPosition = _Board.GetTileAt(_Player1StartX, _Player1StartY);
        }
        else
            Debug.LogError("Player start tile indexes are out of bounds");
        
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
