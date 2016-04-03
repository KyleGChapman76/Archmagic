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
		Transform playerTransform = transform.root;
		GameObject boulder = Instantiate(rockPrefab, playerTransform.position + Vector3.up + forward, playerTransform.rotation) as GameObject;
		
		//set the boulder's velocity, etc
		boulder.GetComponent<Rigidbody>().velocity = playerTransform.GetComponent<CharacterController>().velocity;
		boulder.transform.localScale *= sizeMult.GetValue();
		boulder.GetComponent<Rigidbody>().AddForce(forward*force.GetValue());
		
		//add the owner side components
		RockProjectile projectile = (RockProjectile)boulder.AddComponent<RockProjectile>();
		projectile.damage = (int)damage.GetValue();
		
		SphereCollider collider = (SphereCollider)boulder.AddComponent<SphereCollider>();
		collider.radius = .5f;
		collider.isTrigger = true;
		
		//put the player into animation and end the spell
		SpellHandler handler = (SpellHandler)playerTransform.GetComponent<SpellHandler>();
		handler.ReportFrozenTime(stopTicks);
		Destroy(this.gameObject);
	}
}
