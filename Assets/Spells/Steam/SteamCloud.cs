using UnityEngine;
using System.Collections;

public class SteamCloud : MonoBehaviour
{
	public int damage;
	public float timer;
	public float duration;
	public float increase;

	private void Start ()
	{
		timer = 0;
	}
	
	private void OnCollisionEnter (Collision collision)
	{
		Health health = collision.collider.GetComponent<Health>();
		if (health)
		{
			health.Damage(damage);
        }
		if (enabled && !collision.collider.name.Equals(gameObject.name))
			Destroy(gameObject);
	}

	private void Update ()
	{
		float inc = Time.deltaTime*increase;
		transform.localScale += new Vector3(inc,inc,inc);

		timer += Time.deltaTime;
		if (timer > duration)
			Destroy(gameObject);
	}
}