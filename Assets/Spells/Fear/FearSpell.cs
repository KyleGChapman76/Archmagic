using UnityEngine;
using System.Collections;

public class FearSpell : MonoBehaviour
{
	public CustomizedValue slowPercent;
	public CustomizedValue duration;

	void Start ()
	{
		//get the targeting data
		GameObject unit = GetComponent<UnitTargeting>().GetUnit();
		
		//apply the fear debuff
		MovementSlowDebuff currentBuff = (MovementSlowDebuff)unit.GetComponent<MovementSlowDebuff>();
		if (currentBuff != null)
			Destroy(currentBuff);
			
		//place the fear debuff on the target
		MovementSlowDebuff debuff = (MovementSlowDebuff)unit.AddComponent<MovementSlowDebuff>();
		
		//set the debuffs properties
		debuff.duration = (int)(duration.GetValue());
		debuff.slowPercent = (float)(slowPercent.GetValue());;
		
		//put the player into animation and end the spell
		Transform playerTransform = transform.root;
		Destroy(gameObject);
	}
}
