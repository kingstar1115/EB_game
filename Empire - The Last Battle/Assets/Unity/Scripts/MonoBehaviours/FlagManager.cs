using UnityEngine;
using System.Collections.Generic;

public class FlagManager : MonoBehaviour {

    public GameObject BattlebeardFlag;
    public GameObject StormshaperFlag;
    Dictionary<TileData, FlagData> flagLookup;

	// Use this for initialization
	public void Initialise () {
        flagLookup = new Dictionary<TileData, FlagData>();
	}

    public void SetFlagForTile(TileData t) {
        FlagData flagData;
        flagLookup.TryGetValue(t, out flagData);
        if (flagData == null) {
            flagData = new FlagData();
            flagData.Marker = Utils.GetFirstChildWithTag("MarkerFlag", t.TileObject);
            flagLookup.Add(t, flagData);
        }

        if (flagData.Marker) {
            flagData.CurrentFlag = setFlagObject(t, flagData);
            flagData.CurrentType = t.Owner;
        }  
    }


    GameObject setFlagObject(TileData t, FlagData fd) {
        if (t.Owner == PlayerType.None) {
            if (fd.CurrentFlag != null) {
                Destroy(fd.CurrentFlag.gameObject);
            }
            return null;
        } else {
            if (fd.CurrentFlag != null) {
                if (fd.CurrentType == t.Owner) {
                    return fd.CurrentFlag;
                }
                else {
                    Destroy(fd.CurrentFlag.gameObject);
                }
            }
            GameObject flagToSet = t.Owner == PlayerType.Battlebeard ? BattlebeardFlag : StormshaperFlag;
            return (GameObject)Instantiate(flagToSet, fd.Marker.transform.position, fd.Marker.transform.rotation);
        }   
    }
}

public class FlagData {
    public GameObject Marker;
    public GameObject CurrentFlag;
    public PlayerType CurrentType;
}