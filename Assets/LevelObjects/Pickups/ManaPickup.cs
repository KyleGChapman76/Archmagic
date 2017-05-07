using UnityEngine;
using System.Collections;

public class ManaPickup : MonoBehaviour {

	public int restoreAmount;

	public void OnTriggerEnter(Collider collider)
	{
		Mana mana = collider.GetComponent<Mana>();
		if (mana && mana.RestoreMana(restoreAmount))
		{
			Destroy(gameObject);
		}
	}
}
