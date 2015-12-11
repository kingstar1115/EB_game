using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OverworldUI : MonoBehaviour
{
    public delegate void BoardAction(TileData tile);
    public event BoardAction OnCommanderMove = delegate { };

    public bool _TileHover;
    public CommanderUI _CommanderUI;
    public CameraMovement _CameraMovement;
    public BoardUI _BoardUI;

    // Use this for initialization
    public void Initialise()
    {
        _BoardUI.Init();
		_CommanderUI.Initialise();
        _CommanderUI.OnCommanderMoved += _CommanderUI_OnCommanderMoved;
        _CommanderUI.OnStartDrag += _CommanderUI_OnStartDrag;
		_CommanderUI.OnCommanderDrop += _CommanderUI_OnCommanderDrop;
		_CommanderUI.OnCommanderGrounded += _CommanderUI_Grounded;
        _CommanderUI.OnDropCommander += _CommanderUI_OnDropCommander;
    }

    void _CommanderUI_OnDropCommander(TileData tile)
    {
        _BoardUI.PlayerPrompt_DefaultTiles();
    }

	void _CommanderUI_Grounded()
	{
		//if camera is not moving to a new position then enable it 
		if (!_CameraMovement.IsLerping ())
			_CameraMovement.EnableCameraMovement ();
	}

	void _CommanderUI_OnCommanderDrop(Vector3 vec)
	{
		_CameraMovement.EnableCameraMovement(vec);
	}

    void _CommanderUI_OnStartDrag()
    {
        _CameraMovement.DisableCameraMovement();
        _BoardUI.PlayerPrompt_MovableTiles(_CommanderUI.GetReachableTiles());
    }

    void _CommanderUI_OnCommanderMoved(TileData tile)
    {
        OnCommanderMove(tile);
    }

    public void AllowPlayerMovement(HashSet<TileData> reachableTiles)
    {
        _CommanderUI.AllowPlayerMovement(reachableTiles);
    }

    public void DisablePlayerMovement()
    {
        _CommanderUI.DisablePlayerMovement();
    }

    public void UpdateCommanderPosition()
    {
        _CommanderUI.UpdateToPlayerPosition();
    }
}
