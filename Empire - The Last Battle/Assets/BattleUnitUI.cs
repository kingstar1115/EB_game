using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattleUnitUI : MonoBehaviour 
{
	public TextFlash _DamageFlashDisplay;
	public GameObject _HealthBarDisplays;
	public UnitType _UnitType;

	Dictionary<int, Slider> _healthBarSliders;
	Pool _healthBarPool;
	List<iBattleable> _units;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Init(Pool healthBarPool)
	{
        _units = new List<iBattleable>();
		_healthBarSliders = new Dictionary<int, Slider> ();

		//keep health bar reference
		_healthBarPool = healthBarPool;
	}

	public void OnDisable()
	{
		//clear all health bars
        if (_healthBarSliders != null)
        {
            foreach (var healthBar in _healthBarSliders.Values)
            {
                healthBar.transform.transform.SetParent(null);
                healthBar.gameObject.SetActive(false);
            }
        }
	}

	public void ClearUnits ()
	{
		_units.Clear();
	}

    public void AddUnit(iBattleable unit)
	{
		if (unit == null)
			return;

        BattleManager.OnBattleAbleUpdate += BattleManager_OnBattleAbleUpdate;
        BattleManager.OnBattleAbleTakeDamage+=BattleManager_OnBattleAbleTakeDamage;

		//init a new health bar 
		GameObject healthBar = _healthBarPool.GetPooledObject ();
		healthBar.SetActive (true);
		healthBar.transform.SetParent (_HealthBarDisplays.transform);
        healthBar.transform.localScale = new Vector3(1, 1, 1);
		_healthBarSliders.Add(unit.GetHashCode(), healthBar.GetComponentInChildren<Slider>());

		//refresh
        BattleManager_OnBattleAbleUpdate(unit);

		_units.Add (unit);
	}

    void BattleManager_OnBattleAbleTakeDamage(iBattleable battleAbleObject, int val)
    {
		//check that the battlable thing taking damage is associated with this ui 
		if (_units.Contains (battleAbleObject)) 
		{
			//damage flash 
			TakeDamage(val);
		}
        
    }

    void BattleManager_OnBattleAbleUpdate(iBattleable battleAbleObject)
    {
		if(_healthBarSliders.ContainsKey(battleAbleObject.GetUniqueID()))
        	_healthBarSliders[battleAbleObject.GetUniqueID()].value = battleAbleObject.GetHPPercentage();
    }

	public void TakeDamage(int amount)
	{
		_DamageFlashDisplay.FlashDisplayText(amount.ToString());
	}
}
