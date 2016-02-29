using UnityEngine;
using System.Collections;

public class SlipSpell : MonoBehaviour
{
	public CustomizedValue radius;
	public CustomizedValue slipMult;
	public CustomizedValue duration;
	
	public GameObject waterPuddlePrefab;
	
	public float castRange;
	public float baseSlipFactor;
	
	public float seperation;
	
	public int stopTicks;

	public GameObject waterSlipPrefab;

	void Start ()
	{
		//get the targeting data
		RaycastHit hit = GetComponent<AreaTargeting>().GetHit();
		Vector3 position = hit.point;
		
		//initialize
		Vector3 eulers = Quaternion.LookRotation(hit.normal).eulerAngles;
		Quaternion rotation = Quaternion.Euler(new Vector3(eulers.x+90, eulers.y, eulers.z));
		GameObject waterPuddle = Instantiate(waterSlipPrefab, position+seperation*hit.normal, rotation) as GameObject;
		
		//set the waterPuddles size
		float hScale = radius.GetValue()*2f;
		Vector3 newScale = new Vector3(hScale,waterPuddle.transform.localScale.y,hScale);
		waterPuddle.GetComponent<WaterPuddleTrigger>().Rescale(newScale);
		
		//set the puddle trigger's properties
		WaterPuddleTrigger puddleTrigger = waterPuddle.GetComponent<WaterPuddleTrigger>();
		puddleTrigger.slipFactor = baseSlipFactor*slipMult.GetValue();
		puddleTrigger.duration = 60*duration.GetValue();
		
		//put the player into animation and end the spell
		Transform playerTransform = transform.root;
		SpellHandler handler = (SpellHandler)playerTransform.GetComponent<SpellHandler>();
		handler.ReportFrozenTime(stopTicks);
		Destroy(gameObject);
	}
}
