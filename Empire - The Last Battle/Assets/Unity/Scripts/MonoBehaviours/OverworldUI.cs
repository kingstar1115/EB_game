using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OverworldUI : MonoBehaviour 
{
	public bool _TileHover;
	public CommanderUI _CommmanderUI;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AllowPlayerMovement(HashSet<Tile> reachableTiles)
	{
		_CommmanderUI.AllowPlayerMovement (reachableTiles);
	}
}
