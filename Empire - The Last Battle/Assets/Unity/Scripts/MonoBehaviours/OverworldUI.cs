using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OverworldUI : MonoBehaviour
{
    public delegate void BoardAction(TileData tile);
    public event BoardAction OnCommanderMove = delegate { };

    public bool _TileHover;
    public CommanderUI _CommanderUI;

    // Use this for initialization
    void Start()
    {
        _CommanderUI.OnCommanderMoved += _CommanderUI_OnCommanderMoved;
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
