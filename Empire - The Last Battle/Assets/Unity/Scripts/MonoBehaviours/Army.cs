using UnityEngine;
using System.Collections.Generic;

public class Army : MonoBehaviour {

	List<Unit> Units;
	public UnitDataManager _UnitDataManager;

	public void Initialise() {
		Units = new List<Unit>();
	}

	public void AddUnit(UnitType type) {
		Units.Add(new Unit(_UnitDataManager.GetData(type)));
	}

	public List<Unit> GetUnits(UnitType type) {
		List<Unit> SpecificUnits;
		SpecificUnits = Units.FindAll(_Unit => _Unit.Type == type);

		return SpecificUnits;
	}

	public List<Unit> GetActiveUnits(UnitType type) {
		return Units.FindAll(x => !x.IsKO() && x.Type == type);
	}

	public List<Unit> GetKOUnits(UnitType type) {
		return Units.FindAll(x => x.IsKO() && x.Type == type);
	}

	public List<Unit> GetKOUnits() {
		return Units.FindAll(x => x.IsKO());
	}

	public List<Unit> GetActiveUnits() {
		return Units.FindAll(x => !x.IsKO());
	}

	public List<Unit> GetRandomUnits(int maxNumber, bool b_ShouldBeKo = false) {
		var RandomUnits = new List<Unit>();
		List<Unit> AllUnits = b_ShouldBeKo ? GetKOUnits() : Units;
		if (AllUnits.Count < maxNumber) {
			maxNumber = AllUnits.Count;
		}
		for (int i = 0; i < maxNumber; i++) {
			var randomNumber = UnityEngine.Random.Range(0, AllUnits.Count - 1);
			RandomUnits.Add(AllUnits[randomNumber]);
			AllUnits.RemoveAt(randomNumber);
		}
		return RandomUnits;
	}

	public List<Unit> GetAllUnits() {
		return Units;
	}
}