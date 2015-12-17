using UnityEngine;

public class Player : MonoBehaviour
{
	public delegate void CardAction(CardData cardData);
	public event CardAction OnCardAdded = delegate {};
	public event CardAction OnCardRemoved = delegate {};

    public TileData CommanderPosition;
    public PlayerType Type;
	public PointsSystem Currency;
	public CardList Hand;
	public Army PlayerArmy;
	public int CastleProgress;
	public bool IsScouting;

    int lostImmortalKillCount;
    public int LostImmortalKillCount
    {
        get
        {
            return lostImmortalKillCount;
        }
    }

	public void Initialise() {
		PlayerArmy.Initialise();
		Currency = new PointsSystem();
	}

    public void Reset()
    {
        lostImmortalKillCount = 0;
    }

    public void LostImmortalKilled_Increment()
    {
        //limit the lost immortal kill count
        if (lostImmortalKillCount < 3)
            lostImmortalKillCount++;
        else
            Debug.LogError("Trying to kill too many Lost Immortals");
    }

	public void AddCard(CardData cardToAdd)
	{
		//update hand 
		Hand.cards.Add (cardToAdd);

		//event for adding card
		OnCardAdded (cardToAdd);
	}

	public void RemoveCard(CardData cardToRemove)
	{
		//update hand and trigger event
		if (Hand.cards.Remove (cardToRemove))
			OnCardRemoved (cardToRemove);
		else
			Debug.LogError ("trying ot remove card that doesnt exist :O");
		
	}
}

public enum PlayerType
{
    None,
	Battlebeard,
	Stormshaper
}