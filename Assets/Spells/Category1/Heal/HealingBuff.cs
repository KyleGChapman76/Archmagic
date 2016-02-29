using UnityEngine;
using System.Collections;

public class HealingBuff : MonoBehaviour
{
	public int duration; //number of seconds the buff lasts
	
	private int ticks;
	private int timer;
	
	private float tickTimeDuration; //the amount of time each tick lasts
	
	public int healing;
	private int healingPerTick;
	
	private Health health;
	
	private float tickTime;

	private void Start ()
	{
		health = (Health)GetComponent<Health>();
		ticks = 8;
		tickTimeDuration = duration/(float)ticks;
		healingPerTick = healing/ticks;
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

		health.Heal(healingPerTick);
	}
}
