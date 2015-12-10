using UnityEngine;
using System.Collections.Generic;


static class Utils {
    static public GameObject GetFirstChildWithTag(string tag, GameObject o) {
        Transform[] allChildren = o.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren) {
            if (child.gameObject.tag == tag) {
                return child.gameObject;
            }
        }
        return null;
    }

    static public List<GameObject> GetChildrenWithTag(string tag, GameObject o) {
        List<GameObject> children = new List<GameObject>();
        Transform[] allChildren = o.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren) {
            if (child.gameObject.tag == tag) {
                children.Add(child.gameObject);
            }
        }
        return children;
    }
}

