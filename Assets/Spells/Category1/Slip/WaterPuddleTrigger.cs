using UnityEngine;
using System.Collections;

public class WaterPuddleTrigger : MonoBehaviour
{
	public float slipFactor;
	public float duration;
	
	private float timer;
	
	public GameObject puddlePrefab;
	private GameObject puddle;
	
	private void Start ()
	{
		timer = duration;
		
		puddle = (GameObject)Instantiate(puddlePrefab);
		puddle.transform.parent = transform;
		puddle.transform.localPosition = Vector3.zero;
		puddle.transform.localScale = new Vector3(1f,.00001f,1f);
		puddle.transform.localRotation = Quaternion.Euler(Vector3.zero);
	}

	private void FixedUpdate ()
	{
		timer--;
		if (timer <= 0)
		{
			Destroy(gameObject);
			return;
		}
	}
	
	private void OnTriggerStay (Collider collider)
	{
		if (collider.tag.Equals("Player"))
		{
			print("Found a player!");
			float controlFactor = 1f-slipFactor;
			collider.GetComponent<FPDPhysics>().SetControlFactor(controlFactor);
		}
	}
	
	public void Rescale (Vector3 newScale)
	{
		transform.localScale = newScale;
	}
}
