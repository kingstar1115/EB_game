using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OverworldUI : MonoBehaviour
{
    public delegate void BoardAction(TileData tile);
    public event BoardAction OnCommanderMove = delegate { };

    public event System.Action OnPause = delegate { };
    public event System.Action OnUnPause = delegate { };

    public bool _TileHover;
    public CommanderUI _CommanderUI;
	CommanderUI _battlebeardCommanderUI;
	CommanderUI _stormshaperCommanderUI;
    public CameraMovement _CameraMovement;
    public BoardUI _BoardUI;
	public ArmyUI _ArmyUI;
    public GameObject _PauseScreen;

    bool _paused;
    public bool _Paused
    {
        get { return _paused; }
        set
        {
            if (value)
            {
                _PauseScreen.SetActive(true);
                Disable();
            }
            else
            {
                _PauseScreen.SetActive(false);
                Enable();
            }

            _paused = value;
        }
    }

    // Use this for initialization
    public void Initialise(Player battlebeard, Player stormshaper)
    {
        _BoardUI.Init();
		_battlebeardCommanderUI = battlebeard.GetComponent<CommanderUI>();
		_stormshaperCommanderUI = stormshaper.GetComponent<CommanderUI>();
		_battlebeardCommanderUI.Initialise();
		_stormshaperCommanderUI.Initialise();

		//snap players to start position
		_battlebeardCommanderUI.UpdateToPlayerPosition();
		_stormshaperCommanderUI.UpdateToPlayerPosition();

		_ArmyUI.Initialise(battlebeard, stormshaper);

        //add event listeners
        Enable();
    }

    public void Disable()
    {
		//remove event listeners
		_battlebeardCommanderUI.OnCommanderMoved -= _CommanderUI_OnCommanderMoved;
		_stormshaperCommanderUI.OnCommanderMoved -= _CommanderUI_OnCommanderMoved;
		_battlebeardCommanderUI.OnStartDrag -= _CommanderUI_OnStartDrag;
		_stormshaperCommanderUI.OnStartDrag -= _CommanderUI_OnStartDrag;
		_battlebeardCommanderUI.OnCommanderDrop -= _CommanderUI_OnCommanderDrop;
		_stormshaperCommanderUI.OnCommanderDrop -= _CommanderUI_OnCommanderDrop;
		_battlebeardCommanderUI.OnCommanderGrounded -= _CommanderUI_Grounded;
		_stormshaperCommanderUI.OnCommanderGrounded -= _CommanderUI_Grounded;
		_battlebeardCommanderUI.OnDropCommander -= _CommanderUI_OnDropCommander;
		_stormshaperCommanderUI.OnDropCommander -= _CommanderUI_OnDropCommander;

		//disable components
		_battlebeardCommanderUI._Paused = true;
		_stormshaperCommanderUI._Paused = true;

		_CameraMovement.DisableCameraMovement();

		_ArmyUI.Disable();
    }

    public void Enable()
    {
		//add event listeners
		_battlebeardCommanderUI.OnCommanderMoved += _CommanderUI_OnCommanderMoved;
		_stormshaperCommanderUI.OnCommanderMoved += _CommanderUI_OnCommanderMoved;
		_battlebeardCommanderUI.OnStartDrag += _CommanderUI_OnStartDrag;
		_stormshaperCommanderUI.OnStartDrag += _CommanderUI_OnStartDrag;
		_battlebeardCommanderUI.OnCommanderDrop += _CommanderUI_OnCommanderDrop;
		_stormshaperCommanderUI.OnCommanderDrop += _CommanderUI_OnCommanderDrop;
		_battlebeardCommanderUI.OnCommanderGrounded += _CommanderUI_Grounded;
		_stormshaperCommanderUI.OnCommanderGrounded += _CommanderUI_Grounded;
		_battlebeardCommanderUI.OnDropCommander += _CommanderUI_OnDropCommander;
		_stormshaperCommanderUI.OnDropCommander += _CommanderUI_OnDropCommander;

		//enable components
		_CommanderUI._Paused = false;
		_CommanderUI._Paused = false;

		_CameraMovement.EnableCameraMovement();

		_ArmyUI.Enable();
    }

	public void SetPlayer(Player p) {
		_CommanderUI.DisablePlayerMovement();
		_CommanderUI = p.Type == PlayerType.Battlebeard ? _battlebeardCommanderUI : _stormshaperCommanderUI;
		switchFocus(_CommanderUI);
	}

	void _CommanderUI_OnDropCommander(TileData tile)
    {
        _BoardUI.PlayerPrompt_DefaultTiles();
    }

	void _CommanderUI_Grounded()
	{
		//if camera is not moving to a new position then enable it 
		if (!_CameraMovement.IsLerping ())
			_CameraMovement.EnableCameraMovement ();
	}

	void _CommanderUI_OnCommanderDrop(Vector3 vec)
	{
		_CameraMovement.EnableCameraMovement(vec);
	}

    void _CommanderUI_OnStartDrag()
    {
        _CameraMovement.DisableCameraMovement();
        _BoardUI.PlayerPrompt_MovableTiles(_CommanderUI.GetReachableTiles());
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

    public void PauseScreenClickHandler()
    {
        OnUnPause();
    }

	void switchFocus(CommanderUI u){
		//_CameraMovement._TargetObject = u._Player.transform;
		_CameraMovement.MoveToNewTarget(u._Player.transform, u.getPosition());
		_ArmyUI.SwitchPlayer(u._Player.Type);
	}

    void Update()
    {
        //check for pause switch by key 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_Paused)
                OnUnPause();
            else
                OnPause();
        }
    }
}
