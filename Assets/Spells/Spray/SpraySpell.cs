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

		//instantiate a fireball
		Transform playerTransform = transform.root;
		GameObject ball = Instantiate(waterSprayPrefab, playerTransform.position + Vector3.up + forward * .65f, Quaternion.identity) as GameObject;
		ball.transform.localScale *= sizeMult.GetValue();

		SphereConversion sphereConversion = playerTransform.GetComponent<SphereConversion>();
		ball.transform.rotation = Quaternion.Euler(sphereConversion.GetInclination(), sphereConversion.GetAzimuth(), 0);
		ball.GetComponent<Rigidbody>().velocity = playerTransform.GetComponent<FPDPhysics>().GetVelocity()*.6f;

		//initialize the projectile properties
		WaterSprayer sprayer = ball.GetComponent<WaterSprayer>() as WaterSprayer;
		sprayer.slowdownPercent = slowdown.GetValue();
		sprayer.duration = sprayDuration;
		sprayer.forwardsDirection = forward;
        sprayer.damageDealt = (int)force.GetValue();
		sprayer.spraySystem.startSpeed = force.GetValue();
	}
}
