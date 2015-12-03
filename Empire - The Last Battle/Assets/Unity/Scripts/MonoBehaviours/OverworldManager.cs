using UnityEngine;
using System.Collections;

public class OverworldManager : MonoBehaviour
{
	public OverworldUI _OverworldUI;
	public Board _Board;
	public Player _Player1;
	public int _Player1StartX;
	public int _Player1StartY;

	// Use this for initialization
	void Start () 
	{
		//new game setup
		_Board.Init ();

		//try get the player start tile
		Tile startTile1 = _Board.GetTileAt (_Player1StartX, _Player1StartY);
		if (startTile1 != null)
			_Player1.CommanderPosition = _Board.GetTileAt (_Player1StartX, _Player1StartY);
		else
			Debug.LogError ("Player start tile indexes are out of bounds");

		//allow player movement for the start 
		_OverworldUI.AllowPlayerMovement(_Board.GetReachableTiles(_Player1.CommanderPosition, 1));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
