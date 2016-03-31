using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

public delegate void BasicCardAction (CardData card, EndCardAction done);
public delegate void CardAction(CardData card, Player player, Unit u);
public delegate void EndCardAction(bool success, CardData card, Player player, Unit u);
public delegate void UnitSelectionCallback(CardData card, int number, Player player, CardAction action, EndCardAction done);

public class CardSystem : MonoBehaviour {	

	public CardList cardList;
    // Units availiable from using alliance card at different castle levels.
    public List<UnitType> allianceCardLevel0;
	public List<UnitType> allianceCardLevel1;
	public List<UnitType> allianceCardLevel2;
	public List<UnitType> allianceCardLevel3;
	public List<UnitType> allianceCardLevel4;

	public event EndCardAction OnEffectApplied = delegate { };
	public event UnitSelectionCallback RequestUnitSelection = delegate { };
	public event BasicCardAction RequestBattle = delegate {};


	public bool CanUseCard(CardData cData, GameState gameState)
	{
		//check that the card can use the game state 
		return (cData.UseableGameState & gameState) != gameState;
	}

	public void UseCard(CardData card, Player player, Player inactivePlayer, GameState gameState) 
	{
		//check if card can be used in this game state first 
		if (CanUseCard (card, gameState)) 
		{
			OnEffectApplied(false, card, player, null);
			return;
		}

		//apply the actual effect depending on card type
		switch (card.Type) {
			case CardType.Healing_Card:
				int koUnits = player.PlayerArmy.GetKOUnits().Count,
					numSelection = card.Value <= koUnits ? card.Value : koUnits;
				if (numSelection == 0 || inactivePlayer.CastleProgress == 4) {
					// action failed
					OnEffectApplied(false, card, player, null);
					break;
				}
				RequestUnitSelection(card, numSelection, player, UseHealingCard, OnEffectApplied);
				break;
			case CardType.Resource_Card:
				UseResourceCard(card, player);
				break;
			case CardType.Battle_Card:
				RequestBattle(card, OnEffectApplied);
				break;
			case CardType.Tactic_Card:
				if (player.PlayerArmy.GetTempUpgradableUnits().Count == 0) {
					// action failed
					OnEffectApplied(false, card, player, null);
					break;
				}
				RequestUnitSelection(card, 1, player, UseTacticCard, OnEffectApplied);
				break;
			case CardType.Alliance_Card:
				UseAllianceCard(card, player);
				break;
			case CardType.Scout_Card:
				UseScoutCard(card, player);
				break;
			case CardType.Priority_Card:
				//RequestUnitSelection(card, 1, player, UsePriorityCard, OnEffectApplied);
				OnEffectApplied(false, card, player, null);
				break;
			case CardType.Upgrade_Card:
				if (player.PlayerArmy.GetUpgradableUnits().Count == 0) {
					// action failed
					OnEffectApplied(false, card, player, null);
					break;
				}
				RequestUnitSelection(card, 1, player, UseUpgradeCard, OnEffectApplied);
				break;
			default:
				break;
		}
	}

	
	public void UseHealingCard(CardData card, Player player, Unit u) {
		u.Heal();
	}
	
	private void UseResourceCard(CardData card, Player player) {
		player.Currency.addPoints(card.Value);
		OnEffectApplied(true, card, player, null);
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

		List<UnitType> unitTypes = new List<UnitType>();
		UnitType randomUnitType;
		int rand = UnityEngine.Random.Range(0, 100);
        
        int totalNextUnits = 0;
        if (player.CastleProgress < 4) {
            totalNextUnits = totalUnits[player.CastleProgress+1].Count;
        }
        
		if (rand >= totalNextUnits) {
			for(int i = 0; i <= player.CastleProgress; i++)
			{
				unitTypes.AddRange(totalUnits[i]);
			}
			int randomUnitIndex = UnityEngine.Random.Range(0, unitTypes.Count);
			randomUnitType = unitTypes[randomUnitIndex];
		} else {
			unitTypes = totalUnits[player.CastleProgress + 1];
			int randomUnitIndex = UnityEngine.Random.Range(0, unitTypes.Count);
			randomUnitType = unitTypes[randomUnitIndex];
		}

		Unit unit = player.PlayerArmy.AddUnit(randomUnitType);
		OnEffectApplied(true, card, player, unit);
	}

	private void UseScoutCard(CardData card, Player player) {
		int availableScouts = player.PlayerArmy.GetActiveUnits(UnitType.Scout).Count;
		bool success = false;
        if (availableScouts > 0 && !player.IsScouting) {
			success = true;
		}
		OnEffectApplied(success, card, player, null);
	}

	private void UsePriorityCard(CardData card, Player player, Unit u) {
		//throw new NotImplementedException();
	}

	private void UseUpgradeCard(CardData card, Player player, Unit unitToUpgrade) {
		UnitBaseData upgrade = unitToUpgrade.CreateUpgrade();
		unitToUpgrade.AddUpgrade(upgrade);
	}

	private void UseTacticCard(CardData card, Player player, Unit unitToUpgrade) {
		UnitBaseData TempUpgrade = UnitBaseData.CreateInstance<UnitBaseData>();
		TempUpgrade.Strength = card.Value;
		unitToUpgrade.AddTempUpgrade(TempUpgrade);
	}
	public void RemoveListeners() {
		OnEffectApplied = delegate { };
		RequestUnitSelection = delegate { };
	}

	public CardData GetRandomCard() {
		return GetRandomCard(cardList.cards, GetRandomCardType());
	}

	public CardData GetRandomCard(CardType t) {
		return GetRandomCard(cardList.cards.FindAll(x => x.Type == t));
	}

	public CardData GetRandomCard(List<CardData> c, CardType t) {
		c = c.FindAll(x => x.Type == t);
		if (c.Count == 0) {
			return null;
		}
		int random = UnityEngine.Random.Range(0, c.Count);
		return c[random];
	}

	public CardData GetRandomCard(List<CardData> c) {
		if (c.Count == 0) {
			return null;
		}
		CardType t = GetRandomCardType(c);
		return GetRandomCard(c, t);
	}

	public CardType GetRandomCardType(List<CardData> c) {
		List<CardType> t = c.Select(x => x.Type).Distinct().ToList();
		int random = UnityEngine.Random.Range(0, t.Count);
		return t[random];
	}
	public CardType GetRandomCardType() {
		return GetRandomCardType(cardList.cards);
	}
}