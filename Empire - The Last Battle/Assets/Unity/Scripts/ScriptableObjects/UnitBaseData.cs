using UnityEngine;

public class UnitBaseData : ScriptableObject {
	public UnitType Type;
	public int Speed;
    public int Strength;
    public int HP;
}

public enum UnitType {
	Scout,
	Pikeman,
	Archer,
    AxeThrower,
	Warrior,
	Cavalry,
	Catapult,
	Ballista
}