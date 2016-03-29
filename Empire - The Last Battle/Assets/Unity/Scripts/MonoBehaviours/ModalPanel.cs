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
	public Button OkButton;
	public Button CancelButton;
	static ModalPanel modalPanel;

	public static ModalPanel Instance() {
		if (!modalPanel) {
			modalPanel = FindObjectOfType<ModalPanel>();
			if (!modalPanel) {
				Debug.LogError("No ModalPanel script attatched to any gameobject in the scene");
			}
		}
		return modalPanel;
	}

	// Use this for initialization
	void Awake () {
		hide();
	}

	public static void RemoveListeners() {
		if(modalPanel != null) {
			modalPanel.OkButton.onClick.RemoveAllListeners();
			modalPanel.CancelButton.onClick.RemoveAllListeners();
			modalPanel = null;
		}
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
		setupButton(OkButton, OkPanel, onOk);
		setupButton(CancelButton, CancelPanel, onCancel);
	}

	public void ShowOK(string title, string content, UnityAction onOk) {
		ModalPanelObject.SetActive(true);
		setTitle(title);
		setContent(content);
		setupButton(OkButton, OkPanel, onOk);
	}
	public void ShowCancel(string title, string content, UnityAction onCancel) {
		ModalPanelObject.SetActive(true);
		setTitle(title);
		setContent(content);
		setupButton(CancelButton, CancelPanel, onCancel);
	}

	void hide() {
		if (OkButton !=null) {
			OkButton.onClick.RemoveAllListeners();
		}
		if (CancelButton != null) {
			CancelButton.onClick.RemoveAllListeners();
		}
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