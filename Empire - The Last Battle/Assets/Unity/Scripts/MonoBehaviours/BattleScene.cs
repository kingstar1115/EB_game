using UnityEngine;
using System.Collections.Generic;

public class BattleScene : MonoBehaviour {

	public List<GameObject> _Buildings;
	public List<GameObject> _Terrains;
	public BattleData _BattleData;
	public GameObject BattleUI;

	// Use this for initialization
	public void Initialise () {
		BuildingType b = _BattleData._InitialPlayer.CommanderPosition.Building;
		TerrainType t = _BattleData._InitialPlayer.CommanderPosition.Terrain;

		Vector3 buildingOffset = new Vector3();
		GameObject building = null;

		// setup building
		try {
			if (b == BuildingType.Camp) {
				building = Instantiate(_Buildings[0]);
			}
			else if (b == BuildingType.Cave) {
				buildingOffset = new Vector3(0, 7);
				building = Instantiate(_Buildings[1]);
			}
			else if (b == BuildingType.Fortress) {
				buildingOffset = new Vector3(0, 13.279f);
				building = Instantiate(_Buildings[2]);
			}
		}
		catch {
			Debug.LogError("Building index out of bounds");
		}

		building.transform.SetParent(BattleUI.transform);

		// setup terrain position
		BattleUI.transform.position = _Terrains[(int)t - 4].transform.position + buildingOffset;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
