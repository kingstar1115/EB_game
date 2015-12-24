using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitUI : MonoBehaviour {
	public Image KO;
	public Image Icon;
	public Image Upgrade;
	public Image Background;

	public void SetKO(bool b) {
		var c = KO.color;
		c.a = b ? 255 : 0;
		KO.color = c;
	}

	public void SetUpgrade(bool b) {
		var c = Upgrade.color;
		c.a = b ? 255 : 0;
		Upgrade.color = c;
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
