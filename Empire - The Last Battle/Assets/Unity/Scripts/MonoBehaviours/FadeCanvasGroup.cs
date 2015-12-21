using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
// Just does width at the moment
public class FadeCanvasGroup : MonoBehaviour {
	public float LerpSpeed = 0.4f;

	CanvasGroup canvasGroup;
	float lerpFromAlpha;
	float lerpToAlpha;
	bool isLerping;
	float lerpTime;

	public float GetCurrentPercent() {
		return lerpTime / LerpSpeed;
	}

	public void FadeTo(float alpha) {
		isLerping = true;
		lerpToAlpha = alpha;
		lerpFromAlpha = getCanvasGroup().alpha;
	}

	CanvasGroup getCanvasGroup() {
		if (canvasGroup == null) {
			canvasGroup = gameObject.GetComponent<CanvasGroup>();
		}
		return canvasGroup;
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
			float currentAlpha = 0;
			if (lerpTime > LerpSpeed) {
				currentAlpha = lerpToAlpha;
				ResetLerp();
			} else {
				currentAlpha = Mathf.Lerp(lerpFromAlpha, lerpToAlpha, GetCurrentPercent());
			}
			getCanvasGroup().alpha = currentAlpha;
		}
	}
}
