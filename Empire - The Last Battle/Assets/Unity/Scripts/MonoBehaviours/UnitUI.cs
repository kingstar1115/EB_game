using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitUI : MonoBehaviour {
	public Image KO;
	public Image Icon;
	public Image Upgrade;
	public Image Background;

	public void SetKO(bool b) {
		KO.gameObject.SetActive(b);
	}

	public void SetUpgrade(bool b) {
		Upgrade.gameObject.SetActive(b);
	}

	public void SetImage(Sprite s) {
		Icon.sprite = s;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
