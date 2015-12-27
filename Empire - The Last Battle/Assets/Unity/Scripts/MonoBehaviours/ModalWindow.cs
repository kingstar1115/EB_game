using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ModalWindow : MonoBehaviour {

	public Text Title;
	public Text Content;
	public RectTransform TransparentBackground;
	public RectTransform MainPanel;
	public GameObject ButtonPanel;

	// Use this for initialization
	void Start () {
		SetContent("HELLO THIS IS LOTS OF LINES \nHELLO THIS IS LOTS OF LINES \nHELLO THIS IS LOTS OF LINES \nHELLO THIS IS LOTS OF LINES \nHELLO THIS IS LOTS OF LINES \nHELLO THIS IS LOTS OF LINES \nHELLO THIS IS LOTS OF LINES \n");
	}

	public void SetContent(string content) {
		Content.text = content;
		Debug.Log(MainPanel.sizeDelta);
		TransparentBackground.sizeDelta = MainPanel.sizeDelta + new Vector2(20, 20);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
