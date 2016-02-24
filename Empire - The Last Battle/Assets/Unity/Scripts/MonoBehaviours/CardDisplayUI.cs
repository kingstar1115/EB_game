using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class CardDisplayUI : MonoBehaviour 
{
    public delegate void CardAction(CardData cardData);
    public event CardAction OnCardUse = delegate { };

    public CanvasGroup _CGLeftButton;
    public CanvasGroup _CGRightButton;
	public Button _UseButton;
    public HandUI _HandUI;
    public Text _LeftValDis;
    public Text _CenterValDis1;
    public Image _LeftCard;
    public Image _RightCard;
    public Image _CentreCard;
    public Color _LandR_Colour;
    CanvasGroup _canvasGroup;
    int _currentCentreIndex;

    bool _isShowing;
    public bool _IsShowing
    {
        get { return _isShowing; }
    }

	// Use this for initialization
	void Start () 
    {
        
	}

	public void RemoveListeners() {
		OnCardUse = delegate { };		
		_UseButton.onClick.RemoveListener(UseSelectedCardHandler);
	}

	// Update is called once per frame
	void Update () 
    {
	    //put a wheel scroll thing????? Maybe.
	}

    public void Show()
    {
        //make visible 
		Enable();
        _canvasGroup.alpha = 1;
        _isShowing = true;
    }

    public void Hide()
    {
        //make invisible
		Disable();
        _canvasGroup.alpha = 0;
        _isShowing = false;
    }

	public void Disable()
	{
		_canvasGroup.interactable = false;
		_canvasGroup.blocksRaycasts = false;
	}

	public void Enable()
	{
		_canvasGroup.interactable = true;
		_canvasGroup.blocksRaycasts = true;
	}

    public void Init()
    {
        //grab canvas group
        _canvasGroup = this.GetComponent<CanvasGroup>();

        //events
		SetListeners ();	
			
			//hide
        Hide();

        //------TEST-------
        //if(_HandUI.m_Cards.Count > 0)
         //   _HandUI.SelectCard(0);
    }

	public void SetListeners()
	{
		//events
		_HandUI.OnCardSelect += _HandUI_OnCardSelect;
		_HandUI.OnHandSet += _HandUI_OnHandSet;
		_HandUI.OnCardDeselect += _HandUI_OnCardDeselect;
		
		_UseButton.onClick.AddListener(UseSelectedCardHandler);	
	}
	
	void _HandUI_OnCardDeselect()
    {
        Hide();

		//deselect
		_HandUI.DeselectCurrent();
    }

    void _HandUI_OnHandSet()
    {
        //if there is a card selected then use its index, if not then use index 0
        int index = (_HandUI.m_SelectedCardUI != null) ? _HandUI.m_SelectedCardUI._Index : 0;
        int r_Index = (index == _HandUI.m_NumberOfCards - 1) ? -1 : index + 1;
        int l_Index = (index == 0) ? -1 : index - 1;
		if (_HandUI.m_NumberOfCards == 0) {
			return;
		}

        //focused sprite
        _CentreCard.sprite = _HandUI.GetSpriteOfCard(_HandUI.m_Cards[index]._Card.Type);
        _CenterValDis1.text = (_HandUI.m_Cards[index]._Card.Value > 0) ? _HandUI.m_Cards[index]._Card.Value.ToString() : "";

        //Hide the sideimages if they dont represent a card.
        if (r_Index != -1)
        {
            _RightCard.sprite = _HandUI.GetSpriteOfCard(_HandUI.m_Cards[r_Index]._Card.Type);
            _RightCard.color = _LandR_Colour;

            //if not already set visiable
            if (_CGRightButton.alpha == 0)
            {
                _CGRightButton.alpha = 1;
                _CGRightButton.interactable = true;
                _CGRightButton.blocksRaycasts = true;
            }
        }
        else
        {
            _CGRightButton.alpha = 0.0f;
            _CGRightButton.interactable = false;
            _CGRightButton.blocksRaycasts = false;
			_RightCard.color = new Color(0, 0, 0, 0);
        }

        if (l_Index != -1)
        {
            _LeftCard.sprite = _HandUI.GetSpriteOfCard(_HandUI.m_Cards[l_Index]._Card.Type);
            _LeftCard.color = _LandR_Colour;
            _LeftValDis.text = (_HandUI.m_Cards[l_Index]._Card.Value > 0) ? _HandUI.m_Cards[l_Index]._Card.Value.ToString() : "";

            //if not already set visiable
            if (_CGLeftButton.alpha == 0)
            {
                _CGLeftButton.alpha = 1;
                _CGLeftButton.interactable = true;
                _CGLeftButton.blocksRaycasts = true;
            }
        }
        else
        {
            _CGLeftButton.alpha = 0.0f;
            _CGLeftButton.interactable = false;
            _CGLeftButton.blocksRaycasts = false;
            _LeftCard.color = new Color(0, 0, 0, 0);
			_LeftValDis.text = "";
        }

        //remenber selected index
        _currentCentreIndex = index;
    }

    void _HandUI_OnCardSelect()
    {
        //swich the card sprites so that selected is front.
        _HandUI_OnHandSet();

        //show
        if (!_isShowing)
            Show();
    }

    public void UseSelectedCardHandler()
    {
        //event!
        OnCardUse(_HandUI.m_SelectedCardUI._Card);
		_HandUI.DeselectCurrent();
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
		if (_currentCentreIndex < _HandUI.m_NumberOfCards - 1)
            _HandUI.SelectCard(_currentCentreIndex + 1);
    }

    public void OnBackClickedHandler()
    {
        //hide 
        Hide();

		//deselect
		_HandUI.DeselectCurrent();
    }

}
