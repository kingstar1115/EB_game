using UnityEngine;

public class Player : MonoBehaviour
{
    public TileData CommanderPosition;
    public PlayerType Type;
	public PointsSystem Currency;
	public CardData Hand;
	public Army PlayerArmy;
	public int CastleProgress;

    int lostImmortalKillCount;
    public int LostImmortalKillCount
    {
        get
        {
            return lostImmortalKillCount;
        }
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
}
public enum PlayerType
{
    None,
	Battlebeard,
	Stormshaper
}