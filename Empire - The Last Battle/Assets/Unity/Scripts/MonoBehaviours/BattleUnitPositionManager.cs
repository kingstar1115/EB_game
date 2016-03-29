using UnityEngine;
using System.Collections.Generic;

public class BattleUnitPositionManager : MonoBehaviour {

    //events
    public delegate void CardAction(CardData card);
    public event CardAction OnPlayerUseCard = delegate { };

    public event System.Action OnPause = delegate { };
    public event System.Action OnUnPause = delegate { };

    public BattleManager _BattleManager;

    public List<GameObject> _BattlebeardUnits;
    public List<GameObject> _StormshaperUnits;
    public List<GameObject> _Monsters;
    public List<Sprite> _PlayerLogos;

    public GameObject _MarkerActivePlayerUnit;
    public GameObject [] _MarkerActivePlayerUnits;
    public GameObject _MarkerActiveOpposition;
    public GameObject _MarkerReserveOppositionA;
    public GameObject _MarkerReserveOppositionB;
    public GameObject _MarkerReserveOppositionC;

    public GameObject _InstigatorPlayerUnit;
	public BattleUnitUI _InstigatorPlayerUnitUI;
    public GameObject [] _ActivePlayerUnits = new GameObject[8];
	public BattleUnitUI [] _ActivePlayerUnitUIs = new BattleUnitUI[8];
    public GameObject _ActiveOpposition;
    public BattleUnitUI _ActiveOppositionUI;
    public GameObject _ReserveOppositionA;
    public BattleUnitUI _ReserveOppositionAUI;
    public GameObject _ReserveOppositionB;
    public BattleUnitUI _ReserveOppositionBUI;
    public GameObject _ReserveOppositionC;
    public BattleUnitUI _ReserveOppositionCUI;

    //non unit ui stuff	
	public Camera _WorldCamera;
	public Camera _InstigatorCamera;
	public Camera _OppositionCamera;
	Camera activeCamera;
	public Canvas _Canvas;
    public GameObject _PauseScreen;
    public CardDisplayUI _CardDisplayUI;
    public HandUI _HandUI;
    public ArmyUI _ArmyUI;
    public BattleButtonsUI _ButtonsUI;
	public Pool _UnitUIPool;
	public Pool _HealthBarPool;

	Resolution res;

    // Use this for initialization
    void Start() {

    }

    bool _enabled;
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
                Hide();
            }
            else
            {
                _PauseScreen.SetActive(false);
                Enable();
                Show();
            }

            _paused = value;
        }
    }

    // Use this for initialization
    public void Initialise(Player battlebeard, Player stormshaper)
    {
		res = Screen.currentResolution;

        _ArmyUI.Initialise(battlebeard, stormshaper);

        _CardDisplayUI.Init();

		changeCamera(_WorldCamera);

		//int button ui and player image
        _ButtonsUI.Init();
        //_ButtonsUI._PlayerImage.sprite = _PlayerLogos[(int)activePlayer.Type];
		
        //add event listeners
        Enable();
    }

    public void Disable()
    {
        if (!_enabled)
        {
            return;
        }
        //remove event listeners
        _CardDisplayUI.OnCardUse -= _CardDisplayUI_OnCardUse;
        _CardDisplayUI.Hide();

        //disable components
        _ArmyUI.Disable();
        _HandUI._Enabled = false;
        _ButtonsUI._Enabled = false;
        _enabled = false;
    }

    public void Enable()
    {
        if (_enabled)
        {
            return;
        }
        //add event listeners		
        _CardDisplayUI.OnCardUse += _CardDisplayUI_OnCardUse;

		//enable components
		_ArmyUI.Enable();
        _HandUI._Enabled = true;
        _ButtonsUI._Enabled = true;
        _enabled = true;
    }

    public void RemoveListeners()
    {
        OnPlayerUseCard = delegate { };
        OnPause = delegate { };
        OnUnPause = delegate { };
        _ArmyUI.RemoveListeners();
        _CardDisplayUI.RemoveListeners();
        _HandUI.RemoveListeners();
		TutorialPanel.RemoveListeners();
		ModalPanel.RemoveListeners();

	}

    public void Hide()
    {
        _ArmyUI.Hide();
        _HandUI.Hide();
        _ButtonsUI.Hide();

        //show the card ui if there is a selected card
        if (_HandUI.m_SelectedCardUI != null)
            _CardDisplayUI.Show();
    }

    public void Show()
    {
        _ArmyUI.Show();
        _HandUI.Show();
        _ButtonsUI.Show();
    }

    public void ShowUnitSelectionUI(UnitSelection flags)
    {
        _ArmyUI.Show();
        _ArmyUI.MakeSelectable(flags);
        Disable();
    }

    public void HideUnitSelectionUI()
    {
        _ArmyUI.MakeUnselectable();
        Enable();
    }

    public void SetPlayer(Player p)
    {
        SwitchFocus(p.Type);
    }

    public void _CardDisplayUI_OnCardUse(CardData cardData)
    {
        Debug.Log("Use Card");
        OnPlayerUseCard(cardData);
        _CardDisplayUI.Hide();
    }

    public void InitUnitUI(UnitType t, PlayerType p) {
        //set up for check if already initialized
        GameObject marker = _MarkerActivePlayerUnits[(int)t];
        GameObject unitObject = _ActivePlayerUnits[(int)t];

        if (unitObject != null)
        {

            //set the unit object active 
            unitObject.SetActive(true);

            //grab the unit ui object and set it up
            GameObject unitUIObject = _ActivePlayerUnitUIs[(int)t].gameObject;
			unitUIObject.SetActive(true);
            awakenUnitUI(unitUIObject, marker, unitObject);

            return;
        }

		//initialize new uis if needed 
		List<GameObject> prefabs = p == PlayerType.Battlebeard ? _BattlebeardUnits : _StormshaperUnits;

		//models
        _ActivePlayerUnits[(int)t] = (GameObject)Instantiate(prefabs[(int)t], marker.transform.position, marker.transform.rotation);

		//battle unit ui 
		GameObject unitUI = _UnitUIPool.GetPooledObject ();
        awakenUnitUI(unitUI, marker, _ActivePlayerUnits[(int)t]);
		_ActivePlayerUnitUIs [(int)t] = unitUI.GetComponent<BattleUnitUI> ();
    }

	void awakenUnitUI(GameObject unitUI, GameObject marker, GameObject unitModelPrefab)
	{
		//set position
		unitUI.transform.SetParent (_Canvas.transform);
		repositionUnitUI(unitUI, marker, unitModelPrefab);
		
		//init
		unitUI.SetActive (true);
		unitUI.GetComponent<BattleUnitUI> ().Init (_HealthBarPool); 
	}

	void repositionUnitUI(GameObject unitUI, GameObject marker, GameObject unitModelPrefab) {
		Vector3 desiredUIWorldPosition = new Vector3(marker.transform.position.x,
		  marker.transform.position.y + unitModelPrefab.GetComponentInChildren<Renderer>().bounds.extents.y,
		  marker.transform.position.z);

		Vector2 viewportPos = activeCamera.WorldToViewportPoint(desiredUIWorldPosition);
		RectTransform transform = unitUI.GetComponent<RectTransform>();
		transform.anchorMin = viewportPos;
		transform.anchorMax = viewportPos;
		transform.anchoredPosition = new Vector2(0, 0);
		transform.localScale = new Vector3(1, 1, 1);
	}

	void repositionUIs() {
		if(_InstigatorPlayerUnit != null) {
			repositionUnitUI(_InstigatorPlayerUnitUI.gameObject, _MarkerActivePlayerUnit, _InstigatorPlayerUnit);
		}
		else {
			for(int i = 0; i < _ActivePlayerUnitUIs.Length; i++) {
				if(_ActivePlayerUnitUIs[i] != null) {
					repositionUnitUI(_ActivePlayerUnitUIs[i].gameObject, _MarkerActivePlayerUnits[i], _ActivePlayerUnits[i]);
				}			
			}
		}
		if(_ActiveOpposition != null) {
			repositionUnitUI(_ActiveOppositionUI.gameObject, _MarkerActiveOpposition, _ActiveOpposition);
		}
		if(_ReserveOppositionA != null) {
			repositionUnitUI(_ReserveOppositionAUI.gameObject, _MarkerReserveOppositionA, _InstigatorPlayerUnit);
		}
		if(_ReserveOppositionB != null) {
			repositionUnitUI(_ReserveOppositionBUI.gameObject, _MarkerReserveOppositionB, _ReserveOppositionB);
		}
		if(_ReserveOppositionC != null) {
			repositionUnitUI(_ReserveOppositionCUI.gameObject, _MarkerReserveOppositionC, _ReserveOppositionC);
		}

	}

	void changeCamera(Camera c) {
		if (activeCamera != null) {
			activeCamera.gameObject.SetActive(false);
		}
		activeCamera = c;
		activeCamera.gameObject.SetActive(true);
		repositionUIs();
	}

public void AddUnitToUI(Unit unit) 
	{
		//get the apropriate unit ui and add a unit to track
		_ActivePlayerUnitUIs [(int)unit.Type].AddUnit (unit);
	}

    public void RemoveUnit(UnitType t) {
        _ActivePlayerUnits[(int)t].SetActive(false);
		_ActivePlayerUnitUIs [(int)t].gameObject.SetActive (false);
    }

	public void RemoveInstigatorPlayer()
	{
		_InstigatorPlayerUnit.SetActive (false);
		_InstigatorPlayerUnitUI.gameObject.SetActive (false);
	}

    public void SetActiveUnit(Unit u, PlayerType p) {
		List<GameObject> prefabs = p == PlayerType.Battlebeard ? _BattlebeardUnits : _StormshaperUnits;
		_InstigatorPlayerUnit = (GameObject)Instantiate(prefabs[(int)u.Type], _MarkerActivePlayerUnit.transform.position, _MarkerActivePlayerUnit.transform.rotation);

		//battle unit ui 
		GameObject unitUI = _UnitUIPool.GetPooledObject();
		awakenUnitUI(unitUI, _MarkerActivePlayerUnit, _InstigatorPlayerUnit);
		_InstigatorPlayerUnitUI = unitUI.GetComponent<BattleUnitUI>();
		_InstigatorPlayerUnitUI.AddUnit(u);
	}

	public void SetOpposition(UnitType t, PlayerType p, iBattleable unit) {
		List<GameObject> prefabs = p == PlayerType.Battlebeard ? _BattlebeardUnits : _StormshaperUnits;
        setOpposition(prefabs[(int)t], unit);
	}

	public void SetOpposition(MonsterType t, iBattleable monster) {
        setOpposition(_Monsters[(int)t], monster);
	}

	void setOpposition(GameObject g, iBattleable monster) {
	    _ActiveOpposition = (GameObject)Instantiate(g, _MarkerActiveOpposition.transform.position, _MarkerActiveOpposition.transform.rotation);

        //battle unit ui 
        GameObject unitUI = _UnitUIPool.GetPooledObject();
        awakenUnitUI(unitUI, _MarkerActiveOpposition, _ActiveOpposition);
        _ActiveOppositionUI = unitUI.GetComponent<BattleUnitUI>();
        _ActiveOppositionUI.AddUnit(monster);
	}

	public void RemoveOpposition() {
		_ActiveOpposition.SetActive(false);
		_ActiveOppositionUI.gameObject.SetActive (false);
	}

	public void SetReserveAAsOpposition() {
		SetReserveAsOpposition(_ReserveOppositionA);
	}

	public void SetReserveBAsOpposition() {
		SetReserveAsOpposition(_ReserveOppositionB);
	}

	public void SetReserveCAsOpposition() {
		SetReserveAsOpposition(_ReserveOppositionC);
	}

	public void SetReserveAsOpposition(GameObject reserve) {
		_ActiveOpposition = reserve;
		reserve = null;
		_ActiveOpposition.transform.position = _MarkerActiveOpposition.transform.position;
		_ActiveOpposition.transform.rotation = _MarkerActiveOpposition.transform.rotation;
		_ActiveOpposition.SetActive(true);
	}

    public void SetReserveOppositionA(MonsterType t, iBattleable monster)
    {
        GameObject g = _Monsters[(int)t];
        _ReserveOppositionA = (GameObject)Instantiate(g, _MarkerReserveOppositionA.transform.position, _MarkerReserveOppositionA.transform.rotation);

        //battle unit ui 
        GameObject unitUI = _UnitUIPool.GetPooledObject();
        awakenUnitUI(unitUI, _MarkerReserveOppositionA, _ReserveOppositionA);
        _ReserveOppositionAUI = unitUI.GetComponent<BattleUnitUI>();
        _ReserveOppositionAUI.AddUnit(monster);
    }

    public void SetReserveOppositionB(MonsterType t, iBattleable monster)
    {
        GameObject g = _Monsters[(int)t];
        _ReserveOppositionB = (GameObject)Instantiate(g, _MarkerReserveOppositionB.transform.position, _MarkerReserveOppositionB.transform.rotation);

        //battle unit ui 
        GameObject unitUI = _UnitUIPool.GetPooledObject();
        awakenUnitUI(unitUI, _MarkerReserveOppositionB, _ReserveOppositionB);
        _ReserveOppositionBUI = unitUI.GetComponent<BattleUnitUI>();
        _ReserveOppositionBUI.AddUnit(monster);
    }

    public void SetReserveOppositionC(MonsterType t, iBattleable monster)
    {
        GameObject g = _Monsters[(int)t];
        _ReserveOppositionC = (GameObject)Instantiate(g, _MarkerReserveOppositionC.transform.position, _MarkerReserveOppositionC.transform.rotation);

        //battle unit ui 
        GameObject unitUI = _UnitUIPool.GetPooledObject();
        awakenUnitUI(unitUI, _MarkerReserveOppositionC, _ReserveOppositionC);
        _ReserveOppositionCUI = unitUI.GetComponent<BattleUnitUI>();
        _ReserveOppositionCUI.AddUnit(monster);
    }

    public void SwitchFocus(PlayerType pType){

		Camera c = _BattleManager.activePlayer == BattlerType.Instigator ? _InstigatorCamera : _OppositionCamera;
		changeCamera(c);

		_ArmyUI.SwitchPlayer (_BattleManager.GetActivePlayer().Type);
        _HandUI.SetHand(_BattleManager.GetActivePlayer().Hand);
        _ButtonsUI._Enabled = true;
        _ButtonsUI.Show();

        //set logo as well
        _ButtonsUI._PlayerImage.sprite = _PlayerLogos[(int)pType];
	}

	public void PauseScreenClickHandler()
	{
		OnUnPause();
	}

	public void AddPlayerCard(PlayerType pType, CardData cData)
	{
		//update the hand ui here
		Player player = _BattleManager.GetActivePlayer ();
		if (player.Type == pType) 
		{
			_HandUI.SetHand(player.Hand);
		}
	}
	
	public void RemovePlayerCard(PlayerType pType, CardData cData, int index)
	{
		//update the hand ui here
		Player player = _BattleManager.GetActivePlayer ();
		if (player.Type == pType) 
		{
			//deselect card
			_HandUI.DeselectCardNoPopdown(index);
			_HandUI.SetHand(player.Hand);
		}
	}

	void Update()
	{
		if(!res.Equals(Screen.currentResolution)) {

			repositionUIs();

			res = Screen.currentResolution;
		}

		//check for pause switch by key 
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (_Paused) {
				OnUnPause();
			}
			else {
				OnPause();
			}
		}
	}
}
