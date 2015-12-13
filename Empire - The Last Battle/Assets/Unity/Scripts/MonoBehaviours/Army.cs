using UnityEngine;
using System.Collections.Generic;

public class Army : MonoBehaviour {

	public List<Unit> Units;

	void Start () {
	
	}

	public List<Unit> GetUnits(UnitType Type){

		List<Unit> SpecificUnits;
		SpecificUnits = Units.FindAll(_Unit => _Unit.Type == Type);

		return SpecificUnits;
	}

	public List<Unit> GetAllUnits(){
		return Units;
	}
}
