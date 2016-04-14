using UnityEngine;
using System.Collections;

public class RockSpell : MonoBehaviour
{
	public CustomizedValue force;
	public CustomizedValue damage;
	public CustomizedValue sizeMult;
	
	public int stopTicks;

	public GameObject rockPrefab;

	void Start ()
	{
		//get the targeting data
		Vector3 forward = GetComponent<ProjectileTargeting>().GetDirection();
		forward *= sizeMult.GetValue();
			
		//instantiate a purely graphical boulder across the network
		Transform casterTransform = transform.root;
		GameObject boulder = Instantiate(rockPrefab, casterTransform.position + Vector3.up * 1.25f + forward * 1.25f, casterTransform.rotation) as GameObject;

		//set the boulder's velocity, etc
		Rigidbody boulderRB = boulder.GetComponent<Rigidbody>();
		boulderRB.velocity = casterTransform.GetComponent<CharacterController>().velocity;
		boulder.transform.localScale *= sizeMult.GetValue();
		boulderRB.AddForce(forward*force.GetValue()*2f);
		float torqueRange = 100;
        boulderRB.AddRelativeTorque(new Vector3(Random.Range(-torqueRange, torqueRange), Random.Range(-torqueRange, torqueRange), Random.Range(-torqueRange, torqueRange)));

		//add the owner side components
		RockProjectile projectile = boulder.GetComponent<RockProjectile>();
		projectile.damage = (int)damage.GetValue();

		Physics.IgnoreCollision(casterTransform.GetComponent<Collider>(), boulder.GetComponent<Collider>());
	}

}
