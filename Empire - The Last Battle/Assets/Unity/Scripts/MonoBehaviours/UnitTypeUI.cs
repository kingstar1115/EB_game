using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public enum UnitSelection {
	None = 0,
	Active = 1,
	Inactive = 2,
	Upgraded = 4,
	NotUpgraded = 8,
	TempUpgraded = 16,
	NotTempUpgraded = 32,
}

public delegate void UIUnitTypeIndexCallback(UnitType t, int i);
public class UnitTypeUI : MonoBehaviour {

	// Components
	public MouseOverItem Mouse;
	public Image Icon;
	public GameObject UnitsPanel;
	public GameObject UnitList;
	public GameObject UnitOverview;
	public RectTransform PanelSlider;
	RectTransform _unitsPanelTransform;
	CanvasGroup _unitListCanvas;
	CanvasGroup _unitOverviewCanvas;

	public UIUnitTypeIndexCallback OnClickUnit = delegate { };

	// Overview
	public Color OverviewColour;

	// Size
	public float MaxWidth = 820;
	public float MinWidth = 90;
	public float SmallIconSize = 60f;
	public float UnitSpacing = 10;
	public float LargeIconSize = 100f;
	public float UnitOverviewSpacing = 7f;

	UnitType _unitType;
	List<UnitUI> _units;
	bool _selectMode;

	// Lerp Stuff
	public float Speed = 0.4f; // speed that the maximise/minimise takes
	float lerpTimeMinMax;
	float lerpTimeHideShow;
	bool isMinMaxLerping;
	bool isHideShowLerping;
	float lerpToAlpha;
	float lerpFromAlpha;
	float lerpToWidth;
	float lerpFromWidth;
	float lerpFromPos;
	float lerpToPos;

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

	public void RemoveListeners() {
		OnClickUnit = delegate { };
		foreach (UnitUI u in _units) {
			u.RemoveListeners();
		}
	}

	public bool IsMouseOver() {
		return Mouse.isOver;
	}

	public void Initialise(UnitType t) {
		_unitsPanelTransform = UnitsPanel.GetComponent<RectTransform>();
		_unitListCanvas = UnitList.GetComponent<CanvasGroup>();
		_unitOverviewCanvas = UnitOverview.GetComponent<CanvasGroup>();
		_unitType = t;
		_units = new List<UnitUI>();
		Icon.sprite = getSprite(t);
		Hide(true);
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
		ui.SetIndex(_units.Count);
		ui.SetUpgrade(u.HasUpgrade());
		ui.OnClick += _clickUnit;
		if (_selectMode) { ui.EnableSelection(); }
		_units.Add(ui);
		GameObject g = new GameObject();
		Image image = g.AddComponent<Image>();
		g.transform.SetParent(UnitOverview.transform);
		g.transform.localScale = Vector3.one;
		if (u.IsKO()) {
			image.color = Color.clear;
		} else {
			image.color = OverviewColour;
		}
	}

	public void RemoveUnit(int i) {
		GameObject o = _units[i].gameObject;
		_units.RemoveAt(i);
		Destroy(o);
		Destroy(UnitOverview.transform.GetChild(i).gameObject);
	}

	public void Reset() {
		_units.ForEach(unit => Destroy(unit.gameObject));
		_units.Clear();
		// Remove unit overview children
	}

	public void UpdateUnit(int i, Unit u) {
		UnitUI ui = _units[i];
		ui.SetKO(u.IsKO());
		ui.SetUpgrade(u.HasUpgrade());
		if (u.IsKO()) {
			UnitOverview.transform.GetChild(i).GetComponent<Image>().color = Color.clear;
		} else {
			UnitOverview.transform.GetChild(i).GetComponent<Image>().color = OverviewColour;
		}
	}

	public void Hide() {
		Minimise();
		lerpToPos = (MinWidth + UnitOverviewSpacing) * -1;
		lerpFromPos = PanelSlider.offsetMin.x;
		isHideShowLerping = true;
	}

	public void Hide(bool forced) {
		if (forced) {
			PanelSlider.offsetMin = new Vector2((MinWidth + UnitOverviewSpacing) * -1, PanelSlider.offsetMin.y);
		} else {
			Hide();
		}
	}

	public void Show() {
		lerpToPos = 0;
		lerpFromPos = PanelSlider.offsetMin.x;		
		isHideShowLerping = true;
	}

	public void Show(bool forced) {
		if (forced) {
			PanelSlider.offsetMin = new Vector2(0, PanelSlider.offsetMin.y);
		} else {
			Show();
		}
	}

	public void Maximise() {
		// Resize item
		float width = _units.Count * (SmallIconSize + UnitSpacing) + LargeIconSize + UnitSpacing;
		if (width > MaxWidth) { width = MaxWidth; }
		lerpFromWidth = _unitsPanelTransform.sizeDelta.x;
		lerpToWidth = width;

		// fade in
		lerpToAlpha = 1;
		lerpFromAlpha = _unitListCanvas.alpha;

		isMinMaxLerping = true;
	}

	public void Maximise(bool forced) {
		if (forced) {
			float width = _units.Count * (SmallIconSize + UnitSpacing) + LargeIconSize + UnitSpacing;
			if (width > MaxWidth) { width = MaxWidth; }
			_unitListCanvas.alpha = 1;
			_unitOverviewCanvas.alpha = 0f;
			_unitsPanelTransform.sizeDelta = new Vector2(width, _unitsPanelTransform.rect.height);
			UnitList.SetActive(true);
			UnitOverview.SetActive(false);
		} else {
			Maximise();
		}
	}

	public void Minimise() {
		// resize item
		lerpFromWidth = _unitsPanelTransform.sizeDelta.x;
		lerpToWidth = MinWidth;

		// fade out -- -1 looks a lot nicer than 0
		lerpToAlpha = -1;
		lerpFromAlpha = _unitListCanvas.alpha;

		isMinMaxLerping = true;
	}

	public void Minimise(bool forced) {
		if (forced) {
			_unitListCanvas.alpha = -1;
			_unitOverviewCanvas.alpha = 1f;
			_unitsPanelTransform.sizeDelta = new Vector2(MinWidth, _unitsPanelTransform.rect.height);
			UnitList.SetActive(false);
			UnitOverview.SetActive(true);
		} else {
			Minimise();
		}
	}

	// pass flags - 
	public void MakeSelectable(UnitSelection flags) {
		bool active = (flags & UnitSelection.Active) == UnitSelection.Active,
			 inactive = (flags & UnitSelection.Inactive) == UnitSelection.Inactive,
			 upgraded = (flags & UnitSelection.Upgraded) == UnitSelection.Upgraded,
			 notUpgraded = (flags & UnitSelection.NotUpgraded) == UnitSelection.NotUpgraded,
			 tempUpgraded = (flags & UnitSelection.TempUpgraded) == UnitSelection.TempUpgraded,
			 notTempUpgraded = (flags & UnitSelection.NotTempUpgraded) == UnitSelection.NotTempUpgraded;

		_units.ForEach(ui => {
			if (inactive && ui.IsKO || 
				active && !ui.IsKO || 
				upgraded && ui.IsUpgraded || 
				notUpgraded && !ui.IsUpgraded ||
				tempUpgraded && ui.isTempUpgraded ||
				notTempUpgraded && !ui.isTempUpgraded) {

				ui.EnableSelection();
			}
		});
		_selectMode = true;
	}

	public void MakeUnselectable() {
		_units.ForEach(ui => ui.DisableSelection());
		_selectMode = false;
	}

	void _clickUnit(int i) {
		OnClickUnit(_unitType, i);
	}

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		if (isMinMaxLerping) {
			lerpTimeMinMax += Time.deltaTime;
			float currentAlpha = 0;
			float currentWidth = 0;
			if (lerpTimeMinMax > Speed) {
				currentAlpha = lerpToAlpha;
				currentWidth = lerpToWidth;
			}
			else {
				if (lerpFromAlpha < lerpToAlpha) {
					UnitList.SetActive(true);
					UnitOverview.SetActive(false);
				}
				currentAlpha = Mathf.Lerp(lerpFromAlpha, lerpToAlpha, lerpTimeMinMax / Speed);
				currentWidth = Mathf.Lerp(lerpFromWidth, lerpToWidth, lerpTimeMinMax / Speed);
			}
			_unitListCanvas.alpha = currentAlpha;
			_unitOverviewCanvas.alpha = 1f - currentAlpha;
			_unitsPanelTransform.sizeDelta = new Vector2(currentWidth, _unitsPanelTransform.rect.height);

			if (lerpToAlpha == currentAlpha) {
				if (lerpToAlpha < lerpFromAlpha) {
					UnitList.SetActive(false);
					UnitOverview.SetActive(true);
				}
				isMinMaxLerping = false;
				lerpTimeMinMax = 0;
			}
		}

		if (isHideShowLerping) {
			lerpTimeHideShow += Time.deltaTime;
			float currentPos = 0;
			if (lerpTimeHideShow > Speed) {
				currentPos = lerpToPos;
			} else {
				currentPos = Mathf.Lerp(lerpFromPos, lerpToPos, lerpTimeHideShow / Speed);
			}
			PanelSlider.offsetMin = new Vector2(currentPos, PanelSlider.offsetMin.y);
			if (lerpToPos == currentPos) {
				isHideShowLerping = false;
				lerpTimeHideShow = 0;
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
