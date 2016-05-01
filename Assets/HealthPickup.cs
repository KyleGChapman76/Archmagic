using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour
{
	public int healAmount;

	public void OnTriggerEnter(Collider collider)
	{
		Health health = collider.GetComponent<Health>();
		if (health && health.Heal(healAmount))
		{
			Destroy(gameObject);
		}
    }
}
