using UnityEngine;
using System.Collections;

public class OverworldUI : MonoBehaviour 
{
    public CommanderUI _CommanderUI;

	// Use this for initialization
	void Start () 
    {
        _CommanderUI.OnDraggingCommander += _CommanderUI_OnDraggingCommander;	
	}

    void _CommanderUI_OnDraggingCommander()
    {
        throw new System.NotImplementedException();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
