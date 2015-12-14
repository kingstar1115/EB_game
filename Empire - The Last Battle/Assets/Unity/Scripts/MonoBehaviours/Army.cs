using UnityEngine;
using System.Collections.Generic;

public class Army : MonoBehaviour {

	public List<Unit> Units;

	void Start()
	{

	}

	public List<Unit> GetUnits(UnitType Type)
	{

		List<Unit> SpecificUnits;
		SpecificUnits = Units.FindAll(_Unit => _Unit.Type == Type);

		return SpecificUnits;
	}

	public List<Unit> GetRandomUnits(int maxNumber, bool b_ShouldBeKo = false)
	{
		var RandomUnits = new List<Unit>();
		List<Unit> AllUnits = Units;

		int i = 0;

		while (i < maxNumber)
		{
			var randomNumber = UnityEngine.Random.Range(0, AllUnits.Count - 1);

			if (b_ShouldBeKo)
			{
				//Unit isn't KO then start loop again until we get KO troop
				if (!Units[randomNumber].IsKO())
					continue;					
			}

			RandomUnits.Add(Units[randomNumber]);
			AllUnits.RemoveAt(randomNumber);
			i++;
		}
		return RandomUnits;
	}

	public List<Unit> GetAllUnits()
	{
		return Units;
	}
}
