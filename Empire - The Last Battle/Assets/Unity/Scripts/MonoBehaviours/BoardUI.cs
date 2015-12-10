﻿using UnityEngine;
using System.Collections;

public class BoardUI : MonoBehaviour 
{
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
