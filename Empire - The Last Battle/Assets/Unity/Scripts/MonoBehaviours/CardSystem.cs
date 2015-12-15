using System;
using System.Collections.Generic;
using UnityEngine;

public enum Cards
{
	Healing_Card_1,
	Healing_Card_2,
	Resource_Card_100,
	Resource_Card_200,
	Resource_Card_300,
	Resource_Card_400,
	Resource_Card_500,
	Battle_Card_1,
	Tactic_Card_1,
	Tactic_Card_2,
	Tactic_Card_3,
	Alliance_Card_1,
	Scout_Card_1,
	Priority_Card_1,
	Upgrade_Card_1,
	Upgrade_Card_2
}

public class CardSystem : MonoBehaviour
{
	public Dictionary<int, GameObject> cardsLinker;
	public List<Unit> castleZeroUnits;
	public List<Unit> castleOneUnits;
	public List<Unit> castleTwoUnits;
	public List<Unit> castleThreeUnits;
	public List<Unit> castleFourUnits;

	public delegate void CardEffectCallback(Cards card);
	public event CardEffectCallback OnEffectApplied = delegate { };

	public void ApplyEffect(Cards card, Player player) {
		switch (card) {
			case Cards.Healing_Card_1:
				UseHealingCard(1, player);
				break;
			case Cards.Healing_Card_2:
				UseHealingCard(2, player);
				break;
			case Cards.Resource_Card_100:
				UseResourceCard(100, player);
				break;
			case Cards.Resource_Card_200:
				UseResourceCard(200, player);
				break;
			case Cards.Resource_Card_300:
				UseResourceCard(300, player);
				break;
			case Cards.Resource_Card_400:
				UseResourceCard(400, player);
				break;
			case Cards.Resource_Card_500:
				UseResourceCard(500, player);
				break;
			case Cards.Battle_Card_1:
				UseResourceCard(1, player);
				break;
			case Cards.Tactic_Card_1:
				UseTacticCard(1, player);
				break;
			case Cards.Tactic_Card_2:
				UseTacticCard(2, player);
				break;
			case Cards.Tactic_Card_3:
				UseTacticCard(3, player);
				break;
			case Cards.Alliance_Card_1:
				UseAllianceCard(1, player);
				break;
			case Cards.Scout_Card_1:
				UseScoutCard(1, player);
				break;
			case Cards.Priority_Card_1:
				UsePriorityCard(1, player);
				break;
			case Cards.Upgrade_Card_1:
				UseUpgradeCard(1, player);
				break;
			case Cards.Upgrade_Card_2:
				UseUpgradeCard(2, player);
				break;
			default:
				break;
		}
		OnEffectApplied(card);
	}

	private void UseHealingCard(int amount, Player player) {
		throw new NotImplementedException();
	}

	private void UseResourceCard(int amount, Player player) {
		player.Currency.addPoints(amount);
	}

	private void UseBattleCard(int amount, Player player) {
		throw new NotImplementedException();
	}

	private void UseTacticCard(int amount, Player player) {
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

		List<Unit> units;
		Unit randomUnit;
		float rand = UnityEngine.Random.Range(0, 4);

		if (rand < totalUnits[player.CastleProgress].Count) {
			// pick from
			for(int i = 0; i <= player.CastleProgress; i++)
			{
				units.Add (totalUnits[i]);
			}
			float randomUnitIndex = UnityEngine.Random.Range(0, units.Count);
			randomUnit = units[randomUnitIndex];
		} else {
			// pick from
			units = totalUnits[player.CastleProgress + 1];
			float randomUnitIndex = UnityEngine.Random.Range(0, units.Count);
			randomUnit = units[randomUnitIndex];
		}
		
		player.PlayerArmy.Units.Add (randomUnit);
	}

	private void UseScoutCard(int amount, Player player) {
		throw new NotImplementedException();
	}

	private void UsePriorityCard(int amount, Player player) {
		throw new NotImplementedException();
	}

	private void UseUpgradeCard(int amount, Player player) {
		throw new NotImplementedException();
	}
}