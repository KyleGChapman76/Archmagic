using UnityEngine;
using System.Collections;

public class Flame : MonoBehaviour
{
	public float damagePerHit;
	public float timer;
	public float duration;
	public float increase;

	public float burnDuration;
	public float timeBetweenBurnDamages;
	public int damageEachBurn;

	private void Start ()
	{
		timer = 0;
	}
	
	private void OnCollisionEnter (Collision collision)
	{
		Health health = collision.collider.GetComponent<Health>();
		if (health)
		{
			if (Random.Range(0, 1) < damagePerHit)
				health.Damage(1);

			DOTDebuff currentDebuff = collision.collider.GetComponent<DOTDebuff>();
			if (currentDebuff)
			{
				currentDebuff.damagePerTick = 1;
				currentDebuff.duration = burnDuration;
				currentDebuff.timeBetweenDamages = timeBetweenBurnDamages;
            }
			else
			{
				DOTDebuff newDebuff = collision.gameObject.AddComponent<DOTDebuff>() as DOTDebuff;
				newDebuff.damagePerTick = damageEachBurn;
				newDebuff.duration = burnDuration;
				newDebuff.timeBetweenDamages = timeBetweenBurnDamages;
			}
		}
		if (enabled && !collision.collider.name.Equals(gameObject.name))
			Destroy(gameObject);
	}

	private void Update ()
	{
		float inc = Time.deltaTime*increase;
		transform.localScale += new Vector3(inc,inc,inc);

		timer += Time.deltaTime;
		if (timer > duration)
			Destroy(gameObject);
	}
}