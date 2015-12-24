using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class ArmyUI : MonoBehaviour {
	
	public GameObject BattlebeardUI;
	public GameObject StormshaperUI;
	GameObject currentUI;

	public float MaxWidth = 820;
	public float MinWidth = 110;
	public float SmallIconSize = 70f;
	public float UnitSpacing = 10;
	public float LargeIconSize = 100f;

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

	int previousSelection = 0;
	int currentSelection = -1;

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
		// test
		SwitchPlayer(PlayerType.Battlebeard);

	}

	private void unitUpdated(Player p, Unit u) {
		List<UnitTypeUI> data = getUnitTypeUI(p.Type);
		UnitTypeUI unitUI = data.Find(ui => (ui.GetType() == u.Type));
		if (unitUI != null) {
			int i = p.PlayerArmy.GetUnits().IndexOf(u);
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
		//if (p != currentPlayer) { (for now)
			if (currentUI) {
				resetSelection();
				currentUI.SetActive(false);
			}
			currentUI = p == PlayerType.Battlebeard ? BattlebeardUI : StormshaperUI;
			currentUI.SetActive(true);
			currentPlayer = p;

		//}
	}

	void resetSelection() {
		setSelection(-1);
		previousSelection = 0;
	}


	public void GenerateUI(Player p) {

		GameObject thisUI = p.Type == PlayerType.Battlebeard ? BattlebeardUI : StormshaperUI;

		List<UnitTypeUI> data = getUnitTypeUI(p.Type);

		data.ForEach(ui => ui.Reset()); // maybe not necesarry?

		List<GameObject> children = new List<GameObject>();
		foreach (Transform child in thisUI.transform) children.Add(child.gameObject);
		children.ForEach(child => Destroy(child));

		data.Clear();

		List<Unit> allUnits = p.PlayerArmy.GetUnits();
		List<UnitType> unitTypes = new List<UnitType>();
		allUnits.ForEach(unit => {
			if (!unitTypes.Contains(unit.Type)) {
				unitTypes.Add(unit.Type);
			}
		});
		
		unitTypes.ForEach(t => {
			UnitTypeUI unitTypeUI = createUIForUnitType(thisUI, t, data);
			p.PlayerArmy.GetUnits(t).ForEach(u => unitTypeUI.AddUnit(u));
		});
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
		return unitUI;
	}

	void showSubmenu(int i) {
		getUnitTypeUI(currentPlayer)[i].Maximise();
	}

	void hideSubmenu(int i) {
		getUnitTypeUI(currentPlayer)[i].Minimise();
	}


	void Update() {
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
			} else {
				// something is selected
				if (leftDown) {
					//hide all items;
					setSelection(-1);
				} else {
					int count = getUnitTypeUI(currentPlayer).Count;
					if (upDown) {
						// select the previous menu
						setSelection((currentSelection - 1 + count) % count);
					} else if (downDown) {
						//select the next menu
						setSelection((currentSelection + 1) % count);
					}
				}
			}
		}
		
	}

	void OnGUI() {
		// listen for mouse move
		if (Input.GetAxis("Mouse X") != lastMousePos.x || Input.GetAxis("Mouse Y") != lastMousePos.y) {
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
