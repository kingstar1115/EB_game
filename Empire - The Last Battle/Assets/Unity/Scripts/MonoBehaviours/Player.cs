using UnityEngine;

public class Player : MonoBehaviour
{
    public TileData CommanderPosition;
    public PlayerType Type;
	public PointsSystem Currency;
	public CardData Hand;
	public Army PlayerArmy;

	public int CastleProgress;

    public void SetPosition()
    {
    
    }
}
public enum PlayerType
{
    None,
	Battlebeard,
	Stormshaper
}