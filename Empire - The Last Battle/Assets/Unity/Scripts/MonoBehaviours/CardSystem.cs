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
	Scout_Card_2,
	Scout_Card_3,
	Scout_Card_4,
	Priority_Card_1,
	Upgrade_Card_1,
	Upgrade_Card_2
}

public class CardSystem : MonoBehaviour
{
	public Dictionary<int, GameObject> cardsLinker;

	public delegate void CardEffectCallback(Cards card);
	public event CardEffectCallback OnEffectApplied = delegate { };
	
	void Start()
	{

	}
	
	public void ApplyEffect(Cards card, Player player)
	{
		switch (card)
		{
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
		case Cards.Scout_Card_2:
			UseScoutCard(2, player);
			break;
		case Cards.Scout_Card_3:
			UseScoutCard(3, player);
			break;
		case Cards.Scout_Card_4:
			UseScoutCard(4, player);
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
	
	private void UseHealingCard(int amount, Player player)
	{
		throw new NotImplementedException();
	}
	
	private void UseResourceCard(int amount, Player player)
	{
		player.currency.addPoints(amount);
	}
	
	private void UseBattleCard(int amount, Player player)
	{
		throw new NotImplementedException();
	}
	
	private void UseTacticCard(int amount, Player player)
	{
		throw new NotImplementedException();
	}
	
	private void UseAllianceCard(int amount, Player player)
	{
		throw new NotImplementedException();
	}
	
	private void UseScoutCard(int amount, Player player)
	{
		throw new NotImplementedException();
	}
	
	private void UsePriorityCard(int amount, Player player)
	{
		throw new NotImplementedException();
	}
	
	private void UseUpgradeCard(int amount, Player player)
	{
		throw new NotImplementedException();
	}
}