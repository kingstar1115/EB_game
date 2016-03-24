using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Animator), typeof(Text))]
public class TextFlash : MonoBehaviour 
{
	public string _FlashTrigger;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void FlashDisplayText(string text)
	{
		this.GetComponent<Text> ().text = text;
		this.GetComponent<Animator> ().SetTrigger (_FlashTrigger);
	}
}
