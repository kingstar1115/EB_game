using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class ModalPanel : MonoBehaviour {
	// Hacky shit here. Do not look.
	public Text TitleForeground;
	public Text TitleBackground;
	public Text ContentForeground;
	public Text ContentBackground;
	public GameObject OkPanel;
	public GameObject CancelPanel;
	public GameObject ModalPanelObject;
	Button okButton;
	Button cancelButton;
	static ModalPanel modalPanel;

	public static ModalPanel Instance() {
		if (!modalPanel) {
			modalPanel = FindObjectOfType<ModalPanel>();
			if (!modalPanel) {
				Debug.LogError("No ModalWindow script attatched to any gameobject in the scene");
			}
		}
		return modalPanel;
	}

	// Use this for initialization
	void Start () {
		okButton = OkPanel.GetComponentInChildren<Button>();
		cancelButton = CancelPanel.GetComponentInChildren<Button>();
		hide();
	}

	public static void RemoveListeners() {
		modalPanel.okButton.onClick.RemoveAllListeners();
		modalPanel.cancelButton.onClick.RemoveAllListeners();
	}

	void setupButton(Button b, GameObject p, UnityAction a) {
		p.SetActive(true);
		b.onClick.RemoveAllListeners();
		if (a != null) {
			b.onClick.AddListener(a);
		}
		b.onClick.AddListener(hide);
	}

	public void ShowOKCancel(string title, string content, UnityAction onOk, UnityAction onCancel) {
		ModalPanelObject.SetActive(true);
		setTitle(title);
		setContent(content);
		setupButton(okButton, OkPanel, onOk);
		setupButton(cancelButton, CancelPanel, onCancel);
	}

	public void ShowOK(string title, string content, UnityAction onOk) {
		ModalPanelObject.SetActive(true);
		setTitle(title);
		setContent(content);
		setupButton(okButton, OkPanel, onOk);
	}
	public void ShowCancel(string title, string content, UnityAction onCancel) {
		ModalPanelObject.SetActive(true);
		setTitle(title);
		setContent(content);
		setupButton(cancelButton, CancelPanel, onCancel);
	}

	void hide() {
		okButton.onClick.RemoveAllListeners();
		cancelButton.onClick.RemoveAllListeners();
		OkPanel.SetActive(false);
		CancelPanel.SetActive(false);
		ModalPanelObject.SetActive(false);
	}

	void setContent(string content) {
		ContentForeground.text = content;
		ContentBackground.text = content;
	}

	void setTitle(string title) {
		TitleForeground.text = title;
		TitleBackground.text = title;
	}
	
}