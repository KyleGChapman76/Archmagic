using UnityEngine;
using System.Collections;

public class MovementSlowDebuff : MonoBehaviour
{
	//number of seconds the buff lasts
	public float duration;
	
	//by what percent the entity is slowed
	public float slowPercent;
	
	public FPDPhysics physics;
	
	private float timer;
	
	private void Start ()
	{
		timer = duration;
		physics = GetComponent<FPDPhysics>() as FPDPhysics;
		
		physics.ChangeMultiplicativeMovementSpeed(-slowPercent/100f);
	}
	
	private void Update ()
	{
		timer -= Time.deltaTime;
		if (timer <= 0)
		{
			physics.ChangeMultiplicativeMovementSpeed(slowPercent/100f);
			Destroy(this);
		}
	}

	public void Reset()
	{
		physics.ChangeMultiplicativeMovementSpeed(slowPercent / 100f);
		Start();
	}
}
