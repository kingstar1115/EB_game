using UnityEngine;
using System.Collections.Generic;
using System;

public class ArmyUI : MonoBehaviour {
	
	public GameObject BattlebeardUI;
	public GameObject StormshaperUI;
	GameObject currentUI;
	Dictionary<PlayerType, List<List<GameObject>>> UIData;

	// Use this for initialization
	void Initialise () {
		UIData = new Dictionary<PlayeqedrType, List<List<GameObject>>> ();
		for (int i = 1; i <= 2; i++) {
			List<List<GameObject>> playerUI = new List<List<GameObject>>();
			for (int j = 0; j < Enum.GetNames(typeof(UnitType)).Length; j++) {
				List<GameObject> unitList = new List<GameObject>();
				playerUI.Add(unitList);
			}
			UIData.Add((PlayerType)i, playerUI);
		}
	}

	public void SwitchPlayer(Player p) {
		currentUI = p.Type == PlayerType.Battlebeard ? 
			BattlebeardUI : StormshaperUI;
	}

	public void GenerateUI(Player p) {

	}

	public void ShowSubmenu(UnitType t) {

	}

	void addUnit() {
		// grow ui
	}

	void removeUnit() {
		// shrink ui
	}

	public void UpdateUI(Player p, Unit u, bool removeUnit = false) {
		List<List<GameObject>> playerList;
		UIData.TryGetValue (p.Type, out playerList);
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
	void Update () {
		
	}
}

public class ArmyUIData {

}
