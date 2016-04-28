using UnityEngine;
using System.Collections;

public class DustTornado : MonoBehaviour
{
	public int damagePerHit;
	public float knockback;

	public float duration;
	public float timer;

	public float horizontalKnockbackFactor;
	public float verticalKnockbackFactor;

	private Rigidbody rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		timer = duration;
    }

	private void Update()
	{
		timer -= Time.deltaTime;

		if (timer <= 0)
		{
			Destroy(gameObject);
		}
    }

	private void OnTriggerStay (Collider other)
	{
		Vector3 currentGustMomentum = rb.velocity * rb.mass;
		currentGustMomentum = new Vector3(currentGustMomentum.x * horizontalKnockbackFactor, currentGustMomentum.y * verticalKnockbackFactor, currentGustMomentum.z * horizontalKnockbackFactor);
	
		FPDPhysics physics = other.GetComponent<FPDPhysics>();
		Rigidbody colliderRB = other.GetComponent<Rigidbody>();

		Vector3 knockbackDirection = currentGustMomentum + (other.transform.position - transform.position).normalized;

		if (physics)
		{
			physics.Knockback(knockbackDirection * knockback);
		}
		else if (colliderRB)
		{
			colliderRB.AddForce(knockbackDirection * knockback * 20);
		}

		Health health = other.GetComponent<Health>();
		if (health != null)
		{
			health.Damage(damagePerHit);
		}
	}
}
