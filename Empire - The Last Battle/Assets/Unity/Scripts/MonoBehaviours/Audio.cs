
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioEventArgs : EventArgs
{
	//Clip to play
	public string ClipName { get; set; }
	///<summary>
	///0-1
	///</summary>
	public float Volume { get; set; }
	public bool Mute { get; set; }
	public bool Loop { get; set; }
	///<summary>
	///0-256, The higher the number means it takes priority over other sounds, default = 130
	///</summary>
	public int Priority 
	{ 
		get 
		{
			return _priority;
		} 
		set 
		{
			if (value >= 256)
				_priority = 256;
		} 
	}

	int _priority;
	
	public AudioEventArgs()
	{
		ClipName = null;
		Volume = 1.0f;
		Mute = false;
		Loop = false;
		Priority = 130;
	}
}

public class Audio : MonoBehaviour
{
	public TurnManager TurnManager;
	public Armoury Armoury;
	public Army Army;
	public CardSystem CardSystem;
	public CommanderUI BattlebeardUI;
	public CommanderUI StormshaperUI;
	public OverworldUI OverworldUI;

	private AudioSource _audioSource;

	private static readonly List<AudioClip> Clips = new List<AudioClip>()
	{
		//Put audio clips here
		Resources.Load<AudioClip>("Audio/SFX/cartoon001"),

	};

	// Use this for initialization
	void Awake()
	{
		_audioSource = GetComponent<AudioSource>();
		AudioHandler ah = new AudioHandler ();
		ah.Init(this, TurnManager, Armoury, Army, CardSystem, BattlebeardUI, StormshaperUI, OverworldUI);
	}

	public void Play(object source, AudioEventArgs e)
	{
		var clip = Clips.First(x => x.name == e.ClipName);
		var volume = e.Volume;
		_audioSource.mute = e.Mute;
		_audioSource.priority = e.Priority;
		_audioSource.loop = e.Loop;
		
		//PlayOneShot allows multiple sounds to be played on one audio source
		_audioSource.PlayOneShot(clip, volume);
	}
}