using UnityEngine;
using System.Collections;

public class Marker : MonoBehaviour {
    public MarkerType Type;
}

public enum MarkerType {
    Flag,
    CommanderStormshapers,
    CommanderBattlebeards,
    PlayerScouts,
    PlayerPikemen,
    PlayerArchers,
    PlayerAxeThrowers,
    PlayerWarriors,
    PlayerCavalry,
    PlayerCatapult,
    PlayerBallista,
    PlayerSingleUnit,
    ActiveOpposition,
    ReserveOppositionA,
    ReserveOppositionB
}
