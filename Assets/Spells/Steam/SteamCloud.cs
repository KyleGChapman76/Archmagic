using UnityEngine;
using System.Collections;

public class SteamCloud : MonoBehaviour
{
	public int damage;
	public float timer;
	public float duration;
	public float maxSize;
	public float growthRate;
	public float bouyancy;

	private Rigidbody rb;

	private void Start ()
	{
		timer = 0;
		rb = GetComponent<Rigidbody>();
    }
	
	private void OnCollisionEnter (Collision collision)
	{
		Health health = collision.collider.GetComponent<Health>();
		if (health)
		{
			health.Damage(damage);
        }
	}

	private void Update ()
	{
		rb.AddForceAtPosition(-Physics.gravity * rb.mass * bouyancy * Time.deltaTime, transform.position);

		timer += Time.deltaTime;
		if (timer > duration)
			Destroy(gameObject);
	}

	private void FixedUpdate()
	{
		float newLerpedSize = Mathf.Lerp(transform.localScale.x, maxSize, growthRate);
		transform.localScale = Vector3.one * newLerpedSize;
	}
}