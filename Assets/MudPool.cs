using UnityEngine;
using System.Collections;

public class MudPool : MonoBehaviour
{
	public float slowTime;
	public float slowdownPercent;

	private void OnTriggerStay(Collider collider)
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
		}
	}
}
