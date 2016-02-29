using UnityEngine;
using System.Collections;

public class ItemCollection : MonoBehaviour {
	
	public SpellHandler spells;
	
	private bool remote;
	
	private void Start ()
	{
		spells = GetComponent<SpellHandler>();
	}
	
	private void OnTriggerEnter(Collider other)
	{
		//if the trigger we entered is an item spawner, add the item to our inventory
		//and remove the item from the spawner
		if (other.tag.Equals("ItemSpawner"))
		{
			ItemSpawner spawner = other.GetComponent<ItemSpawner>();
			spawner.Collect();
			string name = spawner.getItemName();
			spells.GetInventory().AddItem(name);
		}
	}
}
