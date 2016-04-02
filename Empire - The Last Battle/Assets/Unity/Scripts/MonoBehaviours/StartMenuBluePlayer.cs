using UnityEngine;
using System.Collections;

public class StartMenuBluePlayer : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		var rnd = UnityEngine.Random.Range (0, 2);
		if (rnd == 0) 
		{
			gameObject.SetActive(false);
		}
	}
}
