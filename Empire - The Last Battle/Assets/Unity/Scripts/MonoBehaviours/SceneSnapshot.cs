using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PreviousScene {
	public static Texture2D screenshot;
}

public class SceneSnapshot : MonoBehaviour {

	public RawImage image;

	static SceneSnapshot _sceneSnapShot;
	public static SceneSnapshot ScreenSnapshot
	{
		get
		{
			return _sceneSnapShot;
		}
	}

	void Awake(){
		_sceneSnapShot = this;
	}

	public void SnapScreenShot(){
		if (PreviousScene.screenshot != null) {
			image.texture = PreviousScene.screenshot;
		}
	}
}
