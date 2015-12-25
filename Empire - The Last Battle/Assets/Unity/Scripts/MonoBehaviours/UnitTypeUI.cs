using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UnitTypeUI : MonoBehaviour {

	// Components
	public MouseOverItem Mouse;
	public Image Icon;
	public GameObject UnitsPanel;
	public GameObject UnitList;
	RectTransform _unitsPanelTransform;
	CanvasGroup _unitListCanvas;

	// Size
	public float MaxWidth = 820;
	public float MinWidth = 110;
	public float SmallIconSize = 70f;
	public float UnitSpacing = 10;
	public float LargeIconSize = 100f;

	UnitType _unitType;
	List<UnitUI> _units;

	// Lerp Stuff
	public float Speed = 0.4f; // speed that the maximise/minimise takes
	float lerpTime;
	bool isLerping;
	float lerpToAlpha;
	float lerpFromAlpha;
	float lerpToWidth;
	float lerpFromWidth;

	// Individual unit prefab
	public GameObject UnitPrefab;

	// Sprites
	public Sprite ScoutSprite;
	public Sprite PikemanSprite;
	public Sprite AxeThrowerSprite;
	public Sprite WarriorSprite;
	public Sprite ArcherSprite;
	public Sprite CavalrySprite;
	public Sprite BallistaSprite;
	public Sprite CatapultSprite;

	Sprite getSprite(UnitType t) {
		switch (t) {
			case UnitType.Archer:
				return ArcherSprite;
			case UnitType.AxeThrower:
				return AxeThrowerSprite;
			case UnitType.Ballista:
				return BallistaSprite;
			case UnitType.Catapult:
				return CatapultSprite;
			case UnitType.Cavalry:
				return CavalrySprite;
			case UnitType.Pikeman:
				return PikemanSprite;
			case UnitType.Scout:
				return ScoutSprite;
			case UnitType.Warrior:
				return WarriorSprite;
			default:
				return null;
		}
	}

	public bool IsMouseOver() {
		return Mouse.isOver;
	}

	public void Initialise(UnitType t) {
		_unitsPanelTransform = UnitsPanel.GetComponent<RectTransform>();
		_unitListCanvas = UnitList.GetComponent<CanvasGroup>();
		_unitType = t;
		_units = new List<UnitUI>();
		Icon.sprite = getSprite(t);
	}

	new public UnitType GetType() {
		return _unitType;
	}

	public void AddUnit(Unit u) {
		GameObject obj = Instantiate(UnitPrefab);
		obj.transform.SetParent(UnitList.transform);
		obj.transform.localScale = Vector3.one;
		UnitUI ui = obj.GetComponent<UnitUI>();
		ui.SetImage(getSprite(u.Type));
		ui.SetKO(u.IsKO());
		ui.SetUpgrade(u.HasUpgrade());
		_units.Add(ui);
	}

	public void RemoveUnit(int i) {
		GameObject o = _units[i].gameObject;
		_units.RemoveAt(i);
		Destroy(o);
	}

	public void Reset() {
		_units.ForEach(unit => Destroy(unit.gameObject));
		_units.Clear();
	}

	public void UpdateUnit(int i, Unit u) {
		UnitUI ui = _units[i].GetComponent<UnitUI>();
		ui.SetKO(u.IsKO());
		ui.SetUpgrade(u.HasUpgrade());
	}

	public void Maximise() {
		// Resize item
		float width = _units.Count * (SmallIconSize + UnitSpacing) + LargeIconSize + UnitSpacing;
		if (width > MaxWidth) { width = MaxWidth; }
		lerpFromWidth = MinWidth;
		lerpToWidth = width;

		// fade in
		lerpToAlpha = 1;
		lerpFromAlpha = 0;

		isLerping = true;
	}

	public void Minimise() {
		// resize item
		lerpFromWidth = _unitsPanelTransform.sizeDelta.x;
		lerpToWidth = MinWidth;

		// fade out
		lerpToAlpha = 0;
		lerpFromAlpha = 1;

		isLerping = true;
	}

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		if (isLerping) {
			lerpTime += Time.deltaTime;
			float currentAlpha = 0;
			float currentWidth = 0;
			if (lerpTime > Speed) {
				currentAlpha = lerpToAlpha;
				currentWidth = lerpToWidth;
			}
			else {
				if (lerpFromAlpha == 0) {
					UnitList.SetActive(true);
				}
				currentAlpha = Mathf.Lerp(lerpFromAlpha, lerpToAlpha, lerpTime / Speed);
				currentWidth = Mathf.Lerp(lerpFromWidth, lerpToWidth, lerpTime / Speed);
			}
			_unitListCanvas.alpha = currentAlpha;
			_unitsPanelTransform.sizeDelta = new Vector2(currentWidth, _unitsPanelTransform.rect.height);

			if (lerpToAlpha == currentAlpha) {
				if (lerpToAlpha == 0) {
					UnitList.SetActive(false);
				}
				isLerping = false;
				lerpTime = 0;
			}
		}
	}
}

public class CompareUnitUI : IComparer<UnitTypeUI> {
	public int Compare(UnitTypeUI x, UnitTypeUI y) {
		if (x == null) return -1;
		if (y == null) return 1;
		return x.GetType().CompareTo(y.GetType());
	}
}
