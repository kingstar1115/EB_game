using UnityEngine;
using System.Collections;
<<<<<<< HEAD

public class OverworldUI : MonoBehaviour 
{
    public CommanderUI _CommanderUI;

	// Use this for initialization
	void Start () 
    {
        _CommanderUI.OnDraggingCommander += _CommanderUI_OnDraggingCommander;	
	}

    void _CommanderUI_OnDraggingCommander()
    {
        throw new System.NotImplementedException();
    }
=======
using System.Collections.Generic;

public class OverworldUI : MonoBehaviour 
{
	public bool _TileHover;
	public CommanderUI _CommmanderUI;

	// Use this for initialization
	void Start () {
	
	}
>>>>>>> origin/origin/P11_CommanderMovement
	
	// Update is called once per frame
	void Update () {
	
	}
<<<<<<< HEAD
=======

	public void AllowPlayerMovement(HashSet<Tile> reachableTiles)
	{
		_CommmanderUI.AllowPlayerMovement (reachableTiles);
	}
>>>>>>> origin/origin/P11_CommanderMovement
}
