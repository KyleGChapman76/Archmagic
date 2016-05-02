using UnityEngine;
using System.Collections;

public class WaterSprayer : MonoBehaviour {

	public float slowdownPercent;
	public float duration;
	public int damageDealt;
	public Vector3 forwardsDirection;
	private float timer;
	public ParticleSystem spraySystem;
	public ArrayList objectsPushed;
	public float vertFactor = .4f;
	public float slowTime = 2f;

	private void Start()
	{
		timer = duration;
		objectsPushed = new ArrayList();
		GetComponent<Rigidbody>().velocity = forwardsDirection;
	}

	private void FixedUpdate()
	{
		timer -= Time.deltaTime;
		if (timer <= 0 && duration != 0)
			Destroy(gameObject);
	}

	private void OnTriggerEnter(Collider collider)
	{
		FPDPhysics physics = collider.GetComponent<FPDPhysics>();
		if (physics)
		{
			MovementSlowDebuff currentDebuff = collider.GetComponent<MovementSlowDebuff>();
			if (currentDebuff)
			{
				currentDebuff.duration = slowTime;
				currentDebuff.slowPercent = slowdownPercent;
				currentDebuff.Reset();
			}
			else
			{
				MovementSlowDebuff debuff = collider.gameObject.AddComponent<MovementSlowDebuff>() as MovementSlowDebuff;
				debuff.duration = slowTime;
				debuff.slowPercent = slowdownPercent;
			}
			AddObject(collider.gameObject);
		}

		Health health = collider.GetComponent<Health>();
		if (health)
			health.Damage(damageDealt);
    }

	private void AddObject(GameObject obj)
	{
		if (!objectsPushed.Contains(obj))
		{
			PushObject(obj);
			objectsPushed.Add(obj);
		}
	}

	private void PushObject(GameObject unit)
	{
		Vector3 knockbackDirection = forwardsDirection;
		Vector3 horiz = new Vector3(knockbackDirection.x, 0, knockbackDirection.z);
		Vector3 modifiedKnockback = new Vector3(horiz.x, horiz.magnitude * vertFactor, horiz.z);

		//apply the knockback to the target
		FPDPhysics physics = unit.GetComponent<FPDPhysics>();
		if (physics != null)
		{
			physics.Knockback(modifiedKnockback);
		}
		else if (unit.GetComponent<Rigidbody>() != null)
		{
			unit.GetComponent<Rigidbody>().AddForce(modifiedKnockback);
		}
	}
}
