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

    public GameObject _MarkerActivePlayerUnit;
    public GameObject [] _MarkerActivePlayerUnits;
    public GameObject _MarkerActiveOpposition;
    public GameObject _MarkerReserveOppositionA;
    public GameObject _MarkerReserveOppositionB;
    public GameObject _MarkerReserveOppositionC;

    public GameObject _ActivePlayerUnit;
    public GameObject [] _ActivePlayerUnits = new GameObject[8];
    public GameObject _ActiveOpposition;
    public GameObject _ReserveOppositionA;
    public GameObject _ReserveOppositionB;
    public GameObject _ReserveOppositionC;

    //non unit ui stuff	
    public GameObject _PauseScreen;
    public CardDisplayUI _CardDisplayUI;
    public HandUI _HandUI;
    public ArmyUI _ArmyUI;

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
        _ArmyUI.Initialise(battlebeard, stormshaper);

        _CardDisplayUI.Init();

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

    public void SetUnit(UnitType t, PlayerType p) {
        List<GameObject> prefabs = p == PlayerType.Battlebeard ? _BattlebeardUnits : _StormshaperUnits;
        GameObject marker = _MarkerActivePlayerUnits[(int)t];
        GameObject unitObject = _ActivePlayerUnits[(int)t];
        if (unitObject != null) {
            unitObject.SetActive(true);
            return;
        }
        _ActivePlayerUnits[(int)t] = (GameObject)Instantiate(prefabs[(int)t], marker.transform.position, marker.transform.rotation);
    }

    public void RemoveUnit(UnitType t) {
        _ActivePlayerUnits[(int)t].SetActive(false);
    }

    public void SetActiveUnit(UnitType t, PlayerType p) {
		List<GameObject> prefabs = p == PlayerType.Battlebeard ? _BattlebeardUnits : _StormshaperUnits;
        _ActivePlayerUnit = (GameObject)Instantiate(prefabs[(int)t], _MarkerActivePlayerUnit.transform.position, _MarkerActivePlayerUnit.transform.rotation);
	}

	public void SetOpposition(UnitType t, PlayerType p) {
		List<GameObject> prefabs = p == PlayerType.Battlebeard ? _BattlebeardUnits : _StormshaperUnits;
		setOpposition(prefabs[(int)t]);
	}

	public void SetOpposition(MonsterType t) {
		setOpposition(_Monsters[(int)t]);
	}

	void setOpposition(GameObject g) {
	    _ActiveOpposition = (GameObject)Instantiate(g, _MarkerActiveOpposition.transform.position, _MarkerActiveOpposition.transform.rotation);
	}

	public void RemoveOpposition() {
		_ActiveOpposition.SetActive(false);
	}

	public void SetReserveOppositionA(MonsterType t) {
        GameObject g = _Monsters[(int)t];
        _ReserveOppositionA = (GameObject)Instantiate(g, _MarkerReserveOppositionA.transform.position, _MarkerReserveOppositionA.transform.rotation);
    }

	public void SetReserveOppositionB(MonsterType t) {
        GameObject g = _Monsters[(int)t];
        _ReserveOppositionB = (GameObject)Instantiate(g, _MarkerReserveOppositionB.transform.position, _MarkerReserveOppositionB.transform.rotation);
    }

    public void SetReserveOppositionC(MonsterType t) {
        GameObject g = _Monsters[(int)t];
        _ReserveOppositionC = (GameObject)Instantiate(g, _MarkerReserveOppositionB.transform.position, _MarkerReserveOppositionB.transform.rotation);
    }

    public void SwitchFocus(PlayerType pType){
		_ArmyUI.SwitchPlayer (pType);
        _HandUI.SetHand(_BattleManager.GetActivePlayer().Hand);
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


public enum MonsterType {
    Cyclops,
    Minotaur,
	Sasquatch,
    FireElemental,
    WaterElemental,
    EarthElemental,
    AirElemental,
    Wyrm,
    Wyvern,
    Dragon,
    Hydra,
    LostImmortalSs1,
    LostImmortalSs2,
    LostImmortalSs3,
    LostImmortalSs4,
    LostImmortalBb1,
    LostImmortalBb2,
    LostImmortalBb3,
    LostImmortalBb4
}