using UnityEngine;
using System.Collections;

public class LavaPool : MonoBehaviour
{
	public float duration;
	private float timer;

	public GameObject fireFlarePrefab;
	public float minTimeBetweenFlares;
	public float maxTimeBetweenFlares;
	public float flareSmallScale;
	public float flareLargeScale;
	public float flareHorizVel;
	public float flareVertVel;
	public float flareLaunchHeight;
	public float flareLaunchRadius;
    private float flareTimer;

	public float burnDuration;
	public float timeBetweenBurnDamages;
	public int damageEachBurn;

	private void Start()
	{
		timer = duration;
		flareTimer = (Random.Range(minTimeBetweenFlares, maxTimeBetweenFlares) + Random.Range(minTimeBetweenFlares, maxTimeBetweenFlares))/2f;
    }

	private void Update()
	{
		timer -= Time.deltaTime;

		if (timer <= 0)
		{
			Destroy(gameObject);
		}

		flareTimer -= Time.deltaTime;

		if (flareTimer <= 0)
		{
			float orbitAngle = Random.Range(0f, Mathf.PI*2);
			Vector3 offset = new Vector3(flareLaunchRadius * Mathf.Cos(orbitAngle), flareLaunchHeight, flareLaunchRadius * Mathf.Sin(orbitAngle));
			GameObject flare = Instantiate(fireFlarePrefab, transform.position + offset, Quaternion.identity) as GameObject;

			flare.transform.localScale = Vector3.one * Random.Range(flareSmallScale, flareLargeScale);
			flare.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-flareHorizVel, flareHorizVel), Random.Range(flareVertVel / 2, flareVertVel), Random.Range(-flareHorizVel, flareHorizVel));

			flareTimer = (Random.Range(minTimeBetweenFlares, maxTimeBetweenFlares) + Random.Range(minTimeBetweenFlares, maxTimeBetweenFlares)) / 2f;
		}
	}

	private void OnTriggerStay(Collider collider)
	{
		Health health = collider.gameObject.GetComponent<Health>();
		if (health != null)
		{
			DOTDebuff currentDebuff = collider.GetComponent<DOTDebuff>();
			if (currentDebuff)
			{
				currentDebuff.damagePerTick = damageEachBurn;
				currentDebuff.duration = burnDuration;
				currentDebuff.timeBetweenDamages = timeBetweenBurnDamages;
			}
			else
			{
				DOTDebuff newDebuff = collider.gameObject.AddComponent<DOTDebuff>() as DOTDebuff;
				newDebuff.damagePerTick = damageEachBurn;
				newDebuff.duration = burnDuration;
				newDebuff.timeBetweenDamages = timeBetweenBurnDamages;
			}
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		Health health = collider.gameObject.GetComponent<Health>();
		if (health != null)
		{
			DOTDebuff currentDebuff = collider.GetComponent<DOTDebuff>();
			if (currentDebuff)
			{
				currentDebuff.damagePerTick = damageEachBurn/2;
				currentDebuff.duration = burnDuration;
				currentDebuff.timeBetweenDamages = timeBetweenBurnDamages*2f;
			}
			else
			{
				DOTDebuff newDebuff = collider.gameObject.AddComponent<DOTDebuff>() as DOTDebuff;
				newDebuff.damagePerTick = damageEachBurn/2;
				newDebuff.duration = burnDuration;
				newDebuff.timeBetweenDamages = timeBetweenBurnDamages*2f;
			}
		}
	}
}
