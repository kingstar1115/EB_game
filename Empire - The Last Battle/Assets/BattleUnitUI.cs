using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattleUnitUI : MonoBehaviour 
{
	public TextFlash _DamageFlashDisplay;
	public GameObject _HealthBarDisplays;
	public UnitType _UnitType;

	Dictionary<Unit, Slider> _healthBarSliders;
	Pool _healthBarPool;
	List<Unit> _units;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Init(Pool healthBarPool)
	{
		_units = new List<Unit> ();
		_healthBarSliders = new Dictionary<Unit, Slider> ();

		//keep health bar reference
		_healthBarPool = healthBarPool;
	}

	public void OnDisable()
	{
		Debug.Log (this.name + " Disabled");

		//clear all health bars
		foreach (var healthBar in _healthBarSliders.Values) {
			healthBar.transform.transform.SetParent(null);
			healthBar.gameObject.SetActive(false);
		}
	}

	public void ClearUnits ()
	{
		_units.Clear();
	}

	public void AddUnit(Unit unit)
	{
		if (unit == null)
			return;

		//set up event listeners before add
		unit.OnUnitTakeDamage += UnitDamagedHandler;
		unit.OnUpdate += UnitUpdateHandler;

		//init a new health bar 
		GameObject healthBar = _healthBarPool.GetPooledObject ();
		healthBar.SetActive (true);
		healthBar.transform.SetParent (_HealthBarDisplays.transform);
		_healthBarSliders.Add(unit, healthBar.GetComponentInChildren<Slider>());

		//refresh
		UnitUpdateHandler(unit);

		_units.Add (unit);
	}

	public void UnitUpdateHandler(Unit unit)
	{
		//set the health bar
		_healthBarSliders[unit].value = unit.GetHPPercentage();
	}

	public void UnitDamagedHandler(Unit unit, int damage)
	{
		//damage flash 
		TakeDamage (damage);
	}

	public void TakeDamage(int amount)
	{
		_DamageFlashDisplay.FlashDisplayText(amount.ToString());
	}
}
