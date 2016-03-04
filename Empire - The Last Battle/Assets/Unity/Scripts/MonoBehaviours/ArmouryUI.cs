using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//Don't want to ever create a scriptable object instance of just 
//this class so leave it here and mark abstract
public abstract class PurchasableItem : ScriptableObject
{
	public int cost;
}

public class ArmouryUI : MonoBehaviour
{
	public List<PurchasableUnit> purchasableUnits;
	public List<PurchasableCard> purchasableCards;
	public List<PurchasableCastlePiece> purchasableCastlePieces;

	public event Action<PurchasableItem> OnPurchasedItem = delegate { };

	int previousCurrency;

	const string k_UnitsSection = "UnitsSection";
	const string k_CardsSection = "CardsSection";
	const string k_CastleSection = "CastlesSection";

	public IEnumerable<PurchasableUnit> AvailableUnits(Player player)
	{
		return purchasableUnits.Where(x => x.purchaseLevel <= player.CastleProgress && x.cost <= player.Currency.getPoints()).ToList();
	}

	public IEnumerable<PurchasableCard> AvailableCards(Player player)
	{
		return purchasableCards.Where(x => x.cost <= player.Currency.getPoints()).ToList();
	}

	public IEnumerable<PurchasableCastlePiece> AvailableCastlePieces(Player player)
	{
		//return early if no purchasable castle pieces
		if (purchasableCastlePieces.Count == 0)
		{
			Debug.Log("No purchasable castle peices at all");
			return Enumerable.Empty<PurchasableCastlePiece>();
		}

		return purchasableCastlePieces.Where(x => player.LostImmortalKillCount >= x.purchaseLevel && x.b_AlreadyPurchased).ToList();
	}

	//When the currency is changed it will update what can be shown in the armoury
	public void CurrencyChangedUpdate(int val, Player player)
	{
		bool disable = false;

		if (val <= previousCurrency)
			disable = true;

		var units = AvailableUnits(player).ToList();
		ToggleUIImages(units, k_UnitsSection, disable);

		var cards = AvailableCards(player).ToList();
		ToggleUIImages(cards, k_CardsSection, disable);

		var castlePieces = AvailableCastlePieces(player).ToList();
		ToggleUIImages(castlePieces, k_CastleSection, disable);

		previousCurrency = val;
	}

	void UpdateItems(Player player)
	{
		ToggleUIImages(AvailableUnits(player).ToList(), k_UnitsSection);
		ToggleUIImages(AvailableCards(player).ToList(), k_CardsSection);
		ToggleUIImages(AvailableCastlePieces(player).ToList(), k_CastleSection);
	}

	void ToggleUIImages<T>(IList<T> purchasableItems, string sectionName, bool disableOn = true) where T : PurchasableItem
	{
		Transform section = null;

		//Need to loop through children transforms to get correct section gameobject (Unit, Card or Castle UI)
        foreach (Transform child in transform)
		{
			section = child.Find(sectionName);
		}

		if (section != null)
		{
			//Disables any images to stop them being clickable, usually when the user doesn't have enough money
			//Else enable the images on the items that are passed into this method
			//Gameobject UI items must contain the same name as scriptable objects purchasble items for this to work.
			if (disableOn)
				section.GetComponentsInChildren<Image>(true).Where(x => !purchasableItems.Any(z => x.name.Contains(z.name.ToString())))
					.ToList().ForEach(x => { x.color = Color.grey; x.GetComponent<Button>().interactable = false; });
			else
				section.GetComponentsInChildren<Image>(true).Where(x => purchasableItems.Any(z => x.name.Contains(z.name.ToString())))
					.ToList().ForEach(x => { x.color = Color.white; x.GetComponent<Button>().interactable = true; });
		}
	}

	public void ToggleShow(bool toggledOn, Player player)
	{
		gameObject.SetActive(toggledOn);
		UpdateItems(player);
	}

	public void BuyUnit(PurchasableUnit purchasedUnit)
	{
		OnPurchasedItem(purchasedUnit);

		if (Debug.isDebugBuild)
			Debug.Log("Unit " + purchasedUnit.name + " bought");
	}

	public void BuyCard(PurchasableCard purchasedCard)
	{
		OnPurchasedItem(purchasedCard);

		if (Debug.isDebugBuild)
			Debug.Log("Card " + purchasedCard.name + " bought");
	}

	public void BuyCastlePiece(PurchasableCastlePiece purchasedCastlePiece)
	{
		OnPurchasedItem(purchasedCastlePiece);

		if (Debug.isDebugBuild)
			Debug.Log("Castle Piece " + purchasedCastlePiece.name + " bought");
	}

	public void RemoveListeners()
	{
		OnPurchasedItem = delegate { };
	}
}