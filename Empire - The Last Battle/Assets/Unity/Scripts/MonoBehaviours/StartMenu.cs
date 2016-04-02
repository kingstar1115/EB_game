using UnityEngine;
using System.Collections;

public class StartMenu : MonoBehaviour {

	public void HandlePlayGameClick(){
		StartCoroutine(SceneSwitcher.ChangeScene ("Overworld"));
	}

	public void HandleExitGameClick(){
		Application.Quit ();
	}
}
