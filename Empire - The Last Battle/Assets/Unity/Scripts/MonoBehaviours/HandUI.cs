using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LerpRotation), typeof(LerpPosition))]
public class HandUI : MonoBehaviour 
{
    public event System.Action OnHandSet = delegate { };
    public event System.Action OnCardSelect = delegate { };
    public event System.Action OnCardDeselect = delegate { };

    public Transform _HandInPos;
    public Transform _HandOutPos;
	public Pool m_CardPrefabPool;
    public List<Sprite> m_Sprites;
    public List<CardUI> m_Cards;
    public float m_ZRotSpacing;
    public LerpRotation m_LerpRotation;
    public CardUI m_SelectedCardUI;
    public bool m_CardsSelectable;
    public int m_focusedCardIndex;
	public int m_NumberOfCards;

	bool _enabled;
	public bool _Enabled
	{
		get { return _enabled; }
		set { _enabled = value; }
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void RemoveListeners() {
		OnHandSet = delegate { };
		OnCardSelect = delegate { };
		OnCardDeselect = delegate { };
	}

    public void Show()
    {
        //lerp to out pos
        GetComponent<LerpPosition>().LerpTo(_HandOutPos.position);
    }

    public void Hide()
    {
        //lerp to in pos
        GetComponent<LerpPosition>().LerpTo(_HandInPos.position);
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
            {
                m_Cards[i]._Image.sprite = GetSpriteOfCard(cardsToShow.cards[i].Type);
                m_Cards[i]._Index = i;
                m_Cards[i]._Card = cardsToShow.cards[i];
                m_Cards[i].gameObject.SetActive(true);
            }
            else
            {
                //add new card
                AddNewCard(cardsToShow.cards[i]);
            }
        }
		m_NumberOfCards = cardsToShow.cards.Count;
        //update spacing 
        UpdateRotations();

        //event 
        OnHandSet();
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
            cardUI._Value = data.Value;
            cardUI._Index = m_Cards.Count;
            cardUI._Card = data;

            //events
            cardUI.OnPointerEnter += cardUI_OnPointerEnter;
            cardUI.OnPointerExit += cardUI_OnPointerExit;
            cardUI.OnPointerUp += cardUI_OnPointerUP;

			//add as perent 
			newCard.transform.SetParent(this.transform);
			newCard.transform.localPosition = Vector3.zero;
            newCard.transform.localScale = new Vector3(1, 1, 1);

			//add to the list
			m_Cards.Add(cardUI);
		} else {
			Debug.LogError("No CarUI script on card ui pooled object prefab :O");
		}
	}

    void cardUI_OnPointerUP(CardUI cardUI)
    {
		//only if enabled
		if (_enabled) 
		{
			//pop the card in or out
			if (cardUI == m_SelectedCardUI) {
				DeselectCard (cardUI._Index);
			} else {
				SelectCard (cardUI._Index);
			}
		}
    }

    public void SelectCard(int index)
    {
        if (m_SelectedCardUI != null)
            m_SelectedCardUI.PopDown();

        m_SelectedCardUI = m_Cards[index];
        m_Cards[index].Popup();

        //event
        OnCardSelect();
    }

    public void DeselectCard(int index)
    {
        m_Cards[index].PopDown();
        m_SelectedCardUI = null;
        OnCardDeselect();
    }
	public void DeselectCardNoPopdown(int index) {
		m_SelectedCardUI = null;
		OnCardDeselect();
	}

    void cardUI_OnPointerExit(CardUI cardUI)
    {
		//only if enabled
		if (_enabled) 
		{
			//set the focus index to non (-1)
			SetFocusIndex (-1);
		}
    }

    void cardUI_OnPointerEnter(CardUI cardUI)
    {
		//only if enabled
		if (_enabled) 
		{
			//set to apropriate index
			SetFocusIndex (cardUI._Index);
		}
    }

    public void SetFocusIndex(int index)
    {
        //set to apropriate index
        m_focusedCardIndex = index;

        //refresh the rotations
        UpdateRotations();
    }

    public void UpdateRotations()
    {
        //rotate each card ui
        float totalRotation = 0;
        float prevRotToAdd = 0;
        for (int i = 1; i < m_Cards.Count; i++)
        {
            if (m_Cards[i].isActiveAndEnabled)
            {
                prevRotToAdd = ((!m_CardsSelectable || m_focusedCardIndex == -1) || i != m_focusedCardIndex + 1) ? m_ZRotSpacing : (m_ZRotSpacing * 2);
                //Debug.Log("RotToAdd["+i+"] "+prevRotToAdd);
                m_Cards[i]._TargetRotation = Quaternion.Euler(0, 0, prevRotToAdd + totalRotation);
                //Debug.Log("Target " + m_Cards[i]._TargetRotation);
                totalRotation += prevRotToAdd;
            }
        }

        //rotate the hand ui minus half total
        m_LerpRotation.LerpTo(Quaternion.Euler(0,0,-totalRotation/2));
    }

    public Sprite GetSpriteOfCard(CardType type)
    {
        return m_Sprites[(int)type];
    }

    public void DeselectCurrent()
    {
        if (m_SelectedCardUI!=null)
            DeselectCard(m_SelectedCardUI._Index);
    }
}
