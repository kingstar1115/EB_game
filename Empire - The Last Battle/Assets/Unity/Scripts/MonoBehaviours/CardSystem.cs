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
    public List<Unit> castleZeroUnits;
	public List<Unit> castleOneUnits;
	public List<Unit> castleTwoUnits;
	public List<Unit> castleThreeUnits;
	public List<Unit> castleFourUnits;

	public delegate void CardCallback(CardData card, Player player);
	public event CardCallback OnEffectApplied = delegate { };
	public event CardCallback OnHealingCardUsed = delegate { };
	
	public void Start(){

	}

	public void ApplyEffect(CardData card, Player player) {
		switch (card.CardType) {
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
			UseScoutCard(player);
			OnEffectApplied (card, player);
			break;
		case CardType.Priority_Card:
			UsePriorityCard(player);
			OnEffectApplied (card, player);
			break;
		case CardType.Upgrade_Card:
			UseUpgradeCard(card, player);
			OnEffectApplied (card, player);
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
		List<Unit>[] totalUnits = 
		{
			castleZeroUnits,
			castleOneUnits,
			castleTwoUnits,
			castleThreeUnits,
			castleFourUnits
		};

		var units = new List<Unit>();
		Unit randomUnit;
		float rand = UnityEngine.Random.Range(0, 4);

		if (rand < totalUnits[player.CastleProgress].Count) {
			// pick from
			for(int i = 0; i <= player.CastleProgress; i++)
			{
				units.AddRange (totalUnits[i]);
			}
			int randomUnitIndex = UnityEngine.Random.Range(0, units.Count);
			randomUnit = units[randomUnitIndex];
		} else {
			// pick from
			units = totalUnits[player.CastleProgress + 1];
			int randomUnitIndex = UnityEngine.Random.Range(0, units.Count);
			randomUnit = units[randomUnitIndex];
		}
		
		player.PlayerArmy.Units.Add (randomUnit);
	}
	
	private void UseScoutCard(Player player) {
		throw new NotImplementedException();
	}
	
	private void UsePriorityCard(Player player) {
		throw new NotImplementedException();
	}
	
	private void UseUpgradeCard(CardData card, Player player) {
		throw new NotImplementedException();
	}
}