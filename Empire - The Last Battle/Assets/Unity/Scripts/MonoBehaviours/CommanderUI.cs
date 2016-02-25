using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LerpPosition), typeof(LerpRotation), typeof(Collider))]
public class CommanderUI : MonoBehaviour
{
    public event System.Action OnDraggingCommander = delegate { };
    public event System.Action OnStartDrag = delegate { };

    public delegate void BoardAction(TileData tile);
    public event BoardAction OnDropCommander = delegate { };
	public event BoardAction OnCommanderGrounded = delegate { };
    public event BoardAction OnCommanderMoved = delegate { };
	public event BoardAction OnCommanderForceMoved = delegate { };

    public delegate void V3Action(Vector3 vec);
    public event V3Action OnCommanderDrop = delegate { };

    public Player _Player;
	public HandUI _HandUI;
    public float _LiftedHeight;
    public float _LiftTime;
    public float _MoveTime;

    LerpPosition _lerpPosition;
	LerpRotation _lerpRotation;
    Vector3 _toGoTo;
    HashSet<TileData> _reachableTiles;
    TileHolder _destinationTile;
    Vector3 _destination;
	Quaternion _destinationRotation;
    bool _liftingPiece;
    bool _hasBeenLifted;
    bool _allowMovement;
    bool _dragging;
    float _targetY;
    int _defaultLayer;
	bool _forceMove;
	bool _forceMoveEvents;

    bool _paused;
    public bool _Paused
    {
        get
        {
            return _paused;
        }

        set
        {
            if (value)
            {
                PausePlayerMovement();
                _allowMovement = false;
            }
            else
            {
                ContinuePlayerMovement();
                _allowMovement = true;
            }

            _paused = value;
        }
    }

	public Vector3 getPosition() {
		return _toGoTo;
	}

	public void RemoveListeners() {
		OnDraggingCommander = delegate { };
		OnStartDrag = delegate { };
		OnDropCommander = delegate { };
		OnCommanderGrounded = delegate { };
		OnCommanderMoved = delegate { };
		OnCommanderForceMoved = delegate { };
		OnCommanderDrop = delegate { };
	}

    // Use this for initialization
	public void Initialise () 
    {
        _lerpPosition = this.GetComponent<LerpPosition>();
		_lerpRotation = this.GetComponent<LerpRotation>();
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

			if (_forceMove) {
				// move over the destination tile
				_lerpPosition._LerpTime = _MoveTime;
				_toGoTo = _destination;
				_toGoTo.y = _LiftedHeight;
				_lerpPosition.LerpTo(_toGoTo);
			}
        }
		// if commander has been dropped
        else if (this.transform.position.y != _LiftedHeight)
        {
			OnCommanderGrounded(_Player.CommanderPosition);
		
			if (_destinationTile != null){
				if (_forceMove && _forceMoveEvents) {
					OnCommanderForceMoved(_destinationTile._Tile);
				}
				else {
					OnCommanderMoved(_destinationTile._Tile);
				}
			}
			_destinationTile = null;
            _hasBeenLifted = false;

			if (_forceMove) {
				// force move has ended
				_forceMove = false;
				_forceMoveEvents = false;
			}
		}
		// if commander has been force moved over a tile
		else if (_forceMove && _hasBeenLifted) {
			_toGoTo.y = _targetY;
			if (_destinationRotation != null) {
				_lerpRotation.LerpTo(_destinationRotation);
			}
		}
    }
	
	// Update is called once per frame
	void Update () 
    {
        //update the lerp goto point
		if (_hasBeenLifted && _lerpPosition.GetEndPosition() != _toGoTo) {
            _lerpPosition._LerpTime = _MoveTime;
            _lerpPosition.LerpTo(_toGoTo);
        }
	}

    void OnMouseDown()
    {
        //OnStartDrag();
    }

    void OnMouseDrag()
    {
		//only if movement is allowed
		if (_allowMovement) {

			if (!_dragging) {
				OnStartDrag ();
				_dragging = true;
			}

			//if hovered a tile that is reachable then move have the player move there
			Collider tileParent;
			TileHolder tileHolder = null;
			if (BoardUI.GetTileHovered_Position (out tileParent)){
				tileHolder = tileParent.GetComponentInChildren<TileHolder> ();
			}

            if (tileHolder != null)
            {

                //try get the commander position marker
                GameObject posMarker = getCommanderMarker(tileHolder);
                if (posMarker != null)
                {
                    _toGoTo = posMarker.transform.position;
                    _destination = _toGoTo;
					_destinationRotation = posMarker.transform.rotation;
                }
                else
                {
                    _toGoTo = tileHolder.transform.position;
                    _destination = _toGoTo;
                }

                _toGoTo.y = _LiftedHeight;

				_targetY = tileHolder._Tile.Height;

                _destinationTile = tileHolder;

                //dragging
                OnDraggingCommander();
            }
            else
            {
                _destinationTile = null;
            }

			if (!_hasBeenLifted && !_liftingPiece) {// && _destinationTile == null )//|| _prevHovered != hoveredCollider))
				LiftPiece ();
			}
            //block raycast 
            this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		}
    }

    void OnMouseUp()
    {
        _dragging = false;

		//if the tile hovered is not in the reachable set then back to original tile
        if (_destinationTile == null || !_reachableTiles.Contains(_destinationTile._Tile))
        {
			//commander not moved
			_destinationTile = null;
            GameObject posMarker = getCommanderMarker(_Player.CommanderPosition.TileObject.GetComponentInChildren<TileHolder>());
            _toGoTo = (posMarker!=null) ? posMarker.transform.position: _Player.CommanderPosition.TileObject.transform.position;
			_targetY = _Player.CommanderPosition.Height;
        }
        else
        {
            //commander moved
            _toGoTo = _destination;
			OnCommanderDrop(new Vector3(_destination.x, _targetY, _destination.z));
           // _destinationTile = null;
        }

        //drop the commander
        _toGoTo.y = _targetY;
		if (_destinationRotation != null) {
			// rotate to the new marker rotation
			_lerpRotation.LerpTo(_destinationRotation);
		}
        OnDropCommander(_Player.CommanderPosition);


        //raycast 
        this.gameObject.layer = _defaultLayer;
    }


	// moves the commander to a certain position. fires events as usual
	public void MoveCommander(TileData tile){
		if (_hasBeenLifted || _liftingPiece) {
			return;
		}
		TileHolder tileHolder = tile.TileObject.GetComponentInChildren<TileHolder>();
		if (tileHolder != null) {

			_forceMove = true;
			
			//try get the commander position marker
			GameObject posMarker = getCommanderMarker(tileHolder);
			if (posMarker != null) {
				_destination = posMarker.transform.position;
				_destinationRotation = posMarker.transform.rotation;
			}
			else {
				_destination = tileHolder.transform.position;
			}

			_targetY = tileHolder._Tile.Height;
			_destinationTile = tileHolder;

			LiftPiece();
		}
	}

	// moves the commander to a position and fires different move events
	public void ForceMoveCommander(TileData tile) {
		_forceMoveEvents = true;
		MoveCommander(tile);
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
		_lerpPosition.ResumeLerp();
	}

    public void UpdateToPlayerPosition()
    {
        GameObject posMarker = getCommanderMarker(_Player.CommanderPosition.TileObject.GetComponentInChildren<TileHolder>());
        Vector3 newPosition = (posMarker != null) ? posMarker.transform.position : _Player.CommanderPosition.TileObject.transform.position;
		Quaternion newRotation = (posMarker != null) ? posMarker.transform.rotation : _Player.CommanderPosition.TileObject.transform.rotation;
		newPosition.y = _Player.CommanderPosition.Height;
        this.transform.position = newPosition;
		this.transform.rotation = newRotation;
		
        _lerpPosition.StopLerp();
    }

    public HashSet<TileData> GetReachableTiles()
    {
        return _reachableTiles;
    }

    public void DisplayInfo()
    {
        //update the hand ui 
        _HandUI.SetHand(_Player.Hand);
        //should update the army ui here as well I guess
    }

    GameObject getCommanderMarker(TileHolder tHolder)
    {
		if (tHolder == null) {
			return null;
		}

        switch (_Player.Type)
        {
            case PlayerType.None:
                Debug.LogError("Cannot get marker for player type None");
                break;
            case PlayerType.Battlebeard:
                return tHolder._MarkerCommanderBB;
            case PlayerType.Stormshaper:
                return tHolder._MarkerCommanderSS;
            default:
                Debug.LogError("Unhandled player type getting position marker");
                break;
        }

        return null;
    }
}
