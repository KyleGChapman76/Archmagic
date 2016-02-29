using UnityEngine;
using System.Collections.Generic;

public class ItemInventory : MonoBehaviour
{
	public bool deleteOnZero = false;

	private Dictionary<string,int> itemInventory = new Dictionary<string,int>();	

	private void Start()
	{
		foreach (string name in EssenceInfo.essenceNames)
		{
			AddItem(name, 0);
		}
	}

	public List<string> GetKeys ()
	{
		return new List<string>(itemInventory.Keys);
	}

	public int GetItemAmount (string key)
	{
		return itemInventory[key];
	}

	public void AddItem (string key)
	{
		AddItem(key, 1);
	}

	public void AddItem (string key, int amount)
	{
		if (itemInventory.ContainsKey(key))
			itemInventory[key] += amount;
		else
			itemInventory[key] = amount;
	}

	public void RemoveItem (string key)
	{
		RemoveItem(key, 1);
	}

	public void RemoveItem (string key, int amount)
	{
		if (EnoughOfItem(key, amount))
			itemInventory[key] -= amount;
		else
		{
			if (deleteOnZero)
				itemInventory.Remove(key);
			else
				itemInventory[key] = 0;
		}
	}

	public bool EnoughOfItem (string key, int amount)
	{
		return GetItemAmount(key) >= amount;
	}
}
