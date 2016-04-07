using UnityEngine;
using System.Collections;

public class StartMenu : MonoBehaviour {
	public MusicTrack[] Tracks;
	public SoundEffect Button_Click;
	public GameStateHolder _GameStateHolder;

	//Moorland Kevin MacLeod (incompetech.com)
	//Licensed under Creative Commons: By Attribution 3.0 License
		//http://creativecommons.org/licenses/by/3.0/

	//Royalty Free SFX from www.audioblocks.com https://www.audioblocks.com/stock-audio/whoosh-low-synth.html

	//Royalty Free Music from Bensound http://www.bensound.com/royalty-free-music/track/epic

	void Start(){
		Audio.AudioInstance.PlayMusic(Tracks, true);
		_GameStateHolder._gameState = GameState.MainMenu;
	}

	public void HandlePlayGameClick(){
		Audio.AudioInstance.PlaySFX (Button_Click);
		StartCoroutine(SceneSwitcher.ChangeScene ("Overworld"));
	}

	public void HandleExitGameClick(){
		Application.Quit ();
	}
}
