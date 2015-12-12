using UnityEngine;
using System.Collections;

[System.Serializable]
public class Unit : MonoBehaviour {

	public UnitBaseData BaseData;
	//TODO: Decide if this is the best way to handle an upgrade. Is it better to have a separate Upgrade class?
	UnitBaseData CurrentUpgrade;
	public TileData Position;
	public UnitType Type;
	public int CurrentHP;

	void Start () {
		
	}

	public bool IsKO() {
		return GetCurrentHP() <= 0;
	}

	public int GetCurrentHP() {
		return CurrentUpgrade == null ? CurrentHP : CurrentHP + CurrentUpgrade.HP;
	}

	public int GetStrength() {
		return CurrentUpgrade == null ? BaseData.Strength : BaseData.Strength + CurrentUpgrade.Strength;
	}

	public int GetSpeed() {
		return CurrentUpgrade == null ? BaseData.Speed : BaseData.Speed + CurrentUpgrade.Speed;
	}

	public void ReduceHP(int HP) {
		CurrentHP -= HP;
	}

	public void Heal(int HP) {
		CurrentHP = BaseData.HP; 
	}

	public void AddUpgrade(UnitBaseData Upgrade) {
		CurrentUpgrade = Upgrade;
	}

	public void RemoveUpgrade() { 
		CurrentUpgrade = null;
	}
}
