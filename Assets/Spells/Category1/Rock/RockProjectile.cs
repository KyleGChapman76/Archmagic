using UnityEngine;
using System.Collections;

public class RockProjectile : MonoBehaviour
{
	public int damage;
	
	private void Start ()
	{
		foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
		{
			if (player.name == "MainPlayer")
			{
				Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>());
			}
		}
		foreach (GameObject projectile in GameObject.FindGameObjectsWithTag("Projectile"))
		{
			if (projectile != gameObject && projectile.GetComponent<Collider>() != null)
				Physics.IgnoreCollision(projectile.GetComponent<Collider>(), GetComponent<Collider>());
		}
	}
	
	private void OnTriggerEnter (Collider collider)
	{
		if (collider.isTrigger)
			return;
		if (enabled)
			Destroy(gameObject);
		
		GameObject obj = collider.gameObject;
		Health health = obj.GetComponent<Health>();
		if (health != null)
		{
			health.Damage(damage);
		}
	}
}