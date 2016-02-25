using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardData : ScriptableObject
{
	public CardType Type;	
	[EnumFlagAttribute]
	public GameState UseableGameState;
	public int Value;
	public string Name;
}
