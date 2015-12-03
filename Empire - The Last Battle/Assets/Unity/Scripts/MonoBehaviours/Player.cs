using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public TileData CommanderPosition;
	public PointsSystem Points;
	public PlayerType Type;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
public enum PlayerType
{
	Battlebeard,
	Stormshaper,
	Neutral
}