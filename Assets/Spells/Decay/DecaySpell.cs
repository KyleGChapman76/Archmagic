using UnityEngine;
using System.Collections;

public class DecaySpell : MonoBehaviour
{
	public CustomizedValue damage;
	public CustomizedValue time;
	
	public int stopTicks;
	
	private void Start ()
	{
		//get the targeting data
		GameObject unit = GetComponent<UnitTargeting>().GetUnit();
		
		//if the targeted unit already has the debuff, destroy it
		DOTDebuff currentBuff = (DOTDebuff)unit.GetComponent<DOTDebuff>();
		if (currentBuff != null)
			Destroy(currentBuff);
		
		//place the damaging debuff on the target
		DOTDebuff buff = (DOTDebuff)unit.AddComponent<DOTDebuff>();
		
		//set the debuff's properties
		buff.duration = (int)(time.GetValue()*2);
		buff.damage = (int)damage.GetValue();
		
		//put the player into animation and end the spell
		Transform playerTransform = transform.root;
		SpellHandler handler = (SpellHandler)playerTransform.GetComponent<SpellHandler>();
		handler.ReportFrozenTime(stopTicks);
		Destroy(gameObject);
	}
}
