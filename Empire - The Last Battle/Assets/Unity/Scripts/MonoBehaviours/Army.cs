using UnityEngine;
using System.Collections.Generic;

public class Army : MonoBehaviour
{

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

	public List<Unit> GetRandomUnits(int maxNumber)
	{
		var RandomUnits = new List<Unit>();

		for (int i = 0; i < maxNumber; i++)
		{
			var randomNumber = UnityEngine.Random.Range(0, Units.Count - 1);
			RandomUnits.Add(Units[randomNumber]);
		}

		return RandomUnits;
	}

	public List<Unit> GetAllUnits()
	{
		return Units;
	}
}
