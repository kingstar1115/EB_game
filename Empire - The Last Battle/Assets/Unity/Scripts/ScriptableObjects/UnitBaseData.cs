using UnityEngine;

public class UnitBaseData : ScriptableObject {
	public UnitType Type;
	public int Speed;
    public int Strength;
    public int HP;
}

// This is the correct order!
public enum UnitType {
	Scout,
	Pikemen,
	Archer,
	AxeThrower,
	Warrior,
	Cavalry,
	Catapult,
	Ballista
}