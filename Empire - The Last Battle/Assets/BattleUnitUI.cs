using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleUnitUI : MonoBehaviour 
{
	public TextFlash _DamageFlashDisplay;
	public Slider _HealthBarSlider;

	Unit _unit;
	public Unit _Unit
	{
		get{return _unit;}
		set
		{
			if(value!=null)
			{
				//ste event listeners
				value.OnUnitTakeDamage += UnitDamagedHandler;
				value.OnUpdate += UnitUpdateHandler;

                UnitUpdateHandler(value);
			}

			_unit = value;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UnitUpdateHandler(Unit unit)
	{
		//set the health bar
		_HealthBarSlider.value = unit.GetHPPercentage();
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
