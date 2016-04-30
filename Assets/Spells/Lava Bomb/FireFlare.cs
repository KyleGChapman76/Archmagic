using UnityEngine;
using System.Collections;

public class FireFlare : MonoBehaviour
{
	public float gravityValue;
	private Rigidbody rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		rb.AddForce(Vector3.down * gravityValue);
    }

	private void OnCollisionEnter(Collision collision)
	{
		if (enabled && !collision.collider.isTrigger)
		{
			Destroy(gameObject);
		}
	}
}
