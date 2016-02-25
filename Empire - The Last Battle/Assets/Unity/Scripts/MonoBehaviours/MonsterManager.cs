using UnityEngine;
using System.Collections.Generic;

public class MonsterManager : MonoBehaviour {

    public List<MonsterBaseData> monsterData;

    public MonsterBaseData GetMonsterBaseData(MonsterType t) {
        return monsterData[(int)t];
    }

    public Monster NewMonster(MonsterType t) {
        return new Monster(GetMonsterBaseData(t));
    }

}
