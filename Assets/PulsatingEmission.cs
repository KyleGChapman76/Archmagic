using UnityEngine;
using System.Collections;

public class PulsatingEmission : MonoBehaviour {

	public Color colorOfEmission;
	private Renderer renderer;
	public float minLight;
	public float maxLight;
	private Light light;
	public float maxEmission;
	public float minEmission;
	public float pulsatingPeriod;
	private float timer;

	// Use this for initialization
	void Start ()
	{
		renderer = GetComponent<Renderer>();
		light = GetComponent<Light>();
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
    }
}
