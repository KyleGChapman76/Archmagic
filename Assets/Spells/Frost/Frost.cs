using UnityEngine;
using System.Collections;

public class Frost : MonoBehaviour
{
	public int damage;
	private float timer;
	public float duration;
	public float increase;

	public float slowTime;
	public float slowdownPercent;

	private void Start()
	{
		timer = 0;
	}

	private void OnCollisionEnter(Collision collision)
	{
		Health health = collision.collider.GetComponent<Health>();
		if (health)
		{
			health.Damage(damage);
		}

		GameObject collider = collision.gameObject;
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
				print("Slowing target for " + slowTime + " at efficacy " + slowdownPercent);
				MovementSlowDebuff debuff = collider.gameObject.AddComponent<MovementSlowDebuff>() as MovementSlowDebuff;
				debuff.duration = slowTime;
				debuff.slowPercent = slowdownPercent;
			}
		}

		if (enabled && !collision.collider.name.Equals(gameObject.name))
			Destroy(gameObject);
	}

	private void Update()
	{
		float inc = Time.deltaTime * increase;
		transform.localScale += new Vector3(inc, inc, inc);

		timer += Time.deltaTime;
		if (timer > duration)
			Destroy(gameObject);
	}
}
