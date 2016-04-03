using UnityEngine;
using System.Collections;

public class DOTDebuff : MonoBehaviour
{
	public int duration; //number of seconds the buff lasts
	
	private int ticks;
	private int timer;
	
	private float tickTimeDuration; //the amount of time each tick lasts
	
	public int damage;
	private int damagePerTick;
	
	private Health health;
	
	private float tickTime;
	
	private void Start ()
	{
		health = (Health)GetComponent<Health>();
		ticks = 8;
		tickTimeDuration = duration/(float)ticks;
		damagePerTick = damage/ticks;
		timer = ticks;
	}
	
	private void FixedUpdate ()
	{
		//counts the time elapsed until the next tick is reached
		tickTime += Time.deltaTime;
		if (tickTime < tickTimeDuration)
		{
			return;
		}
		tickTime = 0;
		
		timer--;
		if (timer <= 0)
			Destroy(this);

		health.Damage(damagePerTick);
	}
}
