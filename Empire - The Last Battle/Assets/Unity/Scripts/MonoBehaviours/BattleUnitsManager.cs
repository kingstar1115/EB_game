using UnityEngine;
using System.Collections.Generic;

//basicaly ui 
public class BattleUnitsManager : MonoBehaviour {

	//events
	public delegate void CardAction(CardData card);
	public event CardAction OnPlayerUseCard = delegate { };
	
	public event System.Action OnPause = delegate { };
	public event System.Action OnUnPause = delegate { };

	public BattleManager _BattleManager;

	public List<GameObject> _BattlebeardUnits;
	public List<GameObject> _StormshaperUnits;
	public List<GameObject> _Monsters;
	public List<GameObject> _LostImmortals;

	public GameObject _MarkerActivePlayerUnit;
	public GameObject _MarkerActivePlayerScouts;
	public GameObject _MarkerActivePlayerPikemen;
	public GameObject _MarkerActivePlayerAxeThrowers;
	public GameObject _MarkerActivePlayerArchers;
	public GameObject _MarkerActivePlayerCavalry;
	public GameObject _MarkerActivePlayerWarriors;
	public GameObject _MarkerActivePlayerCatapult;
	public GameObject _MarkerActivePlayerBallista;
	public GameObject _MarkerActiveOpposition;
	public GameObject _MarkerReserveOppositionA;
	public GameObject _MarkerReserveOppositionB;

	//non unit ui stuff	
	public GameObject _PauseScreen;
	public CardDisplayUI _CardDisplayUI;
	public HandUI _HandUI;
	public ArmyUI _ArmyUI;

	// Use this for initialization
	void Start () {
	
	}
	


	// Update is called once per frame
	void Update () {
	
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
				Hide ();
			}
			else
			{
				_PauseScreen.SetActive(false);
				Enable();
				Show ();
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
		if (!_enabled) {
			return;
		}
		//remove event listeners
		_CardDisplayUI.OnCardUse -= _CardDisplayUI_OnCardUse;
		_CardDisplayUI.Hide();
		_HandUI._Enabled = false;
		
		//disable components
		_ArmyUI.Disable ();
		_enabled = false;
	}
	
	public void Enable() {
		if (_enabled) {
			return;
		}
		//add event listeners		
		_CardDisplayUI.OnCardUse += _CardDisplayUI_OnCardUse;
		
		_HandUI._Enabled = true;
		
		//enable components
		_ArmyUI.Enable();
		_enabled = true;
	}
	
	public void RemoveListeners() {
		OnPlayerUseCard = delegate { };
		OnPause = delegate { };
		OnUnPause = delegate { };
		_ArmyUI.RemoveListeners();
		_CardDisplayUI.RemoveListeners();
		_HandUI.RemoveListeners();
		
	}
	
	public void Hide() {
		_ArmyUI.Hide();
		_HandUI.Hide();
		//show the card ui if there is a selected card
		if (_HandUI.m_SelectedCardUI != null)
			_CardDisplayUI.Show();
	}
	
	public void Show() {
		_ArmyUI.Show();
		_HandUI.Show();
	}
	
	public void ShowUnitSelectionUI(UnitSelection flags) {
		_ArmyUI.Show();
		_ArmyUI.MakeSelectable(flags);
		Disable();
	}
	
	public void HideUnitSelectionUI() {
		_ArmyUI.MakeUnselectable();
		Enable();
	}
	
	public void SetPlayer(Player p) {
		SwitchFocus (p.Type);
	}
	
	public void _CardDisplayUI_OnCardUse(CardData cardData)
	{
		Debug.Log("Use Card");
		OnPlayerUseCard(cardData);
		_CardDisplayUI.Hide();
	}

	public void SetUnit(UnitType t, PlayerType p) {
		List<GameObject> prefabs = p == PlayerType.Battlebeard ? _BattlebeardUnits : _StormshaperUnits;
		GameObject marker;
		switch (t) {
			case UnitType.Archer:
				marker = _MarkerActivePlayerArchers;
				break;
			case UnitType.AxeThrower:
				marker = _MarkerActivePlayerAxeThrowers;
				break;
			case UnitType.Ballista:
				marker = _MarkerActivePlayerBallista;
				break;
			case UnitType.Catapult:
				marker = _MarkerActivePlayerCatapult;
				break;
			case UnitType.Cavalry:
				marker = _MarkerActivePlayerCavalry;
				break;
			case UnitType.Pikeman:
				marker = _MarkerActivePlayerPikemen;
				break;
			case UnitType.Scout:
				marker = _MarkerActivePlayerScouts;
				break;
			case UnitType.Warrior:
				marker = _MarkerActivePlayerWarriors;
				break;
			default:
				marker = _MarkerActivePlayerScouts;
				break;
		}
		// should probably save them sometime
		Instantiate(prefabs[(int)t], marker.transform.position, marker.transform.rotation);
	}

	public void SetActiveUnit(UnitType t, PlayerType p) {
		List<GameObject> prefabs = p == PlayerType.Battlebeard ? _BattlebeardUnits : _StormshaperUnits;
		Instantiate(prefabs[(int)t], _MarkerActivePlayerUnit.transform.position, _MarkerActivePlayerUnit.transform.rotation);
	}

	public void SetOpposition(UnitType t, PlayerType p) {
		List<GameObject> prefabs = p == PlayerType.Battlebeard ? _BattlebeardUnits : _StormshaperUnits;
		setOpposition(prefabs[(int)t]);
	}

	public void SetOpposition(LostImmortal t) {
		setOpposition(_LostImmortals[(int)t]);
	}

	public void SetOpposition(Monster t) {
		setOpposition(_Monsters[(int)t]);
	}

	void setOpposition(GameObject g) {
		Instantiate(g, _MarkerActiveOpposition.transform.position, _MarkerActiveOpposition.transform.rotation);
	}

	public void SetReserveOppositionA(Monster t) {

	}

	public void SetReserveOppositionB(Monster t) {

	}

	public void SwitchFocus(PlayerType pType){
		_ArmyUI.SwitchPlayer (pType);
	}

	public void PauseScreenClickHandler()
	{
		OnUnPause();
	}

	public void AddPlayerCard(PlayerType pType, CardData cData)
	{
		//update the hand ui here
		if (_BattleManager. == pType) 
		{
			_CommanderUI.DisplayInfo();
		}
	}
	
	public void RemovePlayerCard(PlayerType pType, CardData cData, int index)
	{
		//update the hand ui here
		if (_CommanderUI._Player.Type == pType) 
		{
			//deselect card
			_HandUI.DeselectCardNoPopdown(index);
			_CommanderUI.DisplayInfo();
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


public enum Monster {
	Minotaur,
	Ogre,
	Sasquatch
}

public enum LostImmortal {
	bb1,
	bb2,
	bb3,
	bb4,
	ss1,
	ss2,
	ss3,
	ss4
}