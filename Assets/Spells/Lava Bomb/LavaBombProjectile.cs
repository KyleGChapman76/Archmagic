using UnityEngine;
using System.Collections;

public class LavaBombProjectile : MonoBehaviour {

	public int damage;
	public float gravityValue;
	public GameObject lavaPoolPrefab;
	public Vector3 lavaPoolDefaultSize;
	public float lavaPoolSizeMod;
	public LayerMask groundMask;

	public float burnDuration;
	public float timeBetweenBurnDamages;
	public int damageEachBurn;

	private Rigidbody rb;

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
			Physics.Raycast(transform.position + Vector3.up*.5f, Vector3.down, out hit, 3f, groundMask);
			if (hit.point != Vector3.zero)
			{
				GameObject lavaPool = Instantiate(lavaPoolPrefab, hit.point, Quaternion.identity) as GameObject;
				LavaPool lavaPoolProperties = lavaPool.GetComponent<LavaPool>();
				lavaPoolProperties.damageEachBurn = damageEachBurn;
				lavaPoolProperties.burnDuration = burnDuration;
				lavaPoolProperties.timeBetweenBurnDamages = .1f;

				lavaPool.transform.up = hit.normal;
				lavaPool.transform.localScale = lavaPoolDefaultSize * lavaPoolSizeMod;
            }

			Destroy(gameObject);
		}

		GameObject obj = collider.gameObject;
		Health health = obj.GetComponent<Health>();
		if (health != null)
		{
			health.Damage(damage);

			DOTDebuff currentDebuff = collision.collider.GetComponent<DOTDebuff>();
			if (currentDebuff)
			{
				currentDebuff.damagePerTick = damageEachBurn;
				currentDebuff.duration = burnDuration;
				currentDebuff.timeBetweenDamages = timeBetweenBurnDamages;
			}
			else
			{
				DOTDebuff newDebuff = collision.gameObject.AddComponent<DOTDebuff>() as DOTDebuff;
				newDebuff.damagePerTick = damageEachBurn;
				newDebuff.duration = burnDuration;
				newDebuff.timeBetweenDamages = timeBetweenBurnDamages;
			}
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
}
