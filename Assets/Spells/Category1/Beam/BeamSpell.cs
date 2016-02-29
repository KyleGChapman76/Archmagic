using UnityEngine;
using System.Collections;

public class BeamSpell : MonoBehaviour
{
	public CustomizedValue range;
	public CustomizedValue radius;
	public CustomizedValue duration;

	private int timer;
	
	private Light light;
	
	private SphereConversion converter;
	
	public int stopTicks;

	private void Start ()
	{
		//set the timer until this is destroyed
		timer = (int)(duration.GetValue()*60);
	
		//enter the player into animation
		Transform playerTransform = transform.root;
		SpellHandler spells = playerTransform.GetComponent<SpellHandler>();
		spells.ReportFrozenTime(stopTicks);
		
		//find and destroy any currently present lights
		Light presentLight = (Light)gameObject.GetComponent<Light>();
		Destroy(presentLight);
		
		//create the new light object on the current object
		light = (Light)gameObject.AddComponent<Light>();
		light.type = LightType.Spot;
		light.intensity = 1f;
		light.range = range.GetValue();
		light.spotAngle = radius.GetValue()*30f;
		
		//find the players sphere converter
		converter = (SphereConversion)playerTransform.GetComponent<SphereConversion>();
	}
	
	private void FixedUpdate ()
	{
		//after the duration, destroy this spell
		timer--;
		if (timer <= 0)
		{
			Destroy(gameObject);
		}
		
		transform.localPosition = new Vector3(0,.7f,0);
		
		//rotate our object to change the rotation of the light
		transform.localRotation = Quaternion.Euler(new Vector3(converter.GetInclination(),0,0));
	}
}
