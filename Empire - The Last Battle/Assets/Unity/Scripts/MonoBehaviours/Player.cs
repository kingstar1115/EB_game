using UnityEngine;

public class Player : MonoBehaviour
{
    public Tile commanderPosition;
    public PointsSystem currency;
    public PlayerType type;
	public Hand playersHand;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetPosition()
    {

    }
}
public enum PlayerType
{
    Battlebeard,
    Stormshaper,
    Neutral
}