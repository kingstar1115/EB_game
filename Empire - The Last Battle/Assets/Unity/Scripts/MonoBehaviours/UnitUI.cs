using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public delegate void UIUnitIndexCallback(int i);
public class UnitUI : MonoBehaviour {
	public Image KO;
	public Image Icon;
	public Image Upgrade;
	public Button Button;
	public UIUnitIndexCallback OnClick = delegate { };
	int index;
	bool isKO = false;
	public bool IsKO {
		get { return isKO; }
	}

	public void SetKO(bool b) {
		isKO = b;
		KO.gameObject.SetActive(b);
	}

	public void SetIndex(int i) {
		index = i;
	}

	public void SetUpgrade(bool b) {
		Upgrade.gameObject.SetActive(b);
	}

	public void SetImage(Sprite s) {
		Icon.sprite = s;
	}

	public void EnableSelection() {
		
		Button.interactable = true;
	}

	public void DisableSelection() {
		Button.interactable = false;
	}

	// Use this for initialization
	void Start () {
		Button.onClick.AddListener(() => {
			OnClick(index);
		});
	}

	void Reset() {
		Button.onClick.RemoveAllListeners();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
