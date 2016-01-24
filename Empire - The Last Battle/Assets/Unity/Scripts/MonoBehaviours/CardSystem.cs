using System;
using System.Collections.Generic;
using UnityEngine;

public enum CardType {
	Healing_Card,
	Resource_Card,
	Battle_Card,
	Tactic_Card,
	Alliance_Card,
	Scout_Card,
	Priority_Card,
	Upgrade_Card
}

public class CardSystem : MonoBehaviour {	

	public CardList cardList;
    // Units availiable from using alliance card at different castle levels.
    public List<UnitType> allianceCardLevel0;
	public List<UnitType> allianceCardLevel1;
	public List<UnitType> allianceCardLevel2;
	public List<UnitType> allianceCardLevel3;
	public List<UnitType> allianceCardLevel4;

	public delegate void CardCallback(CardData card, Player player);
	public event CardCallback OnEffectApplied = delegate { };
	public event CardCallback OnHealingCardUsed = delegate { };
    public event CardCallback OnCardUseFailed = delegate { };
	public event CardCallback OnTacticCardUsed = delegate { };
	public event CardCallback OnUpgradeCardUsed = delegate { };

	public void Start(){

	}

	public void ApplyEffect(CardData card, Player player) {
		switch (card.Type) {
		case CardType.Healing_Card:
			RegisterCardHeal(card, player);
			break;
		case CardType.Resource_Card:
			UseResourceCard(card, player);
			OnEffectApplied (card, player);
			break;
		case CardType.Battle_Card:
			UseBattleCard(player);
			OnEffectApplied (card, player);
			break;
		case CardType.Tactic_Card:
			UseTacticCard(card, player);
			OnEffectApplied (card, player);
			break;
		case CardType.Alliance_Card:
			UseAllianceCard(card, player);
			OnEffectApplied (card, player);
			break;
		case CardType.Scout_Card:
			UseScoutCard(card, player);
			break;
		case CardType.Priority_Card:
			UsePriorityCard(player);
			OnEffectApplied (card, player);
			break;
		case CardType.Upgrade_Card:
			RegisterCardUpgrade(card, player);
			break;
		default:
			break;
		}
	}
	
	private void RegisterCardHeal(CardData card, Player player) {
		OnHealingCardUsed (card, player);
	}
	
	public void UseHealingCard(CardData card, Player player, List<Unit> unitsToHeal) {
		foreach (var unit in unitsToHeal) {
			unit.Heal();
		}

		OnEffectApplied (card, player);
	}
	
	private void UseResourceCard(CardData card, Player player) {
		player.Currency.addPoints(card.Value);
	}
	
	private void UseBattleCard(Player player) {
		throw new NotImplementedException();
	}
	
	private void UseTacticCard(CardData card, Player player) {
		throw new NotImplementedException();
	}

	private void UseAllianceCard(CardData card, Player player) {	
		List<UnitType>[] totalUnits = 
		{
			allianceCardLevel0,
			allianceCardLevel1,
			allianceCardLevel2,
			allianceCardLevel3,
			allianceCardLevel4
		};

		List<UnitType> units = new List<UnitType>();
		UnitType randomUnit;
		int rand = UnityEngine.Random.Range(0, 100);
        
        int totalNextUnits = 0;
        if (player.CastleProgress < 4) {
            totalNextUnits = totalUnits[player.CastleProgress+1].Count;
        }
        
		if (rand >= totalNextUnits) {
			for(int i = 0; i <= player.CastleProgress; i++)
			{
				units.AddRange (totalUnits[i]);
			}
			int randomUnitIndex = UnityEngine.Random.Range(0, units.Count);
			randomUnit = units[randomUnitIndex];
		} else {
			units = totalUnits[player.CastleProgress + 1];
			int randomUnitIndex = UnityEngine.Random.Range(0, units.Count);
			randomUnit = units[randomUnitIndex];
		}

		player.PlayerArmy.AddUnit(randomUnit);
	}

	private void UseScoutCard(CardData card, Player player) {
		int availableScouts = player.PlayerArmy.GetActiveUnits(UnitType.Scout).Count;
        if (availableScouts > 0) {
            Mathf.Clamp(availableScouts++, 2, 4);
            player.IsScouting = true;
            card.Value = availableScouts;
            OnEffectApplied(card, player);
            return;
        }
        OnCardUseFailed(card, player);
        return;
	}
	
	private void UsePriorityCard(Player player) {
		throw new NotImplementedException();
	}

	private void RegisterCardUpgrade(CardData card, Player player) {
		OnUpgradeCardUsed (card, player);
	}

	private void UseUpgradeCard(CardData card, Player player, Unit unitToUpgrade) {
		UnitBaseData upgrade = unitToUpgrade.CreateUpgrade();
		unitToUpgrade.AddUpgrade(upgrade);
		OnEffectApplied(card, player);
	}

	private void UseTacticCard(CardData card, Player player, Unit unitToUpgrade) {
		UnitBaseData TempUpgrade = UnitBaseData.CreateInstance<UnitBaseData>();
		TempUpgrade.Strength = card.Value;
		unitToUpgrade.AddTempUpgrade(TempUpgrade);
	}
}