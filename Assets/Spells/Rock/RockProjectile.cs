using UnityEngine;
using System.Collections;

public class RockProjectile : MonoBehaviour
{
	public int damage;
	public float gravityValue;
	public GameObject rockDustExplosionPrefab;
	public GameObject smallRockPrefab;
	public float smallRockSmallScale;
	public float smallRockLargeScale;
	public float smallRockHorizVel;
	public float smallRockVertVel;
	public float minNumRocks;
	public float maxNumRocks;
	public float velocityInheritance;

	private Rigidbody rb;

	private void FixedUpdate()
	{
		rb = GetComponent<Rigidbody>();
		rb.AddForce(Vector3.down * gravityValue);
	}

	private void OnCollisionEnter (Collision collision)
	{
		Collider collider = collision.collider;

		if (collider.isTrigger)
			return;

		if (enabled)
		{
			GameObject dustExplosion = Instantiate(rockDustExplosionPrefab, transform.position, Quaternion.identity) as GameObject;
			dustExplosion.GetComponent<Rigidbody>().velocity = new Vector3(rb.velocity.x, 1 ,rb.velocity.z) * .1f;

			int numRocks = (int)Random.Range(minNumRocks, maxNumRocks);
			for (int i = 0; i < numRocks; i++)
			{
				GameObject smallRock = Instantiate(smallRockPrefab, transform.position, Quaternion.identity) as GameObject;
				smallRock.transform.position = transform.position + Vector3.up * .2f;
				smallRock.transform.localScale = new Vector3(Random.Range(smallRockSmallScale, smallRockLargeScale), Random.Range(smallRockSmallScale, smallRockLargeScale), Random.Range(smallRockSmallScale, smallRockLargeScale));
				smallRock.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-smallRockHorizVel, smallRockHorizVel) + rb.velocity.x * velocityInheritance, Random.Range(smallRockVertVel / 2, smallRockVertVel), Random.Range(-smallRockHorizVel, smallRockHorizVel) + rb.velocity.z*velocityInheritance);
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
}