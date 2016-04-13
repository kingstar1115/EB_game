
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SoundEffect {
	Charge,
	Dead,
	Hit1,
	Hit2,
	Inn,
	Roar1,
	Roar2,
	Sword,
	// new sounds
	LostImmortal_Cry,
	Dragon_Attack,
	Dragon_Cry,
	Dragon_Hurt,
	Minotaur_Cry,
	Minotaur_Hurt,
	Minotaur_Dead,
	Battle_Lost,
	Battle_Won,
	Player_Attack,
	Spoils_Collected,
	// UI
	Commander_Land,
	Commander_Pickup,
	Button_Click,
	Button_Hover,
	Card_Shuffle,
	Play_Game
}

public enum MusicTrack {
	Dungeon,
	Mountain,
	Peaceful,
	Town,
	Epic,
	Moorland
}

public class Audio : MonoBehaviour
{
	private const int SFX = 0;
	private const int MUSIC = 1;
	private static Audio _audio;
	public static Audio AudioInstance
	{
		get
		{
			return _audio;
		}
	}
	public List<AudioClip> AudioClips; // sfx
	public List<AudioClip> MusicTracks; // background music
	/// <summary>
	/// Use 0 for SFX, 1 for Music
	/// </summary>
	public AudioSource[] AudioSources;

	// playlist info
	MusicTrack[] playlist;
	int currentTrack;
	bool shuffle;


	void Awake()
	{	
		if (!_audio) {
			_audio = this;
			DontDestroyOnLoad(_audio);
		}
	}

	public void ChangeSFXOptions(float volume = 1f, bool mute = false, int priority = 125) {
		changeOptions(SFX, volume, mute, priority, false);
	}

	public void ChangeMusicOptions(float volume = 1f, bool mute = false, int priority = 125) {
		changeOptions(MUSIC, volume, mute, priority, true);
	}

	void changeOptions(int source, float volume = 1f, bool mute = false, int priority = 125, bool loop = false) {
		AudioSources[source].volume = volume;
		AudioSources[source].mute = mute;
		AudioSources[source].priority = priority;
		AudioSources[source].loop = loop;
	}

	public void PlaySFX(SoundEffect sound)
	{
		var sfxToPlay = AudioClips[(int)sound];

		//PlayOneShot allows multiple sounds to be played on one audio source
		AudioSources[0].PlayOneShot(sfxToPlay);
	}

	public void PlayMusic(MusicTrack sound, float delay = 0)
	{
		StopMusic();
		ClearPlaylist();
		Debug.Log("Playing: " + sound);
		AudioSources[MUSIC].clip = MusicTracks[(int)sound];
		AudioSources[MUSIC].PlayDelayed(delay);
	}

	public void PlayMusic(MusicTrack[] tracks, bool shuffle = false) {
		StopMusic();
		ClearPlaylist();
		playlist = tracks;
		this.shuffle = shuffle;

		playNext(true);
	}

	void playNext(bool start = false) {
		int nextTrack;
		if (shuffle) {
			nextTrack = getRandomTrack(start ? -1 : currentTrack);
		} else {
			nextTrack = (currentTrack + 1) % playlist.Length;
		}
		Debug.Log("Playing: " + playlist[nextTrack]);
		AudioSources[MUSIC].clip = MusicTracks[(int)playlist[nextTrack]];
		currentTrack = nextTrack;
		AudioSources[MUSIC].Play();
	}

	int getRandomTrack(int except = -1) {
		int track = UnityEngine.Random.Range(0, playlist.Length);
		if(except > -1 && track == except) {
			while(except == track) {
				track = UnityEngine.Random.Range(0, playlist.Length);
			}
		}
		return track;
	}

	void Update() {
		// check for the end of a song in the playlist
		if (playlist != null && playlist.Length > 0) {
			int currentTrackSamples = MusicTracks[(int)playlist[currentTrack]].samples;
			if(AudioSources[MUSIC].timeSamples == currentTrackSamples) {
				playNext();
				Debug.Log("playnext called");
			}
		}
	}

	public void StopMusic() {
		AudioSources[MUSIC].Stop();
	}

	public void ClearPlaylist() {
		playlist = null;
		currentTrack = 0;
		shuffle = false;
	}

	public void PauseMusic() {
		AudioSources[MUSIC].Pause();
	}

	public void UnPauseMusic() {
		AudioSources[MUSIC].UnPause();
	}
}