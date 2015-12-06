using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OverworldUI : MonoBehaviour
{
    public delegate void BoardAction(Tile tile);
    public event BoardAction OnCommanderMove = delegate { };

    public bool _TileHover;
    public CommanderUI _CommanderUI;
    public CameraMovement _CameraMovement;

    // Use this for initialization
    void Start()
    {
        _CommanderUI.OnCommanderMoved += _CommanderUI_OnCommanderMoved;
        _CommanderUI.OnStartDrag += _CommanderUI_OnStartDrag;
        _CommanderUI.OnCommanderNewDestination += _CommanderUI_OnCommanderNewDestination;
    }

    void _CommanderUI_OnCommanderNewDestination(Vector3 vec)
    {
        _CameraMovement.EnableCameraMovement(vec);
    }

    void _CommanderUI_OnStartDrag()
    {
        _CameraMovement.DisableCameraMovement();
    }

    void _CommanderUI_OnCommanderMoved(Tile tile)
    {
        OnCommanderMove(tile);
    }

    public void AllowPlayerMovement(HashSet<Tile> reachableTiles)
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
