using UnityEngine;
using System.Collections;

public class UnitUI : MonoBehaviour {

	bool upgrade = false;
	bool ko = false;

	public GameObject UpgradeUI;
	public GameObject KOUI;

	public void SetKO(bool b) {
		ko = b;
		if (b) {
			// show KO UI;
		}
		else {
			// hide KO ui
		}
	}

	public void SetUpgrade(bool b) {
		ko = b;
		if (b) {
			// show upgrade UI;
		}
		else {
			// hide upgrade ui
		}
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
