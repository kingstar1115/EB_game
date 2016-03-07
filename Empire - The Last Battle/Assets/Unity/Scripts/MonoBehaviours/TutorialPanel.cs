using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

class TutorialData {
	public TutorialData() {
		titles = new List<string>();
		contents = new List<string>();
	}
	public void AddData(string title, string content, bool setIndex) {
		titles.Add(title);
		contents.Add(content);
		if (setIndex) {
			index = titles.Count - 1;
		}
	}
	public string GetContent() {
		return contents[index];
	}
	public string GetTitle() {
		return contents[index];
	}
	public void Next() {
		if(HasNext()) {
			index++;
			if(index > highestSeenIndex) {
				highestSeenIndex = index;
			}
		}
	}
	public void Previous() {
		if(HasPrevious()) { index--; }
	}
	public bool HasNext() {
		return index < titles.Count - 1;
	}
	public bool HasPrevious() {
		return index > 0;
	}
	public bool hasSeenNext() {
		return index + 1 <= highestSeenIndex;
	}
	public bool NeverShow;
	int index;
	int highestSeenIndex;
	List<string> titles;
	List<string> contents;
}

public class TutorialPanel : MonoBehaviour {
	// Hacky shit here. Do not look.
	public Text Title;
	public Text Content;
	public Image CommanderImage;
	public Button ContinueButton;
	public Button BackButton;
	public Button ForwardButton;
	public Toggle HideToggle;
	public GameObject TutorialPanelObject;
	public Transform In;
	public Transform Out;
	public Sprite BattlebeardImage;
	public Sprite StormshaperImage;
	public LerpPosition Lerp;
	static TutorialPanel tutorialPanel;
	static TutorialData battlebeardData;
	static TutorialData stormshaperData;
	TutorialData currentData;

	bool showing;
	PlayerType currentPlayer;

	public static TutorialPanel Instance() {
		if (!tutorialPanel) {
			tutorialPanel = FindObjectOfType<TutorialPanel>();
			if (!tutorialPanel) {
				Debug.LogError("No TutorialPanel script attatched to any gameobject in the scene");
			}
		}
		return tutorialPanel;
	}

	// Use this for initialization
	void Start () {
		battlebeardData = new TutorialData();
		stormshaperData = new TutorialData();
		addListeners();
	}

	void addListeners() {
		Instance().BackButton.onClick.AddListener(previous);
		Instance().ForwardButton.onClick.AddListener(next);
		Instance().ContinueButton.onClick.AddListener(proceed);
	}

	public static void RemoveListeners() {
		Instance().ForwardButton.onClick.RemoveAllListeners();
		Instance().BackButton.onClick.RemoveAllListeners();
		Instance().ContinueButton.onClick.RemoveAllListeners();
	}

	public void Tutor(PlayerType p, string title, string content, bool jump) {
		show(p);
		currentData.AddData(title, content, jump);
		setData();
	}

	void setData() {
		Title.text = currentData.GetTitle();
		Content.text = currentData.GetContent();
		checkButtons();
	}

	void checkButtons() {
		ForwardButton.interactable = currentData.HasNext();
		BackButton.interactable = currentData.HasPrevious();
	}

	void show(PlayerType p) {
		if (showing || p == PlayerType.None) { return; }
		if (p != currentPlayer) {
			currentData = (p == PlayerType.Battlebeard ? battlebeardData : stormshaperData);
			CommanderImage.sprite = (p == PlayerType.Battlebeard ? BattlebeardImage : StormshaperImage);
		}
		if(currentData.NeverShow) { return; }
		HideToggle.isOn = false;
		showing = true;
		Lerp.LerpTo(In.position);
	}

	void previous() {
		currentData.Previous();
		setData();
	}

	void next() {
		currentData.Next();
		setData();
	}

	void proceed() {
		if (currentData.HasNext() && !currentData.hasSeenNext()) {
			next();
		}
		else {
			currentData.NeverShow = HideToggle.isOn;
			Hide();
		}
	}

	public void Disable() {
		
	}

	public void Enable() {

	}

	public void Show() {
		show(currentPlayer);
	}

	public void Hide() {
		showing = false;
		Lerp.LerpTo(Out.position);
	}

	
}