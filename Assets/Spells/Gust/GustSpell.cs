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

				Transform playerTransform = this.transform.root;
				SphereConversion converter = playerTransform.GetComponent<SphereConversion>();
				Vector3 forward = converter.Convert(1F);

				//instantiate a flame
				GameObject flameCube = Instantiate(gustPrefab, playerTransform.position + Vector3.up * (1 + vertChange) + forward, playerTransform.rotation) as GameObject;

				//set the flames size and velocity, etc
				flameCube.transform.Rotate(converter.GetInclination(), 0, 0);
				flameCube.GetComponent<Rigidbody>().velocity = playerTransform.GetComponent<CharacterController>().velocity;
				flameCube.GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * (20 + vertChange * spread));
				flameCube.GetComponent<Rigidbody>().AddRelativeForce(Vector3.right * (horizChange * spread));
				flameCube.GetComponent<Rigidbody>().AddForce(forward * force);

				//set the flames other attributes
				Gust gust = flameCube.GetComponent<Gust>() as Gust;
				gust.duration = gustDuration * (int)rangeValue.GetValue();
				gust.increase = gustIncrease;
			}
		}
	}
}