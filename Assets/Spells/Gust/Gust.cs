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

		if (enabled)
			Destroy(gameObject);

		if (collider.tag == "Enemy")
		{
			FPDPhysics physics = collider.GetComponent<FPDPhysics>();
			Vector3 currentGustVelocity = GetComponent<Rigidbody>().velocity;
			currentGustVelocity = new Vector3(currentGustVelocity.x*horizontalKnockbackFactor, currentGustVelocity.y * verticalKnockbackFactor, currentGustVelocity.z);
			physics.Knockback(currentGustVelocity);
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