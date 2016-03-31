using UnityEngine;
using System.Collections.Generic;

public class Army : ScriptableObject {

	public UnitsData _UnitDataManager;
	[SerializeField]
	List<Unit> units;
	public Army defaultData;
	public event UnitCallback OnUpdateUnit = delegate { };
	public event UnitCallback OnAddUnit = delegate { };
	public event UnitIndexCallback OnRemoveUnit = delegate { };

	public void Initialise() {
        // dont call this twice or it will keep adding listeners
        foreach(Unit u in units) {
            if (u != null) {
                u.OnUpdate += unitUpdated;
            }
        }
    }

    public void Reset() {
        foreach(Unit u in units) {
            if (u != null) {
                u.RemoveListeners();
            }
        }
        units.Clear();
    }

	public Unit AddUnit(UnitType type) {
		Unit u = ScriptableObject.CreateInstance<Unit>();
		u.Initialise(_UnitDataManager.GetData(type));
		units.Add(u);
		u.OnUpdate += unitUpdated;
		OnAddUnit(u);
		return u;
	}

	public void RemoveUnit(Unit u) {
		int i = units.IndexOf(u);
		if (i != -1) {
			units.Remove(u);
			// destroy?
			u.OnUpdate -= unitUpdated;
			OnRemoveUnit(u, i);
		}
	}

	public List<Unit> GetUpgradableUnits() {
		return units.FindAll(x => x.IsActive() && x.IsUpgradeable());
	}

	public List<Unit> GetDefendingUnits() {
		return units.FindAll(x => x.IsDefending());
	}

	public List<Unit> GetNonDefendingUnits() {
		return units.FindAll(x => !x.IsDefending());
	}

	public List<Unit> GetPrisonerUnits() {
		return units.FindAll(x => x.IsPrisoner ());
	}

	public List<Unit> GetNonPrisonerUnits() {
		return units.FindAll(x => !x.IsPrisoner ());
	}

	public Unit GetDefendingUnit(TileData tile) {
		return units.Find(_Unit => _Unit.Position == tile);
	}

	public List<Unit> GetTempUpgradableUnits() {
		return units.FindAll(x => x.IsActive() && x.IsTempUpgradeable());
	}

	public List<Unit> GetUnits(UnitType type) {
		return units.FindAll(_Unit => _Unit.Type == type);
	}

	public List<Unit> GetUnits() {
		return units;
	}

	public List<Unit> GetActiveUnits(UnitType type) {
		return units.FindAll(x => x.IsActive() && x.Type == type);
	}

	public List<UnitType> GetActiveUnitTypes() {
		List<UnitType> types = new List<UnitType>();
		foreach (Unit u in GetActiveUnits()) {
			if (!types.Contains(u.Type)) {
				types.Add(u.Type);
			}
		}
		return types;
	}

	public List<Unit> GetActiveUnits() {
		return units.FindAll(x => x.IsActive());
	}

	public List<Unit> GetKOUnits(UnitType type) {
		return units.FindAll(x => x.IsKO() && x.Type == type);
	}

	public List<Unit> GetKOUnits() {
		return units.FindAll(x => x.IsKO());
	}

	public List<Unit> GetRandomUnits(int maxNumber, List<Unit> from) {
		List<Unit> RandomUnits = new List<Unit>();
		List<Unit> AllUnits = new List<Unit>(from);
		if (AllUnits.Count < maxNumber) {
			maxNumber = AllUnits.Count;
		}
		for (int i = 0; i < maxNumber; i++) {
			var randomNumber = Random.Range(0, AllUnits.Count - 1);
			RandomUnits.Add(AllUnits[randomNumber]);
			AllUnits.RemoveAt(randomNumber);
		}
		return RandomUnits;
	}

	public List<Unit> GetUpgradedUnits() {
		return units.FindAll(x => x.HasUpgrade());

	}

	public List<Unit> GetTempUpgradedUnits() {
		return units.FindAll(x => x.HasTempUpgrade());
	}

	void unitUpdated(Unit u) {
		OnUpdateUnit(u);
	}

	public void RemoveListeners() {
		OnUpdateUnit = delegate { };
		OnAddUnit = delegate { };
		OnRemoveUnit = delegate { };
		foreach (Unit u in units) {
			u.RemoveListeners();
		}
	}
}
