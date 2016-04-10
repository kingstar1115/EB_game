using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class SpoilsUI : MonoBehaviour {

	public Sprite[] CardSprites;
	public Image Card;
	public Text Value;
	public GameObject UI;

	// Use this for initialization
	void Start () {
	
	}

	public void Show() {
		UI.SetActive(true);
	}

	public void Hide() {
		UI.SetActive(false);
	}

	public void SetCard(CardData card) {
		Card.sprite = CardSprites[(int)card.Type];
		Value.text = card.Value == 0 ? "" : card.Value.ToString();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
