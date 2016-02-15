using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public delegate void UIPlayerUnitTypeIndexCallback(PlayerType p, UnitType t, int i);
public class ArmyUI : MonoBehaviour {
	
	public GameObject BattlebeardUI;
	public GameObject StormshaperUI;
	GameObject currentUI;

	public Sprite ScoutSprite;
	public Sprite PikemanSprite;
	public Sprite AxeThrowerSprite;
	public Sprite WarriorSprite;
	public Sprite ArcherSprite;
	public Sprite CavalrySprite;
	public Sprite BallistaSprite;
	public Sprite CatapultSprite;
	public GameObject UnitPrefab;
	public GameObject UnitsPrefab;

	PlayerType currentPlayer;
	List<UnitTypeUI> battlebeardUIData;
	List<UnitTypeUI> stormshaperUIData;
	CompareUnitUI comparator;
	Vector2 lastMousePos;

	public UIPlayerUnitTypeIndexCallback OnClickUnit = delegate { };
	int previousSelection = 0;
	int currentSelection = -1;

	new bool enabled = false;

	public void RemoveListeners() {
		OnClickUnit = delegate { };
		foreach(UnitTypeUI u in battlebeardUIData) {
			u.RemoveListeners();
		}
		foreach (UnitTypeUI u in stormshaperUIData) {
			u.RemoveListeners();
		}
	}

	void setSelection(int i) {
		if (i == currentSelection || getUnitTypeUI(currentPlayer) == null) {
			return;
		} else if (i == -1) {
			hideSubmenu(currentSelection);
		} else {
			if (currentSelection != -1) {
				hideSubmenu(currentSelection);
			}
			showSubmenu(i);
		}
		previousSelection = currentSelection;
		currentSelection = i;
	}

	List<UnitTypeUI> getUnitTypeUI(PlayerType p) {
		if (p == PlayerType.Battlebeard) return battlebeardUIData;
		if (p == PlayerType.Stormshaper) return stormshaperUIData;
		return null;
	}

	// Use this for initialization
	public void Initialise (Player battlebeard, Player stormshaper) {
		lastMousePos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		comparator = new CompareUnitUI();
		battlebeardUIData = new List<UnitTypeUI>();
		stormshaperUIData = new List<UnitTypeUI>();

		BattlebeardUI.SetActive(false);
		StormshaperUI.SetActive(false);
		if (battlebeard != null) {
			GenerateUI(battlebeard);
			battlebeard.OnAddUnit += unitAdded;
			battlebeard.OnRemoveUnit += unitRemoved;
			battlebeard.OnUpdateUnit += unitUpdated;
		}

		if (stormshaper != null) {
			GenerateUI(stormshaper);
			stormshaper.OnAddUnit += unitAdded;
			stormshaper.OnRemoveUnit += unitRemoved;
			stormshaper.OnUpdateUnit += unitUpdated;
		}
		enabled = true;
	}

	public void Enable() {
		enabled = true;	
	}

	public void Disable() {
		enabled = false;
	}

	public void MakeSelectable(UnitSelection flags) {
		Disable();
		getUnitTypeUI(currentPlayer).ForEach(ui => ui.MakeSelectable(flags));
		getUnitTypeUI(currentPlayer).ForEach(ui => ui.Maximise());
		resetSelection();
	}

	public void MakeUnselectable() {
		Enable();
		getUnitTypeUI(currentPlayer).ForEach(ui => ui.MakeUnselectable());
		getUnitTypeUI(currentPlayer).ForEach(ui => ui.Minimise());
	}

	public void Show() {
		getUnitTypeUI(currentPlayer).ForEach(ui => ui.Show());
	}

	public void Hide() {
		getUnitTypeUI(currentPlayer).ForEach(ui => ui.Hide());
		resetSelection();
	}

	void unitUpdated(Player p, Unit u) {
		List<UnitTypeUI> data = getUnitTypeUI(p.Type);
		UnitTypeUI unitUI = data.Find(ui => (ui.GetType() == u.Type));
		if (unitUI != null) {
			int i = p.PlayerArmy.GetUnits(u.Type).IndexOf(u);
			unitUI.UpdateUnit(i, u);
		}
	}

	private void unitRemoved(Player p, Unit u, int i) {
		List<UnitTypeUI> data = getUnitTypeUI(p.Type);
		UnitTypeUI unitUI = data.Find(ui => (ui.GetType() == u.Type));

		if (unitUI != null) {
			//i = p.PlayerArmy.GetAllUnits().IndexOf(u);
			unitUI.RemoveUnit(i);
		}
	}

	private void unitAdded(Player p, Unit u) {
		List<UnitTypeUI> data = getUnitTypeUI(p.Type);
		UnitTypeUI unitUI = null;
		data.ForEach(ui => {
			if (ui.GetType() == u.Type) {
				unitUI = ui;
			}
		});

		if (unitUI == null) {
			GameObject thisUI = p.Type == PlayerType.Battlebeard ? BattlebeardUI : StormshaperUI;
			unitUI = createUIForUnitType(thisUI, u.Type, data);
		}
		unitUI.AddUnit(u);
	}

	public void SwitchPlayer(PlayerType p) {
		if (p != currentPlayer) {
			if (currentUI) {
				resetSelection();
				currentUI.SetActive(false);
			}
			currentUI = p == PlayerType.Battlebeard ? BattlebeardUI : StormshaperUI;
			currentUI.SetActive(true);
			currentPlayer = p;

		}
	}

	void resetSelection() {
		setSelection(-1);
		previousSelection = 0;
	}


	void GenerateUI(Player p) {
		GameObject thisUI = p.Type == PlayerType.Battlebeard ? BattlebeardUI : StormshaperUI;
		List<UnitTypeUI> data = getUnitTypeUI(p.Type);
		data.ForEach(ui => ui.Reset()); // maybe not necesarry?
		List<GameObject> children = new List<GameObject>();
		foreach (Transform child in thisUI.transform) children.Add(child.gameObject);
		children.ForEach(child => Destroy(child));

		data.Clear();
		List<Unit> allUnits = p.PlayerArmy.GetUnits();
		List<UnitType> unitTypes = new List<UnitType>();
		foreach (Unit unit in allUnits) {
			if (!unitTypes.Contains(unit.Type)) {
				unitTypes.Add(unit.Type);
			}
		}
		foreach (UnitType t in unitTypes) {
			UnitTypeUI unitTypeUI = createUIForUnitType(thisUI, t, data);
			p.PlayerArmy.GetUnits(t).ForEach(u => unitTypeUI.AddUnit(u));
		}

	}

	UnitTypeUI createUIForUnitType(GameObject UI, UnitType t, List<UnitTypeUI> data) {
		GameObject o = Instantiate(UnitsPrefab);
		UnitTypeUI unitUI = o.GetComponent<UnitTypeUI>();
		unitUI.Initialise(t);
		int index = ~data.BinarySearch(unitUI, comparator);
		o.transform.SetParent(UI.transform);
		// Make sure they show in order in the UI
		o.transform.SetSiblingIndex(index);
		o.transform.localScale = Vector3.one;
		data.Insert(index, unitUI);
		unitUI.OnClickUnit += _clickUnit;
		unitUI.Show();
		return unitUI;
	}

	void showSubmenu(int i) {
		getUnitTypeUI(currentPlayer)[i].Maximise();
	}

	void hideSubmenu(int i) {
		getUnitTypeUI(currentPlayer)[i].Minimise();
	}

	void _clickUnit(UnitType u, int i) {
		OnClickUnit(currentPlayer, u, i);
	}

	void Update() {

		// If there is anything in the UI
		if (enabled && getUnitTypeUI(currentPlayer).Count > 0) {
			bool leftDown = Input.GetKeyDown(KeyCode.LeftArrow);
			bool rightDown = Input.GetKeyDown(KeyCode.RightArrow);
			bool upDown = Input.GetKeyDown(KeyCode.UpArrow);
			bool downDown = Input.GetKeyDown(KeyCode.DownArrow);

			if (leftDown || rightDown || upDown || downDown) {
				if (currentSelection == -1) {
					// nothing is currently selected
					if (rightDown) {
						//show previous selected item in the list
						setSelection(previousSelection);
					}
				}
				else {
					// something is selected
					if (leftDown) {
						//hide all items;
						setSelection(-1);
					}
					else {
						int count = getUnitTypeUI(currentPlayer).Count;
						if (upDown) {
							// select the previous menu
							setSelection((currentSelection - 1 + count) % count);
						}
						else if (downDown) {
							//select the next menu
							setSelection((currentSelection + 1) % count);
						}
					}
				}
			}
		}
	}

	void OnGUI() {
		// listen for mouse move
		if (enabled && (Input.GetAxis("Mouse X") != lastMousePos.x || Input.GetAxis("Mouse Y") != lastMousePos.y)) {
			List<UnitTypeUI> data = getUnitTypeUI(currentPlayer);
			for (int i = 0; i < data.Count; i++) {
				if (data[i].IsMouseOver()) {
					// show menu at i
					setSelection(i);
				}
				else if (currentSelection == i) {
					// hide all menus
					setSelection(-1);
				}
			}
		}
		lastMousePos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
	}
}
