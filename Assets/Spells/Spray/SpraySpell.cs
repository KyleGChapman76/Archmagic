using UnityEngine;
using System.Collections;

public class SpraySpell : MonoBehaviour
{
	public CustomizedValue force;
	public CustomizedValue slowdown;
	public CustomizedValue sizeMult;
	public float sprayDuration;

	public GameObject waterSprayPrefab;

	public void Start()
	{
		//get targeting data
		Vector3 forward = GetComponent<ProjectileTargeting>().GetDirection();
		Transform casterTransform = transform.root;
		SpellCasterInformation casterInformation = casterTransform.GetComponent<SpellCasterInformation>() as SpellCasterInformation;

		//instantiate a fireball
		GameObject ball = Instantiate(waterSprayPrefab, casterTransform.position + Vector3.up * casterInformation.spellCastingHeight + forward * casterInformation.spellCastingDistance, Quaternion.identity) as GameObject;
		ball.transform.localScale *= sizeMult.GetValue();

		ball.transform.rotation = Quaternion.Euler(casterInformation.GetInclination(), casterInformation.GetAzimuth(), 0);
		ball.GetComponent<Rigidbody>().velocity = casterTransform.GetComponent<FPDPhysics>().GetVelocity()*.6f;

		//initialize the projectile properties
		WaterSprayer sprayer = ball.GetComponent<WaterSprayer>() as WaterSprayer;
		sprayer.slowdownPercent = slowdown.GetValue();
		sprayer.duration = sprayDuration;
		sprayer.forwardsDirection = forward;
        sprayer.damageDealt = (int)force.GetValue();
		sprayer.spraySystem.startSpeed = force.GetValue();

		Physics.IgnoreCollision(ball.GetComponent<Collider>(), casterTransform.GetComponent<Collider>());
	}
}
