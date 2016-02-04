using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pool : MonoBehaviour 
{
	public GameObject Prefab;
	public int PooledAmount = 20;
	public bool WillGrow = true;
	
	protected List<GameObject> pooledSprites;
	
	void Start()
	{
		pooledSprites = new List<GameObject>();
		
		//instantiate pool objects
		for (int i = 0; i < PooledAmount; i++)
		{
			GameObject newSprite = (GameObject)Instantiate(Prefab);
			newSprite.SetActive(false);
			pooledSprites.Add(newSprite);
		}
	}
	
	public GameObject GetPooledObject()
	{
		for (int i = 0; i < pooledSprites.Count; i++)
		{
			if (!pooledSprites[i].activeInHierarchy)
			{
				return pooledSprites[i];
			}
		}
		
		if (WillGrow)
		{
			GameObject obj = (GameObject)Instantiate(Prefab);
			obj.SetActive(false);
			pooledSprites.Add(obj);
			PooledAmount++;
			return obj;
		}
		
		return null;
	}
}