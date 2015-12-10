using UnityEngine;
using System.Collections;

[System.Serializable]
public class Unit : MonoBehaviour {

	public static UnitBaseData UnitBaseData;
	//TODO: Decide if this is the best way to handle an upgrade. Is it better to have a separate Upgrade class?
	public UnitBaseData Upgrade;
	public TileData Position;
	public static UnitType Type;

	void Start () {
	
	}


	public bool IsKO(){
		return UnitBaseData.HP <= 0;
	}
}
