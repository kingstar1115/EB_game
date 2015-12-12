using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardsData : ScriptableObject
{
    public List<Cards> availableCaveCards;

    void InitCards()
    {
        //Add the available cards to list
        availableCaveCards.Add(Cards.Resource_Card_100);
        availableCaveCards.Add(Cards.Resource_Card_200);
        availableCaveCards.Add(Cards.Resource_Card_300);
        availableCaveCards.Add(Cards.Resource_Card_400);
        availableCaveCards.Add(Cards.Resource_Card_500);
    }
}
