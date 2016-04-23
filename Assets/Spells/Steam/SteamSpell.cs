using UnityEngine;
using System.Collections;

public class SteamSpell : MonoBehaviour
{
	private float timer;
	
	public float delay;
	private int flameCount;
	private float spread;
	private float force;
	public int damage;
	public float flameDuration;

	public float maxSize;

	public CustomizedValue spreadValue;
	public CustomizedValue speedValue;
	public CustomizedValue flameCountValue;


	public GameObject flamePrefab;

	private void Start ()
	{
		timer = 0;
		spread = 300*spreadValue.GetValue();
		force = speedValue.GetValue();
		flameCount = (int)flameCountValue.GetValue();
		maxSize *= spreadValue.GetValue();
	}

	private void Update ()
	{
		timer += Time.deltaTime;
		for (int i = 0;i<flameCount;i++)
		{
			if (timer >= delay)
			{
				timer = 0;

				Transform casterTransform = transform.root.transform;
				SpellCasterInformation casterInformation = casterTransform.GetComponent<SpellCasterInformation>() as SpellCasterInformation;
				Vector3 forward = casterInformation.Convert(1F);
				
				//instantiate a flam
				GameObject steamCloudObject = Instantiate(flamePrefab, casterTransform.position + Vector3.up*casterInformation.spellCastingHeight + forward * casterInformation.spellCastingDistance, casterTransform.rotation) as GameObject;

				//set the flames size and velocity, etc
				steamCloudObject.transform.Rotate(casterInformation.GetInclination(), 0, 0);
				steamCloudObject.GetComponent<Rigidbody>().velocity = casterTransform.GetComponent<CharacterController>().velocity;
				steamCloudObject.GetComponent<Rigidbody>().AddForce(forward*force);

				//set the flames other attributes
				SteamCloud steamCloud = steamCloudObject.GetComponent<SteamCloud>() as SteamCloud;
				steamCloud.damage = damage;
				steamCloud.duration = flameDuration * spreadValue.GetValue();
				steamCloud.maxSize = maxSize;

				Physics.IgnoreCollision(steamCloudObject.GetComponent<Collider>(), casterTransform.GetComponent<Collider>());
			}
		}
	}
}