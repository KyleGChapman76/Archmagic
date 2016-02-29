using UnityEngine;
using System.Collections;

public class FireballExplosion : MonoBehaviour
{
	public ParticleSystem particleSystem;

	private void Update ()
	{
		if (!particleSystem || !particleSystem.IsAlive())
		{
			Destroy(gameObject);
		}
	}
}
