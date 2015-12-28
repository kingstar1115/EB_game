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
	public Button OkButton;
	public Button CancelButton;
	public GameObject ModalPanelObject;

	private static ModalPanel modalPanel;

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
		hide();
	}

	void setupButton(Button b, UnityAction a) {
		b.gameObject.SetActive(true);
		if (a != null) {
			b.onClick.AddListener(a);
		}
		b.onClick.AddListener(hide);
	}

	public void ShowOKCancel(string title, string content, UnityAction onOk, UnityAction onCancel) {
		ModalPanelObject.SetActive(true);
		setTitle(title);
		setContent(content);
		setupButton(OkButton, onOk);
		setupButton(CancelButton, onCancel);
	}

	public void ShowOK(string title, string content, UnityAction onOk) {
		ModalPanelObject.SetActive(true);
		setTitle(title);
		setContent(content);
		setupButton(OkButton, onOk);
	}
	public void ShowCancel(string title, string content, UnityAction onCancel) {
		ModalPanelObject.SetActive(true);
		setTitle(title);
		setContent(content);
		setupButton(CancelButton, onCancel);
	}

	void hide() {
		OkButton.onClick.RemoveAllListeners();
		CancelButton.onClick.RemoveAllListeners();
		OkButton.gameObject.SetActive(false);
		CancelButton.gameObject.SetActive(false);
		ModalPanelObject.SetActive(false);
	}

	void setContent(string content) {
		ContentForeground.text = content;
		ContentBackground.text = content + '\n';
	}

	void setTitle(string title) {
		TitleForeground.text = title;
		TitleBackground.text = title + '\n';
	}
	
}