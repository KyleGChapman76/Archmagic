using UnityEngine;
using System.Collections;

public class MudBallProjectile : MonoBehaviour {

	public int damage;
	public float slowPercent;
	public float slowDuration;
	public float explosionSlowPercentModifier;
	public float explosionSlowDurationModifier;
	public float gravityValue;

	public float mudExplosionRadius;

	private Rigidbody rb;

	public GameObject mudPoolPrefab;
	public Vector3 mudPoolDefaultSize;
	public float mudPoolSizeMod;
    public LayerMask groundMask;

	private void FixedUpdate()
	{
		rb = GetComponent<Rigidbody>();
		rb.AddForce(Vector3.down * gravityValue);
	}

	private void OnCollisionEnter(Collision collision)
	{
		Collider collider = collision.collider;

		if (collider.isTrigger)
			return;

		if (enabled)
		{
			RaycastHit hit;
			Physics.Raycast(transform.position + Vector3.up * .5f, Vector3.down, out hit, 3f, groundMask);
			if (hit.point != Vector3.zero)
			{
				GameObject mudPool = Instantiate(mudPoolPrefab, hit.point, Quaternion.Euler(0, Random.Range(0, 360), 0)) as GameObject;
				MudPool poolProperties = mudPool.GetComponent<MudPool>();
				poolProperties.slowdownPercent = slowPercent;
				poolProperties.slowTime = slowDuration;

				mudPool.transform.up = hit.normal;
				mudPool.transform.localScale = mudPoolDefaultSize * mudPoolSizeMod;
			}

			Destroy(gameObject);
		}

		GameObject obj = collider.gameObject;
		Health health = obj.GetComponent<Health>();
		if (health != null)
		{
			health.Damage(damage);
		}

		FPDPhysics physics = collider.GetComponent<FPDPhysics>();
		Rigidbody collisionRB = collider.GetComponent<Rigidbody>();

		Vector3 currentRockVelocity = GetComponent<Rigidbody>().velocity;

		if (physics)
		{
			physics.Knockback(currentRockVelocity * 2);
		}
		else if (collisionRB)
		{
			rb.AddForce(currentRockVelocity * 200);
		}
	}

	private void OnDestroy()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position, mudExplosionRadius);
		foreach (Collider collider in colliders)
		{
			GameObject obj = collider.gameObject;

			FPDPhysics physics = collider.GetComponent<FPDPhysics>();
			if (physics)
			{
				MovementSlowDebuff currentDebuff = collider.GetComponent<MovementSlowDebuff>();
				if (currentDebuff)
				{
					currentDebuff.duration = slowDuration * explosionSlowDurationModifier;
					currentDebuff.slowPercent = slowPercent * explosionSlowPercentModifier;
					currentDebuff.Reset();
				}
				else
				{
					MovementSlowDebuff debuff = collider.gameObject.AddComponent<MovementSlowDebuff>() as MovementSlowDebuff;
					debuff.duration = slowDuration * explosionSlowPercentModifier;
					debuff.slowPercent = slowPercent * explosionSlowPercentModifier;
				}
			}
		}
	}
}
