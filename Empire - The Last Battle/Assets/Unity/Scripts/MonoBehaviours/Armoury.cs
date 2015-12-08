using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PurchasableItem : ScriptableObject
{
    public int Cost { get; set; }
}

public class PurchasableUnit : PurchasableItem
{
    //Change to UnitType once units are in
    public int Id { get; set; }

    //This is 0-4, and depends on how many 
    //castle pieces player has
    public int PurchaseLevel { get; set; }
}

public class PurchasbleCard : PurchasableItem
{
    public int Id { get; set; }
}

public class PurchasbleCastlePiece : PurchasableItem
{
    public int Id { get; set; }
}

public class Armoury : MonoBehaviour
{
    public List<PurchasableUnit> PurchasbleUnits { get; set; }
    public List<PurchasbleCard> PurchasbleCards { get; set; }
    public List<PurchasbleCastlePiece> PurchasableCastlePieces { get; set; }

    public delegate void PurchasedItemCallback(PurchasableItem purchasedItem);
    public event PurchasedItemCallback OnPurchasedItem = delegate { };

    void Initialise()
    {
        PurchasbleUnits = new List<PurchasableUnit>
        {
            //Add units here when unit class is in
        };

        PurchasbleCards = new List<PurchasbleCard>
        {
            //Add cards here when cards class is in 
        };

        PurchasableCastlePieces = new List<PurchasbleCastlePiece>
        {
            new PurchasbleCastlePiece(){ Id = 0, Cost = 2000 },
            new PurchasbleCastlePiece(){ Id = 1, Cost = 2000 },
            new PurchasbleCastlePiece(){ Id = 2, Cost = 2000 },
            new PurchasbleCastlePiece(){ Id = 3, Cost = 2000 },
        };
    }

    public List<PurchasableUnit> AvailableUnits(Player player)
    {
        //Do checks to see if buyable
        //return (List<PurchasableUnit>)PurchasbleUnits.Where(x => x.PurchaseLevel <= player.castlePieceNumber);
        throw new NotImplementedException();
    }

    public List<PurchasbleCard> AvailableCards(Player player)
    {
        //Do checks to see if buyable

        return PurchasbleCards;
    }

    public void BuyUnit(int unitId, Player player)
    {
        var unitToBuy = PurchasbleUnits[unitId];

        //Call Delegate
        OnPurchasedItem(unitToBuy);
    }

    public void BuyCard(int cardId)
    {
        var cardToBuy = PurchasbleCards[cardId];

        OnPurchasedItem(cardToBuy);
    }

    public void BuyCastlePiece(int cardId)
    {
        var castlePieceToBuy = PurchasableCastlePieces[cardId];

        OnPurchasedItem(castlePieceToBuy);
    }
}
