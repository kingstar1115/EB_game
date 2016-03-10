using UnityEngine;
using System.Collections.Generic;

public class MonsterManager : MonoBehaviour {

    public List<MonsterBaseData> monsterData;

    public MonsterBaseData GetMonsterBaseData(MonsterType t) {
        return monsterData[(int)t];
    }

    public Monster NewMonster(MonsterType t) {
        return new Monster(GetMonsterBaseData(t), t);
    }

}

public enum MonsterType {
	Cyclops,
	Minotaur,
	Sasquatch,
	FireElemental,
	WaterElemental,
	EarthElemental,
	AirElemental,
	Wyrm,
	Wyvern,
	Dragon,
	Hydra,
	LostImmortalSs1,
	LostImmortalSs2,
	LostImmortalSs3,
	LostImmortalSs4,
	LostImmortalBb1,
	LostImmortalBb2,
	LostImmortalBb3,
	LostImmortalBb4
}