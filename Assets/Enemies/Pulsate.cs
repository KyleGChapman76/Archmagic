using UnityEngine;
using System.Collections;

public class Pulsate : MonoBehaviour {

	public Color colorOfEmission;
	private Renderer localRenderer;
	public float minLight;
	public float maxLight;
	private Light pulsatingLight;
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
		localRenderer = GetComponent<Renderer>();
		pulsatingLight = GetComponent<Light>();
		originalSize = transform.localScale;
    }
	
	// Update is called once per frame
	void Update ()
	{
		float fractionOfPeriod = (Time.time % pulsatingPeriod) / pulsatingPeriod;
		float strength = Mathf.Sin(fractionOfPeriod * 2 * Mathf.PI);

		float emissionStrength = minEmission + (maxEmission - minEmission) * strength;
		Color finalColor = colorOfEmission * Mathf.LinearToGammaSpace(emissionStrength);

		localRenderer.material.SetColor("_EmissionColor", finalColor);
		pulsatingLight.color = colorOfEmission;
		pulsatingLight.intensity = minLight + (maxLight-minLight)*strength;

		float size = minSize + strength * (maxSize - minSize);

		if (float.IsNaN(originalSize.x) || float.IsNaN(originalSize.y) || float.IsNaN(originalSize.z))
			transform.localScale = originalSize * size;
    }
}
