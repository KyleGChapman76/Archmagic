using UnityEngine;
using System.Collections;

public class FireballSpell : MonoBehaviour
{
	public CustomizedValue force;
	public CustomizedValue damage;
	public CustomizedValue sizeMult;
	public int ballDuration;
	
	public int stopTicks;

	public GameObject fireballPrefab;
	public float baseExplosionRadius;

	public void Start ()
	{
		//get targeting data
		Vector3 forward = GetComponent<ProjectileTargeting>().GetDirection();
		
		//instantiate a fireball
		Transform playerTransform = transform.root;
		GameObject ball = Instantiate(fireballPrefab, playerTransform.position + Vector3.up + forward*.65f, playerTransform.rotation) as GameObject;
		
		//set the balls size and velocity, etc
		ball.transform.localScale *= sizeMult.GetValue();
		Rigidbody ballRB = ball.GetComponent<Rigidbody>();
        ballRB.AddForce(forward*force.GetValue());
		
		//initialize the projectile properties
		FireballProjectile fireball = ball.GetComponent<FireballProjectile>() as FireballProjectile;
		fireball.damage = (int)damage.GetValue();
		fireball.duration = ballDuration;
		fireball.explosionRadius = baseExplosionRadius * sizeMult.GetValue();
        SphereCollider collider = ball.GetComponent<SphereCollider>() as SphereCollider;
		collider.radius = .5f;
		collider.isTrigger = true;

		//put the player into animation and end the spell
		SpellHandler handler = GetComponentInParent<SpellHandler>();
		handler.ReportFrozenTime(stopTicks);
		Destroy(this.gameObject);
	}
}