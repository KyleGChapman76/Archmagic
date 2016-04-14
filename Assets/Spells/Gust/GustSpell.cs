using UnityEngine;
using System.Collections;

public class GustSpell : MonoBehaviour
{
	private float timer;

	public float delay;
	public int windGustsCount;
	private float spread;
	private float force;
	public float gustDuration;
	public float gustIncrease;

	public CustomizedValue spreadValue;
	public CustomizedValue speedValue;
	public CustomizedValue rangeValue;

	public GameObject gustPrefab;

	private void Start()
	{
		timer = 0;
		spread = 1000 * spreadValue.GetValue();
		force = speedValue.GetValue();
		gustIncrease *= spreadValue.GetValue();
	}

	private void Update()
	{
		timer += Time.deltaTime;
		for (int i = 0; i < windGustsCount; i++)
		{
			if (timer >= delay)
			{
				timer = 0;

				float horizChange = Random.Range(-.1F, .1F);
				float vertChange = Random.Range(-.1F, .1F);

				Transform casterTransform = this.transform.root;
				SpellCasterInformation casterInformation = casterTransform.GetComponent<SpellCasterInformation>();
				Vector3 forward = casterInformation.Convert(1F);

				//instantiate a flame
				GameObject gustParticle = Instantiate(gustPrefab, casterTransform.position + Vector3.up * (1 + vertChange) + forward, casterTransform.rotation) as GameObject;
				gustParticle.transform.localScale = Vector3.one * (.4f + Random.Range(0, .25f));

				//set the flames size and velocity, etc
				gustParticle.transform.Rotate(casterInformation.GetInclination(), 0, 0);
				gustParticle.GetComponent<Rigidbody>().velocity = casterTransform.GetComponent<CharacterController>().velocity;
				gustParticle.GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * (20 + vertChange * spread));
				gustParticle.GetComponent<Rigidbody>().AddRelativeForce(Vector3.right * (horizChange * spread));
				gustParticle.GetComponent<Rigidbody>().AddForce(forward * force);

				//set the flames other attributes
				Gust gust = gustParticle.GetComponent<Gust>() as Gust;
				gust.duration = gustDuration * (int)rangeValue.GetValue();
				gust.increase = gustIncrease;

				Physics.IgnoreCollision(gust.GetComponent<Collider>(), casterTransform.GetComponent<Collider>());
			}
		}
	}
}