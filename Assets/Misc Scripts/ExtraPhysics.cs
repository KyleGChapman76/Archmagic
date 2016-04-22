using UnityEngine;
using System.Collections;

public class ExtraPhysics : MonoBehaviour
{
	public void Knockback (Vector3 force)
	{
		if (gameObject.GetComponent<Rigidbody>() != null)
		{
			gameObject.GetComponent<Rigidbody>().AddForce(force);
		}
	}
}
