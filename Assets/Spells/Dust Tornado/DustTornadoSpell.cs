using UnityEngine;
using System.Collections;

public class DustTornadoSpell : MonoBehaviour
{
	public CustomizedValue distance;
	public CustomizedValue power;
	public CustomizedValue size;

	public GameObject tornadoPrefab;

	public int baseDamagePerHit;
	
	private void Start ()
	{
		//get targeting data
		Vector3 forward = GetComponent<ProjectileTargeting>().GetDirection();
		Transform casterTransform = transform.root.transform;
		SpellCasterInformation casterInformation = casterTransform.GetComponent<SpellCasterInformation>() as SpellCasterInformation;

		GameObject tornado = Instantiate(tornadoPrefab, casterTransform.position + Vector3.up * (casterInformation.spellCastingHeight + size.GetValue()/2f) + forward * casterInformation.spellCastingDistance, casterTransform.rotation) as GameObject;

		//set the tornados size and velocity, etc
		tornado.transform.localScale *= size.GetValue();
		Rigidbody ballRB = tornado.GetComponent<Rigidbody>();
		ballRB.AddForce(forward * power.GetValue());

		//set the tornados properties
		DustTornado dustTornado = tornado.GetComponent<DustTornado>();
		dustTornado.damagePerHit = (int)(baseDamagePerHit + baseDamagePerHit * .3f * power.GetPoints());
		dustTornado.duration = distance.GetValue();
		dustTornado.knockback = 1 + power.GetPoints()*.3f;

		Physics.IgnoreCollision(tornado.GetComponent<Collider>(), casterTransform.GetComponent<Collider>());
	}
}
