using UnityEngine;
using System.Collections;

public class SmallRock : MonoBehaviour
{
	public float duration;
	private float timer;

	public float fallthroughVelocity;

	// Update is called once per frame
	private void Update ()
	{
		if (timer > 0)
		{
			timer -= Time.deltaTime;

			if (timer <= 0)
				Destroy(gameObject);
		}
    }

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Ground")))
		{
			timer = duration;
			Rigidbody rb = GetComponent<Rigidbody>();
			rb.useGravity = false;
			rb.velocity = Vector3.down * fallthroughVelocity;
			GetComponent<Collider>().enabled = false;
        }
	}
}
