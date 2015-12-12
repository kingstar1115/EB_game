using UnityEngine;
using System.Collections.Generic;

public class FlagManager : MonoBehaviour {

    public GameObject BattlebeardFlag;
    public GameObject StormshaperFlag;
    Dictionary<TileData, FlagData> flagLookup;
    public GameObject BillboardTarget;

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
                fd.CurrentFlag = null;
            }
            return null;
        } else {
            if (fd.CurrentFlag != null) {
                if (fd.CurrentType == t.Owner) {
                    return fd.CurrentFlag;
                }
                else {
                    Destroy(fd.CurrentFlag.gameObject);
                    fd.CurrentFlag = null;
                }
            }
            GameObject flagToSet = t.Owner == PlayerType.Battlebeard ? BattlebeardFlag : StormshaperFlag;
            return (GameObject)Instantiate(flagToSet, fd.Marker.transform.position, fd.Marker.transform.rotation);
        }   
    }

    void Update() {
        // Billboard the flags to always face the target (target usually == camera)
        if (BillboardTarget != null) {
            foreach (FlagData f in flagLookup.Values) {
                if (f.CurrentFlag != null && f.Marker != null) {
                    Vector3 targetPostition = new Vector3(BillboardTarget.transform.position.x,
                             f.CurrentFlag.transform.position.y,
                             BillboardTarget.transform.position.z);
                    f.CurrentFlag.transform.LookAt(targetPostition);
                }
            }
        }
    }
}

public class FlagData {
    public GameObject Marker;
    public GameObject CurrentFlag;
    public PlayerType CurrentType;
}