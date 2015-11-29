using UnityEngine;
using System.Collections;

public class BoardUI : MonoBehaviour 
{

    public static bool GetTileHovered_Position(out Collider tileCollider)
    {
        //raycast for a collider tagged tile and return position
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == "Tile")
            {
                Debug.Log("TileHovered");
                tileCollider = hit.collider;
                return true;
            }
        }

        tileCollider = null;
        return false;
    }
}
