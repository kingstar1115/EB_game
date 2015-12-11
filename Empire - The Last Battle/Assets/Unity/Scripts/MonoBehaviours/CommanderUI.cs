using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LerpPosition), typeof(Collider))]
public class CommanderUI : MonoBehaviour
{
    public event System.Action OnDraggingCommander = delegate { };
    public event System.Action OnStartDrag = delegate { };
    public event System.Action OnCommanderGrounded = delegate { };

    public delegate void BoardAction(TileData tile);
    public event BoardAction OnDropCommander = delegate { };
    public event BoardAction OnCommanderMoved = delegate { };

    public delegate void V3Action(Vector3 vec);
    public event V3Action OnCommanderDrop = delegate { };

	public Player _Player; 
    public float _LiftedHeight;
    public float _LiftTime;
    public float _MoveTime;
    Collider _collider;
    Collider _prevHovered;
    LerpPosition _lerpPosition;
    Vector3 _toGoTo;
	HashSet<TileData> _reachableTiles;
    TileHolder _destinationTile;
    bool _liftingPiece;
    bool _hasBeenLifted;
    bool _allowMovement;
    float _targetY;
    int _defaultLayer;

	// Use this for initialization
	public void Initialise () 
    {
        _collider = this.GetComponent<Collider>();
        _lerpPosition = this.GetComponent<LerpPosition>();
        _defaultLayer = this.gameObject.layer;

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
        else if (this.transform.position.y != _LiftedHeight)
        {
            OnCommanderGrounded();
            _hasBeenLifted = false;
        }
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

    void OnMouseDown()
    {
        OnStartDrag();
    }

    void OnMouseDrag()
    {
		//only if movement is allowed
		if (_allowMovement) {
			//if hovered a tile that is reachable then move have the player move there
            Collider hoveredCollider  = null;
            TileHolder tileHolder = BoardUI.GetTileHovered();
            if (tileHolder != null)
            {

                hoveredCollider = tileHolder.GetComponent<Collider>();
                if (hoveredCollider != null)
                {
                    _toGoTo = new Vector3(hoveredCollider.transform.position.x,
                        //hoveredCollider.bounds.max.y + _collider.bounds.extents.y,
                    _LiftedHeight,
                    hoveredCollider.transform.position.z);

                    _targetY = hoveredCollider.GetComponent<Collider>().bounds.max.y + _collider.bounds.extents.y;

                    _prevHovered = hoveredCollider;

                    _destinationTile = tileHolder;

                    //dragging
                    OnDraggingCommander();
                }
            }
            else
            {
                _destinationTile = null;
            }

            if (!_hasBeenLifted && !_liftingPiece && (hoveredCollider == null || _prevHovered != hoveredCollider))
				LiftPiece ();

            //block raycast 
            this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		}
    }

    void OnMouseUp()
    {
		//if the tile hovered is not in the reachable set then back to origional tile
        if (_destinationTile == null || !_reachableTiles.Contains(_destinationTile._Tile))
        {
			//commander not moved
            _toGoTo = _Player.CommanderPosition.TileObject.transform.position;
			_targetY = _Player.CommanderPosition.TileObject.GetComponent<Collider>().bounds.max.y + _collider.bounds.extents.y;
			//OnCommanderDrop (new Vector3(_toGoTo.x, _targetY, _toGoTo.z));
        }
        else
        {
            //cammander moved
            _toGoTo = _destinationTile.transform.position;
            OnCommanderMoved(_destinationTile._Tile);
            //OnCommanderNewDestination(new Vector3(_destinationTile.transform.position.x, _targetY, _destinationTile.transform.position.z));
			OnCommanderDrop (new Vector3(_destinationTile.transform.position.x, _targetY, _destinationTile.transform.position.z));
            _destinationTile = null;
        }

        //drop the commander
        _toGoTo.y = _targetY;
        OnDropCommander(_Player.CommanderPosition);


        //raycast 
        this.gameObject.layer = _defaultLayer;
    }

    public void LiftPiece()
    {
        _liftingPiece = true;
        _lerpPosition._LerpTime = _LiftTime;
        _lerpPosition.LerpTo(new Vector3(this.transform.position.x, _LiftedHeight, this.transform.position.z));
        _toGoTo = _lerpPosition.GetEndPosition();
    }

    public void AllowPlayerMovement(HashSet<TileData> reachableTiles)
	{
		_allowMovement = true;
		_reachableTiles = reachableTiles;
	}

	public void DisablePlayerMovement()
	{
		_allowMovement = false;
	}

	public void PausePlayerMovement()
	{
		_lerpPosition.PauseLerp ();
	}

	public void ContinuePlayerMovement()
	{
		_lerpPosition.StartLerp ();
	}

    public void UpdateToPlayerPosition()
    {
        this.transform.position = new Vector3(_Player.CommanderPosition.TileObject.transform.position.x, 
            _targetY = _Player.CommanderPosition.Height + _collider.bounds.extents.y,
            _Player.CommanderPosition.TileObject.transform.position.z);
    }

    public HashSet<TileData> GetReachableTiles()
    {
        return _reachableTiles;
    }
}
