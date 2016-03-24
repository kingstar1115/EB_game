using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class TextPointFilter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.GetComponent<Text>().font.material.mainTexture.filterMode = FilterMode.Point;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
