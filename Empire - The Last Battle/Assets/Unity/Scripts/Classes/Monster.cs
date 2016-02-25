using UnityEngine;
using System.Collections;

public class Monster : iBattleable {

    int hp, currentHp, speed, strength;
    public MonsterType Type;

    public Monster(MonsterBaseData data) {
        hp = data.HP;
        currentHp = data.HP;
        speed = data.Speed;
        strength = data.Strength;
        Type = data.Type;
    }

    public bool IsKO() {
        return currentHp <= 0;
    }

    public int GetCurrentHP() {
		if (currentHp < 0) { return 0; }
        return currentHp;
    }

    public int GetStrength() {
        return strength;
    }

    public int GetSpeed() {
        return speed;
    }

    public float GetHPPercentage() {
       return ((float)GetCurrentHP() / (float)hp);
    }

    public void ReduceHP(int HP) {
        currentHp -= HP;
    }

    public void Heal() {
        currentHp = hp;
    }
}
