using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardUI : MonoBehaviour 
{
    public Board _Board;
    HashSet<Animator> _animatingTiles;

    public void Init()
    {
        _animatingTiles = new HashSet<Animator>();
    }

    public void PlayerPrompt_MovableTiles(HashSet<TileData> reachableTiles)
    {
        //for each of the tiles set animator variable 
        Animator nextAnimator;
        foreach (var tile in reachableTiles)
        {
            //grab animator
            nextAnimator = tile.TileObject.GetComponentInChildren<Animator>();

            //if already animating then reset it
            if (nextAnimator.playbackTime > 0)
                nextAnimator.playbackTime = 0;

            nextAnimator.SetBool("CanBeMovedTo", true);
            _animatingTiles.Add(nextAnimator);
        }
    }

    public void PlayerPrompt_DefaultTiles()
    {
        foreach (var tile in _animatingTiles)
        {
            tile.SetBool("CanBeMovedTo", false);
        }
    }

	public static TileHolder GetTileHovered()
	{
		Collider outCollider;
		if (GetTileHovered_Position (out outCollider)) {
			TileHolder th;
			if((th = outCollider.GetComponent<TileHolder>()) != null)
				return th;
			else
			{
				Debug.LogError("Game Object Labeled Tile with no TileHolder!!! :O");
				return null;
			}
		} else {
			return null;
		}
	}

    public static bool GetTileHovered_Position(out Collider tileCollider)
    {
        //raycast for a collider tagged tile and return position
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, (1 << LayerMask.NameToLayer("Tiles"))))
        {
            if (hit.collider.gameObject.tag == "Tile")
            {
                tileCollider = hit.collider;
                return true;
            }
        }

        tileCollider = null;
        return false;
    }

    
}
