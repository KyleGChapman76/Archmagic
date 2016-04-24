using UnityEngine;
using System.Collections;

public class FireballExplosion : MonoBehaviour
{
	public ParticleSystem particles;

	private void Start()
	{
		particles = GetComponent<ParticleSystem>();
    }
		
	private void Update ()
	{
		if (!particles || !particles.IsAlive())
		{
			Destroy(gameObject);
		}
	}
}
