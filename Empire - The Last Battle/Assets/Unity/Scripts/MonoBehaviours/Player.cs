using UnityEngine;

public class Player : MonoBehaviour
{
    public TileData CommanderPosition;
    public PlayerType Type;
	public PointsSystem Currency;
	public CardData Hand;
	public Army PlayerArmy;
	public int CastleProgress;

    int lostImortalKillCount;
    public int LostImortalKillCount
    {
        get
        {
            return lostImortalKillCount;
        }
    }

    public void Reset()
    {
        lostImortalKillCount = 0;
    }

    public void LostImortalKilled_Increment()
    {
        //limit the lost imortal kill count
        if (lostImortalKillCount + 1 <= 4)
            lostImortalKillCount++;
        else
            Debug.LogError("Trying to kill too many lost imiortals");
    }
}
public enum PlayerType
{
    None,
	Battlebeard,
	Stormshaper
}