using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class CardList : ScriptableObject {
	public List<CardData> cards;

	public CardData GetCardOfType(CardType type)
	{
		foreach (var card in cards) {
			if(card.Type == type)
				return card;
		}

		return null;
	}
}
