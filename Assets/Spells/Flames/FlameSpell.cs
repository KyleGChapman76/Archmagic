using UnityEngine;
using System.Collections;

public class FlameSpell : MonoBehaviour
{
	private float timer;
	
	public float delay;
	private int flameCount;
	private float spread;
	private float force;
	public int damage;
	public float flameDuration;
	public float flameIncrease;

	public CustomizedValue spreadValue;
	public CustomizedValue speedValue;
	public CustomizedValue flameCountValue;


	public GameObject flamePrefab;

	private void Start ()
	{
		timer = 0;
		spread = 1000*spreadValue.GetValue();
		force = speedValue.GetValue();
		flameCount = (int)flameCountValue.GetValue();
		flameIncrease *= spreadValue.GetValue();
	}

	private void Update ()
	{
		timer += Time.deltaTime;
		
		if (timer >= delay)
		{
			timer = 0;
			for (int i = 0; i < flameCount; i++)
			{
				float horizChange = Random.Range(-.1F, .1F);
				float vertChange = Random.Range(-.1F, .1F);

				Transform casterTransform = transform.root.transform;
				SpellCasterInformation casterInformation = casterTransform.GetComponent<SpellCasterInformation>() as SpellCasterInformation;
				Vector3 forward = casterInformation.Convert(1F);
				
				//instantiate a flame
				GameObject flameCube = Instantiate(flamePrefab, casterTransform.position + Vector3.up*(casterInformation.spellCastingHeight+ vertChange) + forward * casterInformation.spellCastingDistance, casterTransform.rotation) as GameObject;

				//set the flames size and velocity, etc
				flameCube.transform.Rotate(casterInformation.GetInclination(), 0, 0);
				flameCube.GetComponent<Rigidbody>().velocity = casterTransform.GetComponent<CharacterController>().velocity;
				flameCube.GetComponent<Rigidbody>().AddRelativeForce(Vector3.up*(20+vertChange*spread));
				flameCube.GetComponent<Rigidbody>().AddRelativeForce(Vector3.right*(horizChange*spread));
				flameCube.GetComponent<Rigidbody>().AddForce(forward*force);

				//set the flames other attributes
				Flame flame = flameCube.GetComponent<Flame>() as Flame;
				flame.damage = damage;
				flame.duration = flameDuration * spreadValue.GetValue();
				flame.increase = flameIncrease;

				Physics.IgnoreCollision(flameCube.GetComponent<Collider>(), casterTransform.GetComponent<Collider>());
			}
		}
	}
}