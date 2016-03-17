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

    public GameObject _ActivePlayerUnit;
	public BattleUnitUI _ActivePlayerUnitUI;
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
    public Canvas _Canvas;
    public GameObject _PauseScreen;
    public CardDisplayUI _CardDisplayUI;
    public HandUI _HandUI;
    public ArmyUI _ArmyUI;
    public UnityEngine.UI.Image _PlayerLogo;
	public Pool _UnitUIPool;
	public Pool _HealthBarPool;

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
    public void Initialise(Player battlebeard, Player stormshaper, Player activePlayer)
    {
        _ArmyUI.Initialise(battlebeard, stormshaper);

        _CardDisplayUI.Init();

		//int the player logo
		_PlayerLogo.sprite = _PlayerLogos[(int)activePlayer.Type];

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
        _HandUI._Enabled = false;

        //disable components
        _ArmyUI.Disable();
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

        _HandUI._Enabled = true;

        //enable components
        _ArmyUI.Enable();
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

    }

    public void Hide()
    {
        _ArmyUI.Hide();
        _HandUI.Hide();
        //show the card ui if there is a selected card
        if (_HandUI.m_SelectedCardUI != null)
            _CardDisplayUI.Show();
    }

    public void Show()
    {
        _ArmyUI.Show();
        _HandUI.Show();
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
        Vector3 desiredUIWorldPosition = new Vector3(marker.transform.position.x, 
            marker.transform.position.y + unitModelPrefab.GetComponentInChildren<Renderer>().bounds.extents.y,
            marker.transform.position.z);

        Vector2 viewportPos = _WorldCamera.WorldToViewportPoint(desiredUIWorldPosition);
        RectTransform tansform = unitUI.GetComponent<RectTransform>();
        tansform.anchorMin = viewportPos;
        tansform.anchorMax = viewportPos;
        tansform.anchoredPosition = new Vector2(0, 0);
        tansform.localScale = new Vector3(1, 1, 1);
		
		//init
		unitUI.SetActive (true);
		unitUI.GetComponent<BattleUnitUI> ().Init (_HealthBarPool); 
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

    public void SetActiveUnit(Unit u, PlayerType p) {
		List<GameObject> prefabs = p == PlayerType.Battlebeard ? _BattlebeardUnits : _StormshaperUnits;
		_ActivePlayerUnit = (GameObject)Instantiate(prefabs[(int)u.Type], _MarkerActivePlayerUnit.transform.position, _MarkerActivePlayerUnit.transform.rotation);

		//battle unit ui 
		GameObject unitUI = _UnitUIPool.GetPooledObject();
		awakenUnitUI(unitUI, _MarkerActivePlayerUnit, _ActivePlayerUnit);
		_ActivePlayerUnitUI = unitUI.GetComponent<BattleUnitUI>();
		_ActivePlayerUnitUI.AddUnit(u);
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
		_ArmyUI.SwitchPlayer (pType);
        _HandUI.SetHand(_BattleManager.GetActivePlayer().Hand);

        //set logo as well
        _PlayerLogo.sprite = _PlayerLogos[(int)pType];
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
