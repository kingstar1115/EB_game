using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LerpPosition), typeof(Collider))]
public class CommanderUI : MonoBehaviour
{
    Collider _collider;
    LerpPosition _lerpPosition; 

	// Use this for initialization
	void Start () 
    {
        _collider = this.GetComponent<Collider>();
        _lerpPosition = this.GetComponent<LerpPosition>();
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
            //this.transform.position = new Vector3(hoveredCollider.transform.position.x,
           //     hoveredCollider.bounds.max.y + _collider.bounds.extents.y, 
            //    hoveredCollider.transform.position.z);

            Vector3 toGoTo = new Vector3(hoveredCollider.transform.position.x,
                hoveredCollider.bounds.max.y + _collider.bounds.extents.y,
                hoveredCollider.transform.position.z);

            if(_lerpPosition.GetEndPosition() != toGoTo)
                _lerpPosition.LerpTo(toGoTo);
        }
    }
}
