using UnityEngine;
using System.Collections;

public class PuffTrigger : MonoBehaviour
{
	public GameObject player;
	public float knockback;
	public float vertFactor;

	public ArrayList objectsPushed;

	public Vector3 originalPosition;

	public float distanceMoved;
	
	public Vector3 direction;

	public int duration;

	private int timer;

	private void Start ()
	{
		timer = duration;
		objectsPushed = new ArrayList();
		originalPosition = transform.position;
	}

	private void FixedUpdate ()
	{
		timer--;
		if (timer <= 0)
		{
			Destroy(gameObject);
			return;
		}
		
		transform.position = transform.position + direction/duration;
		
		distanceMoved = Vector3.Distance(transform.position, originalPosition);
	}

	private void OnTriggerEnter (Collider collider)
	{
		AddObject(collider.gameObject);
	}
	
	private void AddObject (GameObject obj)
	{
		if (obj != player && !objectsPushed.Contains(obj))
		{
			print("Adding " + obj.name);
			PushObject(obj);
			objectsPushed.Add(obj);
		}
	}
	
	private void PushObject (GameObject unit)
	{
		Vector3 knockbackDirection = new Vector3(direction.x, direction.y, direction.z);
		knockbackDirection.Normalize();
		knockbackDirection *= knockback * (1.1f-(distanceMoved/direction.magnitude));
		Vector3 horiz = new Vector3(knockbackDirection.x, 0, knockbackDirection.z);
		Vector3 modifiedKnockback = new Vector3(horiz.x, horiz.magnitude*vertFactor, horiz.z);
		
		//apply the knockback to the target
		FPDPhysics physics = unit.GetComponent<FPDPhysics>();
		if (physics != null)
		{
			physics.Knockback(modifiedKnockback);
		}
		else if (unit.GetComponent<Rigidbody>() != null)
		{
			unit.GetComponent<Rigidbody>().AddForce(modifiedKnockback);
		}
	}
}
