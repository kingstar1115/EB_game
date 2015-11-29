using UnityEngine;
using System.Collections;

public class CommanderUI : MonoBehaviour
{

    Collider collider;

	// Use this for initialization
	void Start () 
    {
        collider = this.GetComponent<Collider>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseDrag()
    {
        //float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        //transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));

        //if hovered a tile then move player to that place
        Collider hoveredCollider;
        if (BoardUI.GetTileHovered_Position(out hoveredCollider))
        {
            Debug.Log("Dragging over a tile");
            this.transform.position = new Vector3(hoveredCollider.transform.position.x, 
                hoveredCollider.bounds.max.y + collider.bounds.extents.y, 
                hoveredCollider.transform.position.z);
        }
    }
}
