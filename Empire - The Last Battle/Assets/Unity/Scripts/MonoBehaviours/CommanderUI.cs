using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LerpPosition), typeof(Collider))]
public class CommanderUI : MonoBehaviour
{
    public event System.Action OnDraggingCommander = delegate { };
    public event System.Action OnDropCommander = delegate { };

    public float _LiftedHeight;
    public float _LiftTime;
    public float _MoveTime;
    Collider _collider;
    Collider _prevHovered;
    LerpPosition _lerpPosition;
    bool _liftingPiece;
    bool _hasBeenLifted;
    float _targetY;
    Vector3 _toGoTo;

	// Use this for initialization
	void Start () 
    {
        _collider = this.GetComponent<Collider>();
        _lerpPosition = this.GetComponent<LerpPosition>();

        //event listener
        _lerpPosition.OnLerpFinished += _lerpPosition_OnLerpFinished;
	}

    void _lerpPosition_OnLerpFinished()
    {
        //if lifting piece then not doin it anymore
        if (_liftingPiece)
        {
            _liftingPiece = false;
            _hasBeenLifted = true;
        }
        else if(this.transform.position.y != _LiftedHeight)
            _hasBeenLifted = false;
    }
	
	// Update is called once per frame
	void Update () 
    {
        //update the lerp goto point
        if (_hasBeenLifted && _lerpPosition.GetEndPosition() != _toGoTo)
        {
            _lerpPosition._LerpTime = _MoveTime;
            _lerpPosition.LerpTo(_toGoTo);
        }
	}

    void OnMouseDrag()
    {
        //if hovered a tile then move have the player move there
        Collider hoveredCollider;
        if (BoardUI.GetTileHovered_Position(out hoveredCollider))
        {
            _toGoTo = new Vector3(hoveredCollider.transform.position.x,
                //hoveredCollider.bounds.max.y + _collider.bounds.extents.y,
                _LiftedHeight,
                hoveredCollider.transform.position.z);

            _targetY = hoveredCollider.bounds.max.y + _collider.bounds.extents.y;

            _prevHovered = hoveredCollider;   
            
            //dragging
            OnDraggingCommander();
        }

        if (!_hasBeenLifted && !_liftingPiece && (hoveredCollider == null || _prevHovered != hoveredCollider))
            LiftPiece();
    }

    void OnMouseUp()
    {
        //drop the commander
        _toGoTo.y = _targetY;
        OnDropCommander();
    }

    public void LiftPiece()
    {
        _liftingPiece = true;
        _lerpPosition._LerpTime = _LiftTime;
        _toGoTo = this.transform.position;
        _targetY = this.transform.position.y;
        _lerpPosition.LerpTo(new Vector3(this.transform.position.x, _LiftedHeight, this.transform.position.z));
    }
}
