using UnityEngine;
using System.Collections;

public class FrostSpell : MonoBehaviour
{
	private float timer;

	public float delay;
	private int frostCount;
	private float spread;
	private float force;
	public int damage;
	public float frostDuration;

	public CustomizedValue slowValue;
	public CustomizedValue speedValue;
	public CustomizedValue frostCountValue;

	public GameObject frostPiecePrefab;

	private void Start()
	{
		timer = 0;
		spread = 300 + 100 * frostCountValue.GetPoints();
		force = speedValue.GetValue();
		frostCount = (int)frostCountValue.GetValue();
	}

	private void Update()
	{
		timer += Time.deltaTime;

		if (timer >= delay)
		{
			timer = 0;

			for (int i = 0; i < frostCount; i++)
			{
				float horizChange = Random.Range(-.1F, .1F);
				float vertChange = Random.Range(-.1F, .1F);

				Transform casterTransform = transform.root.transform;
				SpellCasterInformation casterInformation = casterTransform.GetComponent<SpellCasterInformation>() as SpellCasterInformation;
				Vector3 forward = casterInformation.Convert(1F);

				//instantiate a flame
				GameObject frostPiece = Instantiate(frostPiecePrefab, casterTransform.position + Vector3.up * (casterInformation.spellCastingHeight + vertChange) + forward * casterInformation.spellCastingDistance, casterTransform.rotation) as GameObject;

				//set the flames size and velocity, etc
				frostPiece.transform.Rotate(casterInformation.GetInclination(), 0, 0);
				frostPiece.GetComponent<Rigidbody>().velocity = casterTransform.GetComponent<CharacterController>().velocity;
				frostPiece.GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * (20 + vertChange * spread));
				frostPiece.GetComponent<Rigidbody>().AddRelativeForce(Vector3.right * (horizChange * spread));
				frostPiece.GetComponent<Rigidbody>().AddForce(forward * force);

				//set the flames other attributes
				Frost frost = frostPiece.GetComponent<Frost>() as Frost;
				frost.damage = damage;
				frost.duration = frostDuration;
				frost.slowdownPercent = slowValue.GetValue();

				Physics.IgnoreCollision(frostPiece.GetComponent<Collider>(), casterTransform.GetComponent<Collider>());
			}
		}
	}
}
