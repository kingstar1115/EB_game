using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Armoury : MonoBehaviour
{
    public Player player;
    public PointsSystem currency;

    IList<int> GetBuyableUnits; //Change to UnitType once Units are in
    IList<int> GetBuyableCards;
    bool b_IsCastlePieceBuyable;

    IDictionary<int, int> UnitPrices { get; set; } //Change key to UnitType once Units are in
    IDictionary<int, int> CardPrices { get; set; }
    IDictionary<int, int> CastlePiecePrices { get; set; }

    public delegate void PurchasedItemCallback(int id);
    public event PurchasedItemCallback OnPurchasedItem = delegate { };

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PurchaseUnit(int unitId)
    {
        var unitPrice = UnitPrices.First(x => x.Key == unitId).Value;

        if (GetBuyableUnits.Contains(unitId))
        {
            if (currency.getPoints() < unitPrice)
            {
                //Call UI, saying that player hasn't enough coins
                //Return from function and don't call purchaseItem delegate
                return;
            }

            //Give player the unit

            //Remove currency
            currency.removePoints(unitPrice, unitId.ToString());

            //Call Delegate
            var purchasedItem = new PurchasedItemCallback(PurchaseUnit);
            purchasedItem(unitId);
        }
        else
        {
            Debug.Log("unitId outside of list range");
        }
    }

    public void PurchaseCard(int cardId)
    {
        var cardPrice = CardPrices.First(x => x.Key == cardId).Value;

        if (GetBuyableCards.Contains(cardId))
        {
            if (currency.getPoints() < cardPrice)
            {
                return;
            }

            currency.removePoints(cardPrice, cardId.ToString());

            var purchasedItem = new PurchasedItemCallback(PurchaseCard);
            purchasedItem(cardId);
        }
        else
        {
            Debug.Log("cardId outside of list range");
        }
    }

    public void PurchaseCastlePiece(int castlePieceId)
    {
        var castlePiecePrice = CastlePiecePrices.First(x => x.Key == castlePieceId).Value;

        if (b_IsCastlePieceBuyable)
        {
            if (currency.getPoints() < castlePiecePrice)
            {
                return;
            }

            currency.removePoints(castlePiecePrice, castlePieceId.ToString());

            var purchasedItem = new PurchasedItemCallback(PurchaseCastlePiece);
            purchasedItem(castlePieceId);
        }
        else
        {
            Debug.Log("castlePieceId outside of list range");
        }
    }
}
