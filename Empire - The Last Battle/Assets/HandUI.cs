using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LerpRotation))]
public class HandUI : MonoBehaviour 
{
	public Pool m_CardPrefabPool;
    public List<Sprite> m_Sprites;
    public List<CardUI> m_Cards;
    public float m_ZRotSpacing;
    public int m_FocusedCardIndex;
    public LerpRotation m_LerpRotation;

	// Use this for initialization
	void Start () {

		/*
        //init all the car ui
        foreach (var cardUI in m_Cards)
        {
            cardUI.Init();
        }

        m_lerpRotation = this.GetComponent<LerpRotation>();
        UpdateRotations();
        */
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetHand(CardList cardsToShow)
    {
        //trim cards diplaying that are not needed
        if (m_Cards.Count > cardsToShow.cards.Count)
        {
            for (int i = cardsToShow.cards.Count; i < m_Cards.Count; i++)
            {
                m_Cards[i].gameObject.SetActive(false);
            }
        }

        //for each of the showing cards
        for (int i = 0; i < cardsToShow.cards.Count; i++)
        {
            //if i < cards displaying count then set new sprite
            if (i < m_Cards.Count)
                m_Cards[i]._Image.sprite = GetSpriteOfCard(cardsToShow.cards[i].Type);
            else
            {
                //add new card
                AddNewCard(cardsToShow.cards[i]);
            }
        }

        //update spacing 
        UpdateRotations();
    }

	public void AddNewCard(CardData data)
	{
		//create the prefab 
		GameObject newCard = m_CardPrefabPool.GetPooledObject ();
		CardUI cardUI = newCard.GetComponent<CardUI>();
		if (cardUI != null) {
            //init
            newCard.SetActive(true);
            cardUI.Init();
            cardUI._Image.sprite = GetSpriteOfCard(data.Type);
			//add as perent 
			newCard.transform.SetParent(this.transform);
			newCard.transform.localPosition = Vector3.zero;
			//add to the list
			m_Cards.Add(cardUI);
		} else {
			Debug.LogError("No CarUI script on card ui pooled object prefab :O");
		}
	}

    public void UpdateRotations()
    {
        //rotate each card ui
        float totalRotation = 0;
        float prevRotToAdd = 0;
        for (int i = 1; i < m_Cards.Count; i++)
        {
            prevRotToAdd = (i != m_FocusedCardIndex + 1) ? m_ZRotSpacing : (m_ZRotSpacing * 2);
            //Debug.Log("RotToAdd["+i+"] "+prevRotToAdd);
            m_Cards[i]._TargetRotation = Quaternion.Euler(0, 0, prevRotToAdd + totalRotation);
            //Debug.Log("Target " + m_Cards[i]._TargetRotation);
            totalRotation += prevRotToAdd;
        }

        //rotate the hand ui minus half total
        m_LerpRotation.LerpTo(Quaternion.Euler(0,0,-totalRotation/2));
    }

    public Sprite GetSpriteOfCard(CardType type)
    {
        return m_Sprites[(int)type];
    }
}
