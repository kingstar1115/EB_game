using UnityEngine;
using System.Collections;

[System.Serializable]
public class Unit : MonoBehaviour {

	public UnitBaseData BaseData;
	//TODO: Decide if this is the best way to handle an upgrade. Is it better to have a separate Upgrade class?
	public UnitBaseData Upgrade;
	public TileData Position;
	public UnitType Type;

	void Start () {
		
	}

	public bool IsKO() {
		return GetHP() <= 0;
	}

	public int GetHP() {
		return Upgrade == null ? BaseData.HP : BaseData.HP + Upgrade.HP;
	}

	public int GetStrength() {
		return Upgrade == null ? BaseData.Strength : BaseData.Strength + Upgrade.Strength;
	}

	public int GetSpeed() {
		return Upgrade == null ? BaseData.Speed : BaseData.Speed + Upgrade.Speed;
	}
}
