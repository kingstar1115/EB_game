using UnityEngine;
using System.Collections.Generic;

public class Army : MonoBehaviour {

	public List<Unit> Units;

	void Start () {
	
	}

	public List<Unit> GetUnits(UnitType Type){
		List<Unit> SpecificUnits;

		SpecificUnits = Units.FindAll( x => Unit.Type == Type);

		return SpecificUnits;
	}

	public List<Unit> GetAllUnits(){
		return Units;
	}
}
