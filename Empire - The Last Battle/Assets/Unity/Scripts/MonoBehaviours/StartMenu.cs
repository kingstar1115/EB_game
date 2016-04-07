using UnityEngine;
using System.Collections;

public class StartMenu : MonoBehaviour {


	public GameStateHolder _GameStateHolder;

	void Start() {
		_GameStateHolder._gameState = GameState.MainMenu;
	}

	public void HandlePlayGameClick(){
		StartCoroutine(SceneSwitcher.ChangeScene ("Overworld"));
	}

	public void HandleExitGameClick(){
		Application.Quit ();
	}
}
