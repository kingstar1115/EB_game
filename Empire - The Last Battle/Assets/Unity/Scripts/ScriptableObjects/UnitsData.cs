using UnityEngine;
using System.Collections.Generic;

public class UnitsData : ScriptableObject {
	public List<UnitBaseData> UnitData;

	public UnitBaseData GetData(UnitType t) {
        return UnitData.Find(x => x.Type == t);
	}
}
