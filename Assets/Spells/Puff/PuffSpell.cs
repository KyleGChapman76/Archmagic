using UnityEngine;
using System.Collections;

public class PuffSpell : MonoBehaviour
{
	public CustomizedValue knockback;
	public CustomizedValue range;
	public CustomizedValue radius;
	
	public GameObject triggerPrefab;
	
	public float vertFactor;
	public int triggerDuration;
	
	private void Start ()
	{
		Transform playerTransform = transform.root;
		
		SphereConversion converter = playerTransform.GetComponent<SphereConversion>();
		
		Vector3 forward = converter.Convert(radius.GetValue());
		
		GameObject puffTrigger = (GameObject)Instantiate(triggerPrefab, playerTransform.GetChild(0).position+forward, Quaternion.Euler(forward));
		
		//set the triggers size, etc.
		puffTrigger.transform.localScale = new Vector3(
			puffTrigger.transform.localScale.x*radius.GetValue()*2,
			puffTrigger.transform.localScale.y*radius.GetValue()*2,
			puffTrigger.transform.localScale.y*radius.GetValue()*2);
			
		puffTrigger.transform.rotation = Quaternion.Euler(new Vector3(0, converter.GetAzimuth(), converter.GetInclination()));
		
		//set the triggers properties for pushing objects
		PuffTrigger component = puffTrigger.GetComponent<PuffTrigger>();
		component.player = playerTransform.gameObject;
		component.knockback = knockback.GetValue();
		component.vertFactor = vertFactor;
		component.duration = triggerDuration;
		component.direction = forward*range.GetValue();
		
		//put the player into animation and end the spell
		SpellHandler handler = (SpellHandler)playerTransform.GetComponent<SpellHandler>();
		Destroy(gameObject);
	}
}
