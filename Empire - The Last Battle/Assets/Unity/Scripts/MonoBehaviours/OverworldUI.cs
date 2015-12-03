using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OverworldUI : MonoBehaviour
{
    public bool _TileHover;
    public CommanderUI _CommanderUI;

    // Use this for initialization
    void Start()
    {
    }

    public void AllowPlayerMovement(HashSet<Tile> reachableTiles)
    {
        _CommanderUI.AllowPlayerMovement(reachableTiles);
    }
}
