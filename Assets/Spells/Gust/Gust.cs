using UnityEngine;
using System.Collections;

public class Gust : MonoBehaviour
{
	public float timer;
	public float duration;
	public float increase;

	public float horizontalKnockbackFactor;
	public float verticalKnockbackFactor;

	private void Start()
	{
		timer = 0;
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.GetComponent<Gust>())
			return;

		FPDPhysics physics = collider.GetComponent<FPDPhysics>();
		Rigidbody rb = collider.GetComponent<Rigidbody>();

		Vector3 currentGustVelocity = GetComponent<Rigidbody>().velocity;
		currentGustVelocity = new Vector3(currentGustVelocity.x * horizontalKnockbackFactor, currentGustVelocity.y * verticalKnockbackFactor, currentGustVelocity.z);

		if (physics)
		{
			physics.Knockback(currentGustVelocity);
		}
		else if (rb)
		{
			print(collider.name);
			rb.AddForce(currentGustVelocity*20);
        }

		if (enabled)
			Destroy(gameObject);
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