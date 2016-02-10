
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum SoundsEnum
{
	Catoonz,
}

public class Audio : MonoBehaviour
{
	private static Audio _audio;
	public static Audio AudioInstance
	{
		get
		{
			return _audio;
		}
	}
	public List<AudioClip> AudioClips;
	/// <summary>
	/// Use 0 for SFX, 1 for Music
	/// </summary>
	public AudioSource[] AudioSources = new AudioSource[2];

	private Audio()
	{

	}

	void Awake()
	{	
		_audio = this;
	}

	void Update()
	{
		Debug.Log("Dtes");
	}

	public void ChangeOptions(AudioSource source, bool loop, int priority = 125, float volume = 1f, bool mute = false)
	{
		source.volume = volume;
		source.mute = mute;
		source.priority = priority;
		source.loop = loop;
	}

	public void PlayOnce(SoundsEnum sound, int priority = 125, float volume = 1f, bool mute = false)
	{
		var sfxToPlay = AudioClips[(int)sound];
		AudioSources[0].volume = volume;
		AudioSources[0].mute = mute;
		AudioSources[0].priority = priority;

		//PlayOneShot allows multiple sounds to be played on one audio source
		AudioSources[0].PlayOneShot(sfxToPlay, AudioSources[0].volume);
	}

	public void PlayLooped(SoundsEnum sound, int priority = 125, float volume = 1f, bool mute = false, bool loop = true)
	{
		AudioSources[1].volume = volume;
		AudioSources[1].mute = mute;
		AudioSources[1].priority = priority;
		AudioSources[1].loop = loop;

		AudioSources[1].Play();
	}
}