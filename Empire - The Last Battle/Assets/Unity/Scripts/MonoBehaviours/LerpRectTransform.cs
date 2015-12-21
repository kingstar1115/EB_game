using UnityEngine;

[RequireComponent(typeof(RectTransform))]
// Just does width at the moment
public class LerpRectTransform : MonoBehaviour {
	public float LerpSpeed = 0.4f;

	RectTransform rectTransform;
	float lerpFromWidth;
	float lerpToWidth;
	bool isLerping;
	float lerpTime;

	public float GetCurrentPercent() {
		return lerpTime / LerpSpeed;
	}

	public void ResizeWidth(float width) {
		isLerping = true;
		lerpToWidth = width;
		lerpFromWidth = getRectTransform().rect.width;
	}

	RectTransform getRectTransform() {
		if (rectTransform == null) {
			rectTransform = gameObject.GetComponent<RectTransform>();
		}
		return rectTransform;
	}

	public bool IsLerping() {
		return isLerping;
	}

	public void ResetLerp() {
		isLerping = false;
		lerpTime = 0;
	}

	// Update is called once per frame
	void Update() {
		if (isLerping) {
			lerpTime += Time.deltaTime;
			float currentWidth = 0;
			if (lerpTime > LerpSpeed) {
				currentWidth = lerpToWidth;
				ResetLerp();
			} else {
				currentWidth = Mathf.Lerp(lerpFromWidth, lerpToWidth, GetCurrentPercent());
			}
			getRectTransform().sizeDelta = new Vector2(currentWidth, getRectTransform().rect.height);
		}
	}
}
