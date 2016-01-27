using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardDisplayUI : MonoBehaviour 
{
    public delegate void CardAction(CardData cardData);
    public event CardAction OnCardUse = delegate { };

    public HandUI _HandUI;
    public Image _LeftCard;
    public Image _RightCard;
    public Image _CentreCard;
    public Color _LandR_Colour;
    int _currentCentreIndex;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	    //put a wheel scroll thing????? Maybe.
	}

    public void Enable()
    {
        //make visible 
    }

    public void Disable()
    {
        //make invisible
    }

    public void Init()
    {
        _HandUI.OnCardSelect += _HandUI_OnCardSelect;
        _HandUI.OnHandSet += _HandUI_OnHandSet;

        //------TEST-------
        if(_HandUI.m_Cards.Count > 0)
            _HandUI.SelectCard(0);
    }

    void _HandUI_OnHandSet()
    {
        //if there is a card selected then use its index, if not then use index 0
        int index = (_HandUI.m_SelectedCardUI != null) ? _HandUI.m_SelectedCardUI._Index : 0;
        int r_Index = (index == _HandUI.m_Cards.Count - 1) ? -1 : index + 1;
        int l_Index = (index == 0) ? -1 : index - 1;

        //focused sprite
        _CentreCard.sprite = _HandUI.GetSpriteOfCard(_HandUI.m_Cards[index]._Card.Type);

        //Hide the sideimages if they dont represent a card.
        if (r_Index != -1)
        {
            _RightCard.sprite = _HandUI.GetSpriteOfCard(_HandUI.m_Cards[r_Index]._Card.Type);
            _RightCard.color = _LandR_Colour;
        }
        else
            _RightCard.color = new Color(0, 0, 0, 0);

        if (l_Index != -1)
        {
            _LeftCard.sprite = _HandUI.GetSpriteOfCard(_HandUI.m_Cards[l_Index]._Card.Type);
            _LeftCard.color = _LandR_Colour;
        }
        else
            _LeftCard.color = new Color(0, 0, 0, 0);

        //remenber selected index
        _currentCentreIndex = index;
    }

    void _HandUI_OnCardSelect()
    {
        //swich the card sprites so that selected is front.
        _HandUI_OnHandSet();
    }

    public void UseSelectedCardHandler()
    {
        //event!
        OnCardUse(_HandUI.m_SelectedCardUI._Card);
    }

    public void OnScrollLeft()
    {
        //select next card left
        if (_currentCentreIndex > 0)
            _HandUI.SelectCard(_currentCentreIndex - 1);
    }

    public void OnScrollRight()
    {
        //select next card right
        if (_currentCentreIndex < _HandUI.m_Cards.Count - 1)
            _HandUI.SelectCard(_currentCentreIndex + 1);
    }

}
