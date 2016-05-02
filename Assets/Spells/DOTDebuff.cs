using UnityEngine;
using System.Collections;

public class DOTDebuff : MonoBehaviour
{
	public float duration; //number of seconds the buff lasts
	
	private float durationTimer;
	private float damageTimer;

	public float timeBetweenDamages;
	
	public int damagePerTick;
	
	private Health health;
	
	private void Start ()
	{
		health = GetComponent<Health>();
		durationTimer = duration;
		damageTimer = timeBetweenDamages;
    }
	
	private void Update ()
	{
		//counts the time elapsed until the next tick is reached
		damageTimer += Time.deltaTime;
        if (damageTimer > timeBetweenDamages)
		{
			health.Damage(damagePerTick);
			damageTimer = 0;
		}
		
		durationTimer -= Time.deltaTime;
		if (durationTimer <= 0)
			Destroy(this);
	}
}
