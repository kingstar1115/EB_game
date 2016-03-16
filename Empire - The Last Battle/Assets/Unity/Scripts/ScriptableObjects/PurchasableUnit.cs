using UnityEngine;
using System.Collections;

public class PurchasableUnit : PurchasableItem
{
	public UnitType UNITTYPE;
	//This is 0-4, and depends on how many 
	//castle pieces player has
	public int purchaseLevel;
}
