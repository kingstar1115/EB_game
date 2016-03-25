using UnityEngine;
using System.Collections.Generic;

public class DefendingUnitManager : MonoBehaviour {

	public GameObject BBDefendingObject;
	public GameObject BBJailedObject;

	public GameObject SSDefendingObject;
	public GameObject SSJailedObject;

	public GameObject CageObject;

	Dictionary<TileData, DefenderData> defenderLookup;
	Dictionary<TileData, PrisonerData> prisonerLookup;
	Dictionary<TileData, CageData> cageLookup;

	public void Initialise () {
		defenderLookup = new Dictionary<TileData, DefenderData>();
		prisonerLookup = new Dictionary<TileData, PrisonerData>();
		cageLookup = new Dictionary<TileData, CageData>();
	}

	public void SetDefenderForTile(TileData t) {
		DefenderData defenderData;
		defenderLookup.TryGetValue(t, out defenderData);

		if (defenderData == null) {
			defenderData = new DefenderData();
			defenderData.Marker = Utils.GetFirstChildWithTag("MarkerDefending", t.TileObject);
			defenderLookup.Add(t, defenderData);
		}

		if (defenderData.Marker) {
			defenderData.CurrentDefender = setDefenderObject(t, defenderData);
			defenderData.CurrentType = t.Owner;
		}
	}

	GameObject setDefenderObject(TileData t, DefenderData dd) {

		if (t.Owner == PlayerType.None) {
			if (dd.CurrentDefender != null) {
				Destroy(dd.CurrentDefender.gameObject);
				dd.CurrentDefender = null;
			}
			return null;
		} else {
			if (dd.CurrentDefender != null) {
				if (dd.CurrentType == t.Owner) {
					return dd.CurrentDefender;
				}
				else {
					Destroy(dd.CurrentDefender.gameObject);
					dd.CurrentDefender = null;
				}
			}
			GameObject defenderToSet = t.Owner == PlayerType.Battlebeard ? BBDefendingObject : SSDefendingObject;
			return (GameObject)Instantiate(defenderToSet, dd.Marker.transform.position, dd.Marker.transform.rotation);
		}   
	}

	public void SetPrisonerForTile(TileData t) {
		PrisonerData prisonerData;
		prisonerLookup.TryGetValue(t, out prisonerData);

		if (prisonerData == null ) {
			prisonerData = new PrisonerData();
			prisonerData.Marker = Utils.GetFirstChildWithTag("MarkerJail", t.TileObject);
			prisonerLookup.Add(t, prisonerData);
		}

		if (prisonerData.Marker) {
			prisonerData.CurrentPrisoner = setPrisonerObject(t, prisonerData);
			prisonerData.CurrentType = t.Owner;
		}
	}

	GameObject setPrisonerObject(TileData t, PrisonerData pd) {

		if (t.Owner == PlayerType.None) {
			if (pd.CurrentPrisoner != null) {
				Destroy(pd.CurrentPrisoner.gameObject);
				pd.CurrentPrisoner = null;
			}
			return null;
		} else {
			if (pd.CurrentPrisoner != null) {
				if (pd.CurrentType == t.Owner) {
					return pd.CurrentPrisoner;
				}
				else {
					Destroy(pd.CurrentPrisoner.gameObject);
					pd.CurrentPrisoner = null;
				}
			}
			GameObject cage = Utils.GetFirstChildWithTag ("Cage", t.TileObject);
			if (cage != null) { 
				MeshRenderer m = cage.GetComponent<MeshRenderer>();
				m.enabled = true;
			}
			GameObject prisonerToSet = t.Owner == PlayerType.Battlebeard ? BBJailedObject : SSJailedObject;
			return (GameObject)Instantiate(prisonerToSet, pd.Marker.transform.position, pd.Marker.transform.rotation);
		}   
	}

	public void SetCageForTile(TileData t) {
		CageData cageData;
		cageLookup.TryGetValue(t, out cageData);

		if (cageData == null) {
			cageData = new CageData();
			cageData.Marker = Utils.GetFirstChildWithTag("MarkerCage", t.TileObject);
			cageLookup.Add(t, cageData);
		}

		if(cageData.Marker) {
			cageData.CageObject = setCageObject(t, cageData);
		}
	}

	GameObject setCageObject(TileData t, CageData cd) {

		if (t.Owner == PlayerType.None) {
			return null;
		} else {
			return (GameObject)Instantiate(CageObject, cd.Marker.transform.position, cd.Marker.transform.rotation);
		}   
	}

	public class DefenderData {
		public GameObject Marker;
		public GameObject CurrentDefender;
		public PlayerType CurrentType;
	}

	public class PrisonerData {
		public GameObject Marker;
		public GameObject CurrentPrisoner;
		public PlayerType CurrentType;
	}

	public class CageData {
		public GameObject Marker;
		public GameObject CageObject;
	}
}
