using UnityEngine;
using System.Collections;

public delegate void UnitCallback(Unit u);
public delegate void UnitIndexCallback(Unit u, int i);

[System.Serializable]
public class Unit : ScriptableObject, iBattleable {

	[SerializeField]
	UnitBaseData BaseData;
	//TODO: Decide if this is the best way to handle an upgrade. Is it better to have a separate Upgrade class?
	[SerializeField]
	UnitBaseData CurrentUpgrade;
	[SerializeField]
	UnitBaseData CurrentTempUpgrade;
	//CurrentBaseHP will have a max value that is equal to the BaseData.HP value
	[SerializeField]
	int CurrentBaseHP;
	public TileData Position;
	public UnitType Type;
	public event UnitCallback OnUpdate = delegate { };

	public void Initialise(UnitBaseData data){
		Type = data.Type;
		BaseData = data;
		CurrentBaseHP = data.HP;
	}

	public void RemoveListeners() {
		OnUpdate = delegate { };
	}

	public bool IsKO() {
		return GetCurrentHP() <= 0;
	}

	public bool IsUpgradeable() {
		return IsKO () || !HasUpgrade();
	}

	public bool IsTempUpgradeable() {
		return IsKO() || !HasTempUpgrade();
	}

	//For example if CurrentHP = -2 and CurrentUpgrade.HP is = 3 then this will return 1
	public int GetCurrentHP() {
		int currentHP = CurrentUpgrade == null ? CurrentBaseHP : CurrentBaseHP + CurrentUpgrade.HP;
        if (currentHP < 0) {
            return 0;
        }
        return currentHP;
	}

	public int GetStrength() {
		int totalStrength = BaseData.Strength;
		if (CurrentUpgrade != null) {
			totalStrength += CurrentUpgrade.Strength;
		}
		if (CurrentTempUpgrade != null) {
			totalStrength += CurrentTempUpgrade.Strength;
		}
		return totalStrength;
	}

	public int GetSpeed() {
		return CurrentUpgrade == null ? BaseData.Speed : BaseData.Speed + CurrentUpgrade.Speed;
	}

	public float GetHPPercentage() {
		int MaxHp = CurrentUpgrade == null ? BaseData.HP : BaseData.HP + CurrentUpgrade.HP;
		float Percentage = ((float)GetCurrentHP() / (float)MaxHp);
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

	public bool HasAnyUpgrade() {
		return HasUpgrade() || HasTempUpgrade();
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

	public bool HasTempUpgrade() {
		return CurrentTempUpgrade != null;
	}

	public void AddTempUpgrade(UnitBaseData Upgrade) {
		if (CurrentTempUpgrade == null) {
			CurrentTempUpgrade = Upgrade;
			OnUpdate(this);
		}

	}

	public void RemoveTempUpgrade() {
		CurrentTempUpgrade = null;
		OnUpdate(this);
	}

	public UnitBaseData CreateUpgrade() {
		UnitBaseData Upgrade = UnitBaseData.CreateInstance<UnitBaseData>();
		Upgrade.Type = Type;
		Upgrade.HP = (int)Mathf.Round((float)BaseData.HP / 10);
		Upgrade.Strength = (int)Mathf.Round((float)BaseData.Strength / 10);
		Upgrade.Speed = (int)Mathf.Round((float)BaseData.Speed / 10);
		return Upgrade;
	}
}
