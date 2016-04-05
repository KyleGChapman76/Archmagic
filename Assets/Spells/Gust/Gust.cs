using UnityEngine;
using System.Collections;

public class Gust : MonoBehaviour
{
	public float timer;
	public float duration;
	public float increase;

	private void Start()
	{
		timer = 0;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (enabled)
			Destroy(gameObject);

		GameObject obj = collision.gameObject;
		if (obj.tag == "Enemy")
		{
			//perform knockback
		}
	}

	private void Update()
	{
		float inc = Time.deltaTime * increase;
		transform.localScale += new Vector3(inc, inc, inc);

		timer += Time.deltaTime;
		if (timer > duration)
			Destroy(gameObject);
	}
}