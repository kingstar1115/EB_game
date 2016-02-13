using UnityEngine;
using System.Collections.Generic;

public class BattleScene : MonoBehaviour {

	public List<GameObject> _Buildings;
	public List<GameObject> _Terrains;
	public BattleData _BattleData;

	// Use this for initialization
	void Start () {
		BuildingType b = _BattleData._InitialPlayer.CommanderPosition.Building;
		TerrainType t = _BattleData._InitialPlayer.CommanderPosition.Terrain;

		// setup building
		try {
			if (b == BuildingType.Camp) {
				Instantiate(_Buildings[0]);
			}
			else if (b == BuildingType.Cave) {
				Instantiate(_Buildings[1]);
			}
			else if (b == BuildingType.Fortress) {
				Instantiate(_Buildings[2]);
			}
		}
		catch {
			Debug.LogError("Building index out of bounds");
		}

		// setup terrain
		try {
			GameObject terrain = _Terrains[(int)t-4];
			Instantiate(terrain);
		}
		catch {
			Debug.LogError("Terrain index out of bounds");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
