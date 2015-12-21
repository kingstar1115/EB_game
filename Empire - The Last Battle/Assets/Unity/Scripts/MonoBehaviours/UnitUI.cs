using UnityEngine;
using UnityEngine.UI;
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

	public void SetImage(Sprite s) {
		GetComponentInChildren<Image>().gameObject.GetComponentsInChildren<Image>()[1].sprite = s;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
