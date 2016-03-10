using UnityEngine;
using System.Collections.Generic;

public class BattleUnitPositionManager : MonoBehaviour {

    public List<GameObject> _BattlebeardUnits;
    public List<GameObject> _StormshaperUnits;
    public List<GameObject> _Monsters;

    public GameObject _MarkerActivePlayerUnit;
    public GameObject [] _MarkerActivePlayerUnits;
    public GameObject _MarkerActiveOpposition;
    public GameObject _MarkerReserveOppositionA;
    public GameObject _MarkerReserveOppositionB;
    public GameObject _MarkerReserveOppositionC;

    public GameObject _ActivePlayerUnit;
    public GameObject [] _ActivePlayerUnits = new GameObject[8];
    public GameObject _ActiveOpposition;
    public GameObject _ReserveOppositionA;
    public GameObject _ReserveOppositionB;
    public GameObject _ReserveOppositionC;

    // Use this for initialization
    void Start() {

    }



    // Update is called once per frame
    void Update() {

    }

    public void SetUnit(UnitType t, PlayerType p) {
        List<GameObject> prefabs = p == PlayerType.Battlebeard ? _BattlebeardUnits : _StormshaperUnits;
        GameObject marker = _MarkerActivePlayerUnits[(int)t];
        GameObject unitObject = _ActivePlayerUnits[(int)t];
        if (unitObject != null) {
            unitObject.SetActive(true);
            return;
        }
        _ActivePlayerUnits[(int)t] = (GameObject)Instantiate(prefabs[(int)t], marker.transform.position, marker.transform.rotation);
    }

    public void RemoveUnit(UnitType t) {
        _ActivePlayerUnits[(int)t].SetActive(false);
    }

    public void SetActiveUnit(UnitType t, PlayerType p) {
		List<GameObject> prefabs = p == PlayerType.Battlebeard ? _BattlebeardUnits : _StormshaperUnits;
        _ActivePlayerUnit = (GameObject)Instantiate(prefabs[(int)t], _MarkerActivePlayerUnit.transform.position, _MarkerActivePlayerUnit.transform.rotation);
	}

	public void SetOpposition(UnitType t, PlayerType p) {
		List<GameObject> prefabs = p == PlayerType.Battlebeard ? _BattlebeardUnits : _StormshaperUnits;
		setOpposition(prefabs[(int)t]);
	}

	public void SetOpposition(MonsterType t) {
		setOpposition(_Monsters[(int)t]);
	}

	void setOpposition(GameObject g) {
	    _ActiveOpposition = (GameObject)Instantiate(g, _MarkerActiveOpposition.transform.position, _MarkerActiveOpposition.transform.rotation);
	}

	public void RemoveOpposition() {
		_ActiveOpposition.SetActive(false);
	}

	public void SetReserveAAsOpposition() {
		SetReserveAsOpposition(_ReserveOppositionA);
	}

	public void SetReserveBAsOpposition() {
		SetReserveAsOpposition(_ReserveOppositionB);
	}

	public void SetReserveCAsOpposition() {
		SetReserveAsOpposition(_ReserveOppositionC);
	}

	public void SetReserveAsOpposition(GameObject reserve) {
		_ActiveOpposition = reserve;
		reserve = null;
		_ActiveOpposition.transform.position = _MarkerActiveOpposition.transform.position;
		_ActiveOpposition.transform.rotation = _MarkerActiveOpposition.transform.rotation;
		_ActiveOpposition.SetActive(true);
	}

	public void SetReserveOppositionA(MonsterType t) {
        GameObject g = _Monsters[(int)t];
        _ReserveOppositionA = (GameObject)Instantiate(g, _MarkerReserveOppositionA.transform.position, _MarkerReserveOppositionA.transform.rotation);
    }
	
	public void SetReserveOppositionB(MonsterType t) {
        GameObject g = _Monsters[(int)t];
        _ReserveOppositionB = (GameObject)Instantiate(g, _MarkerReserveOppositionB.transform.position, _MarkerReserveOppositionB.transform.rotation);
    }

    public void SetReserveOppositionC(MonsterType t) {
        GameObject g = _Monsters[(int)t];
        _ReserveOppositionC = (GameObject)Instantiate(g, _MarkerReserveOppositionC.transform.position, _MarkerReserveOppositionC.transform.rotation);
    }
}
