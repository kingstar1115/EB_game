using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class ArmyUI : MonoBehaviour {
	
	public GameObject[] UI;
	GameObject currentUI {
		get {
			return UI[(int)currentPlayer - 1];
		}
	}
	public float MaxWidth = 820;
	public float MinWidth = 105;
	public float SmallIconSize = 88.75f;
	public float LargeIconSize = 110;

	PlayerType currentPlayer;
	Dictionary<PlayerType, Dictionary<MouseOverItem, List<UnitUI>>> uiData;
	// cache
	List<MouseOverItem> cachedUnitHolders;
	Dictionary<MouseOverItem, List<UnitUI>> cachedUIData;

	Vector2 lastMousePos;

	int previousSelection = 0;
	int currentSelection = -1;

	void setSelection(int i) {
		if (i == currentSelection || uiData == null) {
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
	public void Initialise () {
		currentPlayer = PlayerType.Battlebeard;
		lastMousePos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		uiData = new Dictionary<PlayerType, Dictionary<MouseOverItem, List<UnitUI>>>();
		Debug.Log(uiData);
		for (int i = 1; i <= 2; i++) {
			Dictionary<MouseOverItem, List<UnitUI>> playerUI = new Dictionary<MouseOverItem, List<UnitUI>>();
			MouseOverItem[] t = UI[i-1].GetComponentsInChildren<MouseOverItem>();
			for (int j = 0; j < t.Length; j++) {
				if (t[j].transform.parent == UI[i-1].transform) {

					playerUI.Add(t[j], new List<UnitUI>(t[j].GetComponentsInChildren<UnitUI>()));
				}
			}
			uiData.Add((PlayerType)i, playerUI);
		}
	}

	Dictionary<MouseOverItem, List<UnitUI>> getUIData() {
		if (cachedUIData == null) {
			Dictionary<MouseOverItem, List<UnitUI>> selection;
			Debug.Log(currentPlayer);
			Debug.Log(uiData);
			uiData.TryGetValue(currentPlayer, out selection);
			if (selection != null) {
				cachedUIData = selection;
			} else {
				return null;
			}
		}
		return cachedUIData;
	}

	List<UnitUI> getUnitData(MouseOverItem m) {
		Dictionary<MouseOverItem, List<UnitUI>> uiData = getUIData();
		List<UnitUI> ui;
		uiData.TryGetValue(m, out ui);
		return ui;
	}

	List<MouseOverItem> getUnitHolders() {
		if (cachedUnitHolders == null) {
			Dictionary<MouseOverItem, List<UnitUI>> selection = getUIData();
			uiData.TryGetValue(currentPlayer, out selection);
			if (selection != null) {
				cachedUnitHolders = new List<MouseOverItem>(selection.Keys);
			}
			else {
				return null;
			}
		}
		return cachedUnitHolders;
		
	}

	public void SwitchPlayer(PlayerType p) {
		currentPlayer = p;
		resetUI();
	}

	void resetUI() {
		clearCache();
		setSelection(-1);
		previousSelection = 0;
	}

	void clearCache() {
		cachedUnitHolders = null;
		cachedUIData = null;
	}

	public void GenerateUI(Player p) {

	}

	void showSubmenu(int i) {
		MouseOverItem currentItem = getUnitHolders()[i];
		LerpRectTransform trans = currentItem.GetComponentInChildren<LerpRectTransform>();
		// Resize item
		float width = getUnitData(currentItem).Count * SmallIconSize + LargeIconSize;
		if (width > MaxWidth) { width = MaxWidth; }
		trans.ResizeWidth(width);
		// fade in
		FadeCanvasGroup fCG = currentItem.GetComponentInChildren<FadeCanvasGroup>();
		fCG.FadeTo(1);
	}

	void hideSubmenu(int i) {
		MouseOverItem currentItem = getUnitHolders()[i];
		LerpRectTransform trans = currentItem.GetComponentInChildren<LerpRectTransform>();
		// resize item
		trans.ResizeWidth(MinWidth);
		// fade out
		FadeCanvasGroup fCG = currentItem.GetComponentInChildren<FadeCanvasGroup>();
		fCG.FadeTo(0);
	}

	void addUnit() {
		// grow ui
	}

	void removeUnit() {
		// shrink ui
	}

	public void UpdateUI(Player p, Unit u, bool removeUnit = false) {
		List<List<GameObject>> playerList = null;
		//UIData.TryGetValue (p.Type, out playerList);
		if (playerList != null) {
			List<GameObject> listToUpdate = playerList[(int)u.Type];
			int index = p.PlayerArmy.GetAllUnits().IndexOf(u);
			if (index != -1) {
				GameObject unitObject = listToUpdate[index];
				if (removeUnit) {
					listToUpdate.Remove(unitObject);
					return;
				}
				if (u.IsKO()) {
					// add KO icon
				}
				if (u.HasUpgrade()) {
					// add upgrade icon
				}
				return;
			} else {
				// unit does not exist. cannot remove from UI!!!
				Debug.LogError("Unit should have been removed from UI first");
			}

		}
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
						setSelection((currentSelection - 1 + cachedUnitHolders.Count) % cachedUnitHolders.Count);
					} else if (downDown) {
						//select the next menu
						setSelection((currentSelection + 1) % cachedUnitHolders.Count);
					}
				}
			}
		}
		
	}

	void OnGUI () {
		// listen for mouse move
		if (Input.GetAxis("Mouse X") != lastMousePos.x || Input.GetAxis("Mouse Y") != lastMousePos.y) {
			List<MouseOverItem> units = getUnitHolders();
			for (int i = 0; i < units.Count; i++) {
				if (units[i].isOver) {
					// show menu at i
					setSelection(i);
				} else if (currentSelection == i) {
					// hide all menus
					setSelection(-1);
				}
			}
		}
		lastMousePos = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
	}

}
