using UnityEngine;
using System.Collections;

public class HealSpell : MonoBehaviour
{
	public CustomizedValue heal;
	public CustomizedValue time;
	
	private void Start ()
	{
		//get the targeting data
		GameObject unit = GetComponent<UnitTargeting>().GetUnit();
		
		HealingBuff currentBuff = (HealingBuff)unit.AddComponent<HealingBuff>();
		if (currentBuff != null)
			Destroy(currentBuff);
		
		//place the healing buff on the target
		HealingBuff buff = (HealingBuff)unit.AddComponent<HealingBuff>();
		
		//set the buffs properties
		buff.duration = (int)(time.GetValue()*2);
		buff.healing = (int)heal.GetValue();
		
		//put the player into animation and end the spell
		Transform playerTransform = transform.root;
		Destroy(gameObject);
	}
}
