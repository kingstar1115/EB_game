using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inn : MonoBehaviour
{
    public void HealTroops(Player player)
    {
		//Change magic number to function once more castle code is in
		if (player.CastleProgress >= 4)
			return;

		List<Unit> units;
		units = player.PlayerArmy.GetRandomUnits(3);

		foreach (var unit in units)
		{
			unit.Heal();
		}
    }
}
