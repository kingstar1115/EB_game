using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UnitDataManager : MonoBehaviour {
	public List<UnitBaseData> UnitData;

	public UnitBaseData GetData(UnitType t) {
		UnitBaseData b = null;
		foreach (UnitBaseData bd in UnitData) {
			if (bd.Type == t) {
				b = bd;
			}
		}
		return b;
	}
}
