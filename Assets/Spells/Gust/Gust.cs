using UnityEngine;
using System.Collections;

public class Gust : MonoBehaviour
{
	public float timer;
	public float duration;
	public float increase;

	public float horizontalKnockbackFactor;
	public float verticalKnockbackFactor;

	public float damagePerHit;

	public float velConservationOnHit;
	public float velDeathThreshold;

	private Rigidbody rb;

	private void Start()
	{
		timer = 0;
		rb = GetComponent<Rigidbody>();
    }

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.GetComponent<Gust>())
			return;

		FPDPhysics physics = collider.GetComponent<FPDPhysics>();
		Rigidbody colliderRB = collider.GetComponent<Rigidbody>();

		Vector3 currentGustVelocity = rb.velocity;
		currentGustVelocity = new Vector3(currentGustVelocity.x * horizontalKnockbackFactor, currentGustVelocity.y * verticalKnockbackFactor, currentGustVelocity.z * horizontalKnockbackFactor);

		if (physics)
		{
			physics.Knockback(currentGustVelocity);
		}
		else if (colliderRB)
		{
			colliderRB.AddForce(currentGustVelocity*20);
        }

		Health health = collider.GetComponent<Health>();
		if (health != null)
		{	
			if (Random.Range(0,100) < damagePerHit*100)
				health.Damage(1);
		}


		rb.velocity = rb.velocity * velConservationOnHit;
		if (enabled && rb.velocity.magnitude < velDeathThreshold)
		{
			Destroy(gameObject);
		}
	}

	private void Update()
	{
		float inc = Time.deltaTime * increase;
		transform.localScale += new Vector3(inc, inc, inc);

		timer += Time.deltaTime;
		if (timer > duration)
		{
			gameObject.GetComponent<ParticleSystem>().Stop();
			Destroy(gameObject);
		}
	}
}