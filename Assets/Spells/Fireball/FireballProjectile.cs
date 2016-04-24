using UnityEngine;
using System.Collections;

public class FireballProjectile : MonoBehaviour
{
	public int damage;
	public int duration;
	public float explosionRadius;
	private int timer;

	public GameObject fireballExplosionPrefab;

	private void Start ()
	{
		timer = 0;
	}

	private void OnTriggerEnter (Collider collider)
	{
		if (collider.isTrigger)
			return;
		
		GameObject obj = collider.gameObject;
		Health health = obj.GetComponent<Health>();
		if (health != null)
		{
			health.Damage(damage/2);
		}

		if (enabled)
		{
			Destroy(gameObject);
        }
	}

	private void FixedUpdate ()
	{
		timer++;
		if (timer > duration && duration != 0)
			Destroy(gameObject);
	}

	private void OnDestroy()
	{
		Instantiate(fireballExplosionPrefab, transform.position, Quaternion.identity);

		Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
		foreach (Collider collider in colliders)
		{
			GameObject obj = collider.gameObject;
			Health health = obj.GetComponent<Health>();
			if (health != null)
			{
				health.Damage(damage);
				obj.GetComponent<FPDPhysics>().Knockback(((obj.transform.position - transform.position).normalized + Vector3.up) * 10);
			}
		}
	}
}
