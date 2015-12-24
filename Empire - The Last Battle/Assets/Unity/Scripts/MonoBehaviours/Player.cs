using UnityEngine;

public delegate void PlayerUnitCallback(Player p, Unit u);
public delegate void PlayerUnitIndexCallback(Player p, Unit u, int i);

public class Player : MonoBehaviour
{
	TileData commanderPosition;
	public TileData CommanderPosition {
		get { return commanderPosition; }
		set {
			PreviousCommanderPosition = commanderPosition;
			commanderPosition = value;
		}
	}
	public TileData PreviousCommanderPosition;
	public PlayerType Type;
	public PointsSystem Currency;
	public CardList Hand;
	public Army PlayerArmy;
	public int CastleProgress;
	public bool IsScouting;

	public event PlayerUnitCallback OnUpdateUnit = delegate { };
	public event PlayerUnitCallback OnAddUnit = delegate { };
	public event PlayerUnitIndexCallback OnRemoveUnit = delegate { };


	int lostImmortalKillCount;
    public int LostImmortalKillCount {
        get {
            return lostImmortalKillCount;
        }
    }

	public void Initialise() {
		PlayerArmy.Initialise();
		Currency = new PointsSystem();
		PlayerArmy.OnUpdateUnit += PlayerArmy_OnUpdateUnit;
		PlayerArmy.OnRemoveUnit += PlayerArmy_OnRemoveUnit;
		PlayerArmy.OnAddUnit += PlayerArmy_OnAddUnit;
	}

	private void PlayerArmy_OnAddUnit(Unit u) {
		OnAddUnit(this, u);
	}

	private void PlayerArmy_OnRemoveUnit(Unit u, int i) {
		OnRemoveUnit(this, u, i);
	}

	private void PlayerArmy_OnUpdateUnit(Unit u) {
		OnUpdateUnit(this, u);
	}

	public void Reset() {
        lostImmortalKillCount = 0;
    }

    public void LostImmortalKilled_Increment() {
        //limit the lost immortal kill count
        if (lostImmortalKillCount < 3)
            lostImmortalKillCount++;
        else
            Debug.LogError("Trying to kill too many Lost Immortals");
    }
}
public enum PlayerType {
    None,
	Battlebeard,
	Stormshaper
}