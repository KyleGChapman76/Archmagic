using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class FireballSpell : NetworkBehaviour
{
	public CustomizedValue force;
	public CustomizedValue damage;
	public CustomizedValue sizeMult;
	public int ballDuration;

	public GameObject fireballPrefab;
	public float baseExplosionRadius;

	public void Start ()
	{
		if (!isLocalPlayer)
		{
			Destroy(this);
			return;
		}
		//get targeting data
		Vector3 forward = GetComponent<ProjectileTargeting>().GetDirection();
		Transform casterTransform = transform.root.transform;
		SpellCasterInformation casterInformation = casterTransform.GetComponent<SpellCasterInformation>() as SpellCasterInformation;

		//instantiate a fireball
		GameObject ball = Instantiate(fireballPrefab, casterTransform.position + Vector3.up * casterInformation.spellCastingHeight + forward * casterInformation.spellCastingDistance, casterTransform.rotation) as GameObject;

		//set the balls size and velocity, etc
		ball.transform.localScale *= sizeMult.GetValue();
		Rigidbody ballRB = ball.GetComponent<Rigidbody>();
		ballRB.AddForce(forward * force.GetValue());

		//initialize the projectile properties
		FireballProjectile fireball = ball.GetComponent<FireballProjectile>() as FireballProjectile;
		fireball.damage = (int)damage.GetValue();
		fireball.duration = ballDuration;
		fireball.explosionRadius = baseExplosionRadius * sizeMult.GetValue();
		fireball.damageEachBurn = fireball.damage / 4;

		Physics.IgnoreCollision(ball.GetComponent<Collider>(), casterTransform.GetComponent<Collider>());
	}
}