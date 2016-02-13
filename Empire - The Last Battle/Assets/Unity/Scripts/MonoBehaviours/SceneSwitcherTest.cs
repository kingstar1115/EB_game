using UnityEngine;
using System.Collections;

public class SceneSwitcherTest : MonoBehaviour
{

	public int scenes;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (scenes > 0)
		{
			for (int i = 0; i < scenes; i++)
			{
				if (Input.GetKeyDown(i.ToString()))
				{
					Application.LoadLevel(i);
				}
			}
		}
	}
}