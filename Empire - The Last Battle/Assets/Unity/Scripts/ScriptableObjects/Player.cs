using UnityEngine;

public delegate void PlayerUnitCallback(Player p, Unit u);
public delegate void PlayerUnitIndexCallback(Player p, Unit u, int i);
public delegate void PlayerCastleProgressCallback(PlayerType p, int i);

public class Player : ScriptableObject
{
    //card events
	public delegate void CardAction(CardData cardData);
	public delegate void CardIntAction(CardData cardData, int index);

    public TileData PreviousCommanderPosition;
    public PlayerType Type;
	public PointsSystem Currency = new PointsSystem();
	public CardList Hand;
	public Army PlayerArmy;
	public bool IsScouting;

	public event PlayerUnitCallback OnUpdateUnit = delegate { };
	public event PlayerUnitCallback OnAddUnit = delegate { };
	public event PlayerUnitIndexCallback OnRemoveUnit = delegate { };
	public event PlayerCastleProgressCallback OnCastleProgress = delegate { };
	public event CardAction OnCardAdded = delegate { };
	public event CardIntAction OnCardRemoved = delegate { };

	int castleProgress;
	public int CastleProgress {
		get {
			return castleProgress;
		}
		set {
			if (value <= 4 && value >= 0) {
				castleProgress = value;
				OnCastleProgress(this.Type, value);
			}
			else {
				Debug.LogError("Trying to get too many castles");
			}
		}
	}
	int lostImmortalKillCount;
    public int LostImmortalKillCount {
        get {
            return lostImmortalKillCount;
        }
		set {
			if (value <= 4 && value >= 0) {
				lostImmortalKillCount = value;
			}
			else {
				Debug.LogError("Trying to kill too many Lost Immortals");
			}
		}
    }

    TileData commanderPosition;
    public TileData CommanderPosition
    {
        get { return commanderPosition; }
        set
        {
			if (commanderPosition != value) {
				PreviousCommanderPosition = commanderPosition;
				commanderPosition = value;
			}
        }
    }

	public void Initialise() {
		PlayerArmy.Initialise();
        PlayerArmy.OnUpdateUnit += PlayerArmy_OnUpdateUnit;
        PlayerArmy.OnRemoveUnit += PlayerArmy_OnRemoveUnit;
        PlayerArmy.OnAddUnit += PlayerArmy_OnAddUnit;
	}

    public void ResetArmy() {
        PlayerArmy.Reset();
    }

    private void PlayerArmy_OnAddUnit(Unit u)
    {
        OnAddUnit(this, u);
    }

    private void PlayerArmy_OnRemoveUnit(Unit u, int i)
    {
        OnRemoveUnit(this, u, i);
    }

    private void PlayerArmy_OnUpdateUnit(Unit u)
    {
        OnUpdateUnit(this, u);
    }

    public void Reset()
    {
        lostImmortalKillCount = 0;
		CastleProgress = 0;
    }

	public void RemoveListeners() {
		OnUpdateUnit = delegate { };
		OnAddUnit = delegate { };
		OnRemoveUnit = delegate { };
		OnCardAdded = delegate { };
		OnCardRemoved = delegate { };
		OnCastleProgress = delegate { };
		PlayerArmy.RemoveListeners();
		Currency.RemoveListeners ();
	}

	public void AddCard(CardData cardToAdd)
	{
		//update hand 
		Hand.cards.Add (cardToAdd);
		//event for adding card
		OnCardAdded (cardToAdd);

	}

    public CardData GetCardOfType(CardType t) {
        return Hand.cards.Find(x => x.Type == t);
    }

	public void RemoveCard(CardData cardToRemove) {
		int index = Hand.cards.IndexOf(cardToRemove);
		RemoveCard(index);	
	}

	public void RemoveCard(int index) {
		//update hand and trigger event
		CardData card = Hand.cards[index];
		if (card != null) {
			Hand.cards.RemoveAt(index);
			OnCardRemoved(card, index);
		}
		else {
			Debug.LogError("trying to remove card that doesnt exist :O");
		}
	}

	public void SetCards(CardList newCards)
	{
		Hand.cards = new System.Collections.Generic.List<CardData> (newCards.cards);
	}
}

public enum PlayerType
{
    None,
	Battlebeard,
	Stormshaper
}