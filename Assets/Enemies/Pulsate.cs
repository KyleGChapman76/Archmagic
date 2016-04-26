using UnityEngine;
using System.Collections;

public class Pulsate : MonoBehaviour {

	public Color colorOfEmission;
	private Renderer renderer;
	public float minLight;
	public float maxLight;
	private Light light;
	public float maxEmission;
	public float minEmission;
	public float minSize;
	public float maxSize;
	public float pulsatingPeriod;
	private float timer;

	private Vector3 originalSize;

	// Use this for initialization
	void Start ()
	{
		renderer = GetComponent<Renderer>();
		light = GetComponent<Light>();
		originalSize = transform.localScale;
    }
	
	// Update is called once per frame
	void Update ()
	{
		float fractionOfPeriod = (Time.time % pulsatingPeriod) / pulsatingPeriod;
		float strength = Mathf.Sin(fractionOfPeriod * 2 * Mathf.PI);

		float emissionStrength = minEmission + (maxEmission - minEmission) * strength;
		Color finalColor = colorOfEmission * Mathf.LinearToGammaSpace(emissionStrength);

		renderer.material.SetColor("_EmissionColor", finalColor);
		light.color = colorOfEmission;
		light.intensity = minLight + (maxLight-minLight)*strength;

		float size = minSize + strength * (maxSize - minSize);
		transform.localScale = originalSize * size;
		;
    }
}
