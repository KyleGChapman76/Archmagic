using UnityEngine;
using System.Collections;

public class RockSpell : MonoBehaviour
{
	public CustomizedValue force;
	public CustomizedValue damage;
	public CustomizedValue sizeMult;

	public float torqueRange;

	public GameObject rockPrefab;

	void Start ()
	{
		//get the targeting data
		Vector3 forward = GetComponent<ProjectileTargeting>().GetDirection();
		forward *= sizeMult.GetValue();
			
		Transform casterTransform = transform.root;
		GameObject boulder = Instantiate(rockPrefab, casterTransform.position + Vector3.up * 1.25f + forward * 1.25f, casterTransform.rotation) as GameObject;

		//set the boulder's velocity, etc
		Rigidbody boulderRB = boulder.GetComponent<Rigidbody>();
		boulderRB.velocity = casterTransform.GetComponent<CharacterController>().velocity + Vector3.up;
		boulder.transform.localScale *= sizeMult.GetValue();
		boulderRB.AddForce(forward*force.GetValue()*2f);
        boulderRB.AddTorque(new Vector3( Random.Range(torqueRange, torqueRange * 2f), Random.Range(-torqueRange, torqueRange), Random.Range(-torqueRange, torqueRange)));

		RockProjectile projectileRock = boulder.GetComponent<RockProjectile>();
		LavaBombProjectile projectileBomb = boulder.GetComponent<LavaBombProjectile>();
		MudBallProjectile mudBallProjectile = boulder.GetComponent<MudBallProjectile>();
		if (projectileRock)
			projectileRock.damage = (int)damage.GetValue();
		if (projectileBomb)
			projectileBomb.damage = (int)damage.GetValue();
		if (mudBallProjectile)
		{
			mudBallProjectile.damage = (int)damage.GetValue();
			mudBallProjectile.slowPercent = 4*(int)damage.GetValue();
		}

		Physics.IgnoreCollision(casterTransform.GetComponent<Collider>(), boulder.GetComponent<Collider>());
	}

}
