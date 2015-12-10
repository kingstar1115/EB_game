using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PurchasableItem : ScriptableObject
{
    public int cost;
}

public class PurchasableUnit : PurchasableItem
{
    //Change to UnitType once units are in
    public int id;
    //This is 0-4, and depends on how many 
    //castle pieces player has
    public int purchaseLevel;
}

public class PurchasableCard : PurchasableItem
{
    public int id;
}

public class PurchasableCastlePiece : PurchasableItem
{
    public int id;
    //This is 0-4
    public int purchaseLevel;
    public bool b_AlreadyPurchased;
}

public class Armoury : MonoBehaviour
{
    public List<PurchasableUnit> purchasbleUnits;
    public List<PurchasableCard> purchasbleCards;
    public List<PurchasableCastlePiece> purchasableCastlePieces;

    public delegate void PurchasedItemCallback(PurchasableItem purchasedItem);
    public event PurchasedItemCallback OnPurchasedItem = delegate { };

    public List<PurchasableUnit> AvailableUnits(Player player)
    {
        //Do checks to see if buyable
        //return (List<PurchasableUnit>)PurchasbleUnits.Where(x => x.PurchaseLevel <= player.castlePieceNumber);
        throw new NotImplementedException();
    }

    public List<PurchasableCard> AvailableCards(Player player)
    {
        //Do checks to see if buyable
        return purchasbleCards;
    }

    public List<PurchasableCastlePiece> AvailableCastlePieces(Player player)
    {
        //Castle pieces unlock after lost immortals die
        //return purchasableCastlePieces.Where(x => (player.LostImmortalKillCount >= x.purchaseLevel) && (x.b_AlreadyPurchased == false));      
        throw new NotImplementedException();
    }

    public void BuyUnit(PurchasableUnit purchasableUnit)
    {
        //Call Delegate
        OnPurchasedItem(purchasableUnit);
    }

    public void BuyCard(PurchasableCard purchasableCard)
    {
        OnPurchasedItem(purchasableCard);
    }

    public void BuyCastlePiece(PurchasableCastlePiece purchasableCastlePiece)
    {
        OnPurchasedItem(purchasableCastlePiece);
    }
}