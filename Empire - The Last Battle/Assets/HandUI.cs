using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LerpRotation))]
public class HandUI : MonoBehaviour 
{
	public Pool m_CardPrefabPool;
    public List<CardUI> m_Cards;
    public float m_ZRotSpacing;
    public int m_focusedCardIndex;
    LerpRotation m_lerpRotation;

	public void Init(List<CardData> cardsToAdd)
	{
		//add new cards
		foreach (var card in cardsToAdd) {
			AddNewCard(card);
		}

		//update spacing 
		UpdateRotations ();
	}

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

	public void AddNewCard(CardData data)
	{
		//create the prefab 
		GameObject newCard = m_CardPrefabPool.GetPooledObject ();
		CardUI cardUI = newCard.GetComponent<CardUI>();
		if (cardUI != null) {
			//add as perent 
			newCard.transform.parent = this.transform;
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
            prevRotToAdd = (i != m_focusedCardIndex + 1) ? m_ZRotSpacing : (m_ZRotSpacing * 2);
            Debug.Log("RotToAdd["+i+"] "+prevRotToAdd);
            m_Cards[i]._TargetRotation = Quaternion.Euler(0, 0, prevRotToAdd + totalRotation);
            Debug.Log("Target " + m_Cards[i]._TargetRotation);
            totalRotation += prevRotToAdd;
        }

        //rotate the hand ui minus half total
        m_lerpRotation.LerpTo(Quaternion.Euler(0,0,-totalRotation/2));
    }
}
