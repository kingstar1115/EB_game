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
	Dictionary<PlayerType, List<UnitUIAll>> uiDataNew;
	// cached number of unit types
	int currentCount = 0;

	Vector2 lastMousePos;

	int previousSelection = 0;
	int currentSelection = -1;

	void setSelection(int i) {
		if (i == currentSelection || uiDataNew == null) {
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

	// Use this for initialization
	public void Initialise (Player battlebeard, Player stormshaper) {
		lastMousePos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		uiDataNew = new Dictionary<PlayerType, List<UnitUIAll>>();
		for (int i = 1; i <= 2; i++) {
			List<UnitUIAll> unitUI = new List<UnitUIAll>();
			uiDataNew.Add((PlayerType)i, unitUI);
		}

		BattlebeardUI.SetActive(false);
		StormshaperUI.SetActive(false);

		if (battlebeard != null) {
			GenerateUI(battlebeard);
		}

		if (stormshaper != null) {
			GenerateUI(stormshaper);
		}
		// test
		SwitchPlayer(PlayerType.Battlebeard);

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

			List<UnitUIAll> data;
			uiDataNew.TryGetValue(currentPlayer, out data);
			currentCount = data.Count;

		//}
	}

	void resetSelection() {
		setSelection(-1);
		previousSelection = 0;
	}


	public void GenerateUI(Player p) {

		GameObject thisUI = p.Type == PlayerType.Battlebeard ? BattlebeardUI : StormshaperUI;

		List<UnitUIAll> data;
		uiDataNew.TryGetValue(p.Type, out data);

		data.ForEach(ui => ui.Reset()); // maybe not necesarry?

		List<GameObject> children = new List<GameObject>();
		foreach (Transform child in thisUI.transform) children.Add(child.gameObject);
		children.ForEach(child => Destroy(child));

		data.Clear();

		List<Unit> allUnits = p.PlayerArmy.GetAllUnits();
		List<UnitType> unitTypes = new List<UnitType>();
		allUnits.ForEach(unit => {
			if (!unitTypes.Contains(unit.Type)) {
				unitTypes.Add(unit.Type);
			}
		});

		unitTypes.ForEach(t => {
			UnitUIAll unitUI = createUIForUnitType(thisUI, t);
			p.PlayerArmy.GetUnits(t).ForEach(u => unitUI.AddUnit(u));
			int index = (int)t <= data.Count ? (int)t : data.Count;
			data.Insert(index, unitUI);
		});
	}

	UnitUIAll createUIForUnitType(GameObject UI, UnitType t) {
		GameObject o = Instantiate(UnitsPrefab);
		o.transform.SetParent(UI.transform);
		// Make sure they show in order in the UI
		o.transform.SetSiblingIndex((int)t);
		o.transform.localScale = Vector3.one;
		UnitUIAll unitUI = o.GetComponent<UnitUIAll>();
		unitUI.Initialise(t);
		return unitUI;
	}

	public void AddUnit(Player p, Unit u) {
		List<UnitUIAll> data;
		UnitUIAll unitUI = null;
		uiDataNew.TryGetValue(p.Type, out data);
		data.ForEach(ui => {
			if (ui.GetType() == u.Type) {
				unitUI = ui;
			}
		});



		if (unitUI == null) {
			GameObject thisUI = p.Type == PlayerType.Battlebeard ? BattlebeardUI : StormshaperUI;
			unitUI = createUIForUnitType(thisUI, u.Type);
			int index = (int)u.Type <= data.Count ? (int)u.Type : data.Count;
			data.Insert(index, unitUI);
		}

		unitUI.AddUnit(u);

	}

	public void RemoveUnit(Player p, Unit u) {

	}

	public void UpdateUnit(Player p, Unit u) {
		List<UnitUIAll> data;
		uiDataNew.TryGetValue(p.Type, out data);
	}

	void showSubmenu(int i) {
		List<UnitUIAll> data;
		uiDataNew.TryGetValue(currentPlayer, out data);
		data[i].Maximise();
	}

	void hideSubmenu(int i) {
		List<UnitUIAll> data;
		uiDataNew.TryGetValue(currentPlayer, out data);
		data[i].Minimise();
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
					if (upDown) {
						// select the previous menu
						setSelection((currentSelection - 1 + currentCount) % currentCount);
					} else if (downDown) {
						//select the next menu
						setSelection((currentSelection + 1) % currentCount);
					}
				}
			}
		}
		
	}

	void OnGUI() {
		// listen for mouse move
		if (Input.GetAxis("Mouse X") != lastMousePos.x || Input.GetAxis("Mouse Y") != lastMousePos.y) {
			List<UnitUIAll> data;
			uiDataNew.TryGetValue(currentPlayer, out data);
			for (int i = 0; i < currentCount; i++) {
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
