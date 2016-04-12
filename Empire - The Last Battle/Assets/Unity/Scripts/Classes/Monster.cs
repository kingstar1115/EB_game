using UnityEngine;
using System.Collections;

public class Monster : iBattleable {

    int hp, currentHp, speed, strength;
    public MonsterType Type;

	SoundEffect hitSound;
	SoundEffect attackSound;
	SoundEffect deathSound;

	public Monster(MonsterBaseData data, MonsterType type) {
        hp = data.HP;
        currentHp = data.HP;
        speed = data.Speed;
        strength = data.Strength;
        Type = type;
		hitSound = data.HitSound;
		attackSound = data.AttackSound;
		deathSound = data.DeathSound;
    }

    public bool IsKO() {
        return currentHp <= 0;
    }

	public bool isLostImmortal() {
		return (int)Type > 10 && (int)Type < 19;
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

    public int GetUniqueID()
    {
        return this.GetHashCode();
    }


	public SoundEffect GetHitSound() {
		return hitSound;
	}

	public SoundEffect GetAttackSound() {
		return attackSound;
	}

	public SoundEffect GetDeathSound() {
		return deathSound;
	}
}
