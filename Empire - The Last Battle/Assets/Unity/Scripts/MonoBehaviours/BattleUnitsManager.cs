using UnityEngine;
using System.Collections.Generic;

public class BattleUnitsManager : MonoBehaviour {

	public List<GameObject> _BattlebeardUnits;
	public List<GameObject> _StormshaperUnits;
	public List<GameObject> _Monsters;
	public List<GameObject> _LostImmortals;

	public GameObject _MarkerActivePlayerUnit;
	public GameObject _MarkerActivePlayerScouts;
	public GameObject _MarkerActivePlayerPikemen;
	public GameObject _MarkerActivePlayerAxeThrowers;
	public GameObject _MarkerActivePlayerArchers;
	public GameObject _MarkerActivePlayerCavalry;
	public GameObject _MarkerActivePlayerWarriors;
	public GameObject _MarkerActivePlayerCatapult;
	public GameObject _MarkerActivePlayerBallista;
	public GameObject _MarkerActiveOpposition;
	public GameObject _MarkerReserveOppositionA;
	public GameObject _MarkerReserveOppositionB;

	// Use this for initialization
	void Start () {
	
	}
	


	// Update is called once per frame
	void Update () {
	
	}

	public void SetUnit(UnitType t, PlayerType p) {
		List<GameObject> prefabs = p == PlayerType.Battlebeard ? _BattlebeardUnits : _StormshaperUnits;
		GameObject marker;
		switch (t) {
			case UnitType.Archer:
				marker = _MarkerActivePlayerArchers;
				break;
			case UnitType.AxeThrower:
				marker = _MarkerActivePlayerAxeThrowers;
				break;
			case UnitType.Ballista:
				marker = _MarkerActivePlayerBallista;
				break;
			case UnitType.Catapult:
				marker = _MarkerActivePlayerCatapult;
				break;
			case UnitType.Cavalry:
				marker = _MarkerActivePlayerCavalry;
				break;
			case UnitType.Pikeman:
				marker = _MarkerActivePlayerPikemen;
				break;
			case UnitType.Scout:
				marker = _MarkerActivePlayerScouts;
				break;
			case UnitType.Warrior:
				marker = _MarkerActivePlayerWarriors;
				break;
			default:
				marker = _MarkerActivePlayerScouts;
				break;
		}
		// should probably save them sometime
		Instantiate(prefabs[(int)t], marker.transform.position, marker.transform.rotation);
	}

	public void SetActiveUnit(UnitType t, PlayerType p) {
		List<GameObject> prefabs = p == PlayerType.Battlebeard ? _BattlebeardUnits : _StormshaperUnits;
		setOpposition(prefabs[(int)t]);
	}

	public void SetOpposition(UnitType t, PlayerType p) {
		List<GameObject> prefabs = p == PlayerType.Battlebeard ? _BattlebeardUnits : _StormshaperUnits;
		setOpposition(prefabs[(int)t]);
	}

	public void SetOpposition(LostImmortal t) {
		setOpposition(_LostImmortals[(int)t]);
	}

	public void SetOpposition(Monster t) {
		setOpposition(_Monsters[(int)t]);
	}

	void setOpposition(GameObject g) {
		Instantiate(g, _MarkerActiveOpposition.transform.position, _MarkerActiveOpposition.transform.rotation);
	}

	public void SetReserveOppositionA(Monster t) {

	}

	public void SetReserveOppositionB(Monster t) {

	}
}


public enum Monster {
	Minotaur,
	Ogre,
	Sasquatch
}

public enum LostImmortal {
	bb1,
	bb2,
	bb3,
	bb4,
	ss1,
	ss2,
	ss3,
	ss4
}