using UnityEngine;
using System.Collections;

public delegate void UnitCallback(Unit u);
public delegate void UnitIndexCallback(Unit u, int i);

[System.Serializable]
public class Unit {

	UnitBaseData BaseData;
	//TODO: Decide if this is the best way to handle an upgrade. Is it better to have a separate Upgrade class?
	UnitBaseData CurrentUpgrade;
	//CurrentBaseHP will have a max value that is equal to the BaseData.HP value
	int CurrentBaseHP;
	public TileData Position;
	public UnitType Type;
	public event UnitCallback OnUpdate = delegate { };

	public Unit(UnitBaseData data){
		Type = data.Type;
		BaseData = data;
		CurrentBaseHP = data.HP;
	}

	public bool IsKO() {
		return GetCurrentHP() <= 0;
	}

	//For example if CurrentHP = -2 and CurrentUpgrade.HP is = 3 then this will return 1
	public int GetCurrentHP() {
		return CurrentUpgrade == null ? CurrentBaseHP : CurrentBaseHP + CurrentUpgrade.HP;
	}

	public int GetStrength() {
		return CurrentUpgrade == null ? BaseData.Strength : BaseData.Strength + CurrentUpgrade.Strength;
	}

	public int GetSpeed() {
		return CurrentUpgrade == null ? BaseData.Speed : BaseData.Speed + CurrentUpgrade.Speed;
	}

	public float GetHPPercentage() {
		int MaxHp = CurrentUpgrade == null ? BaseData.HP : BaseData.HP + CurrentUpgrade.HP;
		float Percentage = ((float)GetCurrentHP() / (float)MaxHp) * 100;
		return Percentage;
	}

	public void ReduceHP(int HP) {
		CurrentBaseHP -= HP;
		OnUpdate(this);
	}

	public void Heal() {
		CurrentBaseHP = BaseData.HP;
		OnUpdate(this);
	}

	public bool HasUpgrade() {
		return CurrentUpgrade != null;
	}

	public void AddUpgrade(UnitBaseData Upgrade) {
		//Don't apply the upgrade if a unit is knocked out, should probably return something to acknowledge that it wasn't applied
		if(!IsKO()) {
			CurrentUpgrade = Upgrade;
			OnUpdate(this);
		}
	}

	public void RemoveUpgrade() {
		CurrentUpgrade = null;
		OnUpdate(this);
	}
}
