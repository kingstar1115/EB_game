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
	[Range(0, 1)]
	public float MaxWidth = 820;
	public float MinWidth = 105;

	PlayerType currentPlayer;
	Dictionary<PlayerType, Dictionary<MouseOverItem, List<UnitUI>>> uiData;
	List<MouseOverItem> cachedUnitHolders;

	int selection = -1;
	int currentSelection {
		get {
			return selection;
		}
		set {
			if (value == selection) {
				return;
			}
			else if (value == -1) {
				hideSubmenu(selection);
			}
			else {
				if (selection != -1) {
					hideSubmenu(selection);
				}
				showSubmenu(value);
			}
			selection = value;
		}
	}

	// Use this for initialization
	void Initialise () {
		uiData = new Dictionary<PlayerType, Dictionary<MouseOverItem, List<UnitUI>>>();
		for (int i = 1; i <= 2; i++) {
			Dictionary<MouseOverItem, List<UnitUI>> playerUI = new Dictionary<MouseOverItem, List<UnitUI>>();
			MouseOverItem[] t = UI[i-1].GetComponentsInChildren<MouseOverItem>();
			foreach (MouseOverItem child in t) {
				if (child.transform.parent == UI[i-1].transform) {
					playerUI.Add(child, new List<UnitUI>(child.GetComponentsInChildren<UnitUI>()));
				}
			}
			uiData.Add((PlayerType)i, playerUI);
		}
	}



	List<MouseOverItem> getUnitHolders() {
		if (cachedUnitHolders == null) {
			Dictionary<MouseOverItem, List<UnitUI>> selection;
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
		cachedUnitHolders = null;
		currentSelection = -1;
	}

	public void GenerateUI(Player p) {

	}

	void showSubmenu(int i) {
		MouseOverItem currentItem = getUnitHolders()[i];
		RectTransform rect = currentItem.GetComponentInChildren<RectTransform>();
		rect.offsetMax = new Vector2(MaxWidth, rect.offsetMax.y);
		currentItem.GetComponentInChildren<CanvasGroup>().alpha = 1;
	}

	void hideSubmenu(int i) {
		MouseOverItem currentItem = getUnitHolders()[i];
		RectTransform rect = currentItem.GetComponentInChildren<RectTransform>();
		rect.offsetMax = new Vector2(MinWidth, rect.offsetMax.y);
		currentItem.GetComponentInChildren<CanvasGroup>().alpha = 0;
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

	// Update is called once per frame
	void OnGUI () {
		// listen for mouseover
		List<MouseOverItem> units = getUnitHolders();
		for (int i = 0; i < units.Count; i++) {
			if (units[i].isOver) {
				currentSelection = i;
				if (currentSelection != i) {
					//if (currentSelection != -1) {
					//	hideSubmenu(currentSelection);
					//}
					//showSubmenu(i);
					//currentSelection = i;
				}
			}
			else if (currentSelection == i) {
				//hideSubmenu(currentSelection);
				currentSelection = -1;
			}
		}
	}

	void Start() {
		SwitchPlayer(PlayerType.Battlebeard);
		Initialise();
	}

}
