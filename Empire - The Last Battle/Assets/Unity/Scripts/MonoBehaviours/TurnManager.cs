using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour {

    public event System.Action OnTurnStart = delegate { };
    public event System.Action OnTurnEnd = delegate { };
    public event System.Action OnSwitchTurn = delegate { };

    public void Initialise() {

    }

    public void StartTurn() {
        OnTurnStart();
    }

    public void EndTurn(){
        OnTurnEnd();
    }

    public void SwitchTurn(){
        OnSwitchTurn();
    }
}
