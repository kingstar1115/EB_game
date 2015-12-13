using UnityEngine;

public class UnitBaseData : ScriptableObject {
    public int Speed;
    public int Strength;
    public int HP;
}

public enum UnitType {
    Archer,
    AxeThrower,
    Ballista,
    Catapult,
    Cavalry,
    Scout,
    Pikemen,
    Warrior
}