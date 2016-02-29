using UnityEngine;
using System.Collections;

public class MovementSlowDebuff : MonoBehaviour
{
	public int duration; //number of seconds the buff lasts
	
	public float slowPercent;
	
	public FPDPhysics physics;
	
	private int timer;
	
	private void Start ()
	{
		timer = duration*60;
		
		physics = (FPDPhysics)GetComponent<FPDPhysics>();
		
		physics.changeMultiplicativeMovementSpeed(-slowPercent/100f);
	}
	
	private void FixedUpdate ()
	{
		timer--;
		if (timer <= 0)
		{
			physics.changeMultiplicativeMovementSpeed(slowPercent/100f);
			Destroy(this);
		}
	}
}
