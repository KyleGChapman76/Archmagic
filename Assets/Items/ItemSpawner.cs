using UnityEngine;
using System.Collections;

public class ItemSpawner : MonoBehaviour
{
	public int respawn;
	private int timer;
	public GameObject prefab;
	private int spawnedItemID;
	
	public bool itemSpawned;
	
	private void  Start ()
	{
		timer = 0;
		if (prefab == null)
		{
			print("Error! This itemmarker has no attached item to spawn!" + gameObject.name);
			Destroy(gameObject);
		}
	}
	
	public void  Collect ()
	{
		gameObject.GetComponent<CapsuleCollider>().GetComponent<Collider>().enabled = false;
		itemSpawned = false;
	}
	
	public string getItemName()
	{
		return prefab.GetComponent<EssenceInfo>().GetItemName();
	}
	
	private void  FixedUpdate ()
	{
		if (!itemSpawned)
		{
			timer++;
			if (timer >= respawn)
			{
				timer = 0;
				itemSpawned = true;
				gameObject.GetComponent<CapsuleCollider>().GetComponent<Collider>().enabled = true;
			}
		}
	}
}
