using UnityEngine;
using System.Collections;

public class DeathPlane : MonoBehaviour
{
	private void OnTriggerEnter(Collider collider)
	{
		Health health = collider.gameObject.GetComponent<Health>();
		if (health)
		{
			health.Kill();
		}
		else
		{
			Destroy(collider.gameObject);
		}
	}
}
