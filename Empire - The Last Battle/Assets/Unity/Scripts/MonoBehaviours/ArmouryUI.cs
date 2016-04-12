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
	public List<PurchasableUnit> PurchasableUnits;
	public List<PurchasableCard> PurchasableCards;
	public List<PurchasableCastlePiece> PurchasableCastlePieces;
	public Button CastlesButton;

	public event Action<PurchasableItem> OnPurchasedItem = delegate { };

	int previousCurrency;

	public Transform _UnitsUIParent;
	public Transform _CardUIParent;
	public Transform _CastleUIParent;

	//const string k_UnitsSection = "UnitsSection";
	//const string k_CardsSection = "CardsSection";
	//const string k_CastleSection = "CastlesSection";

	public IEnumerable<PurchasableUnit> AvailableUnits(Player player)
	{
		return PurchasableUnits.Where(x => x.purchaseLevel <= player.CastleProgress && x.cost <= player.Currency.getPoints()).ToList();
	}

	public IEnumerable<PurchasableCard> AvailableCards(Player player)
	{
		return PurchasableCards.Where(x => x.cost <= player.Currency.getPoints()).ToList();
	}

	public IEnumerable<PurchasableCastlePiece> AvailableCastlePieces(Player player)
	{
		return PurchasableCastlePieces.Where(x => x.purchaseLevel <= player.LostImmortalKillCount && !x.b_AlreadyPurchased && x.cost <= player.Currency.getPoints()).ToList();
	}

	//When the currency is changed it will update what can be shown in the armoury
	public void CurrencyChangedUpdate(int val, Player player)
	{
		bool disable = false;

		if (val <= previousCurrency)
			disable = true;
		
		var units = AvailableUnits(player).ToList();
        ToggleUIImages(units, _UnitsUIParent, player, disable);

		var cards = AvailableCards(player).ToList();
        ToggleUIImages(cards, _CardUIParent, player, disable);

		var castlePieces = AvailableCastlePieces(player).ToList();
        ToggleUIImages(castlePieces, _CastleUIParent, player, disable);

		previousCurrency = val;
	}

	void UpdateItems(Player player)
	{
		ToggleUIImages(AvailableUnits(player).ToList(), _UnitsUIParent, player);
		ToggleUIImages(AvailableCards(player).ToList(), _CardUIParent, player);
		ToggleUIImages(AvailableCastlePieces(player).ToList(), _CastleUIParent, player);
	}

	void ToggleUIImages<T>(IList<T> purchasableItems, Transform parentTrans, Player player, bool disableOn = true) where T : PurchasableItem
	{
		/*
		Transform section = null;

		//Need to loop through children transforms to get correct section gameobject (Unit, Card or Castle UI)
        foreach (Transform child in transform.GetComponentInChildren<Transform>())
		{
			Debug.Log("CHILD!!: "+child.name);
			section = child.Find(sectionName);
		}
		*/

		if (parentTrans != null)
		{
			//Disables any images to stop them being clickable, usually when the user doesn't have enough money
			//Else enable the images on the items that are passed into this method
			//Gameobject UI items must contain the same name as scriptable objects purchasble items for this to work.
			if (disableOn)
				parentTrans.GetComponentsInChildren<Image>(true).Where(x => x.transform.parent == parentTrans && !purchasableItems.Any(z => x.name.Contains(z.name.ToString())))
					.ToList().ForEach(x => { x.color = Color.grey; x.GetComponent<CanvasGroup>().interactable = false; });
			else
				parentTrans.GetComponentsInChildren<Image>(true).Where(x => x.transform.parent == parentTrans && purchasableItems.Any(z => x.name.Contains(z.name.ToString())))
                    .ToList().ForEach(x => { x.color = Color.white; x.GetComponent<CanvasGroup>().interactable = true; });
		}

		if (player.LostImmortalKillCount > 0) {
			CastlesButton.interactable = true;
		}
	}

	public void Show(Player player)
	{
		Debug.Log("show");
		gameObject.SetActive(true);
		UpdateItems(player);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
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

		PurchasableCastlePieces.Remove(purchasedCastlePiece);

		if (Debug.isDebugBuild)
			Debug.Log("Castle Piece " + purchasedCastlePiece.name + " bought");
	}

	public void RemoveListeners()
	{
		OnPurchasedItem = delegate { };
	}
}