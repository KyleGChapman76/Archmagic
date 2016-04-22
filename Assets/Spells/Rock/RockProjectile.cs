using UnityEngine;
using System.Collections;

public class RockProjectile : MonoBehaviour
{
	public int damage;
	public GameObject rockDustExplosionPrefab;

	private void FixedUpdate()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		rb.AddForce(Physics.gravity * rb.mass * .5f);
	}

	private void OnTriggerEnter (Collider collider)
	{
		if (collider.isTrigger)
			return;

		if (enabled)
		{
			GameObject dustExplosion = Instantiate(rockDustExplosionPrefab, transform.position, Quaternion.identity) as GameObject;
			dustExplosion.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
            Destroy(gameObject);
		}
		
		GameObject obj = collider.gameObject;
		Health health = obj.GetComponent<Health>();
		if (health != null)
		{
			health.Damage(damage);
		}

		FPDPhysics physics = collider.GetComponent<FPDPhysics>();
		Rigidbody rb = collider.GetComponent<Rigidbody>();

		Vector3 currentRockVelocity = GetComponent<Rigidbody>().velocity;

		if (physics)
		{
			physics.Knockback(currentRockVelocity * 10);
		}
		else if (rb)
		{
			rb.AddForce(currentRockVelocity * 200);
		}
	}
}