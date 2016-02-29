using UnityEngine;
using System.Collections;

public class SimpleEnemyAI : MonoBehaviour
{
	public float aggroDistance;

	private GameObject playerTarget;

	private void Start ()
	{
		
	}
	
	private void Update ()
	{
		
	}
	
	private void FixedUpdate ()
	{
		GameObject propsectiveTarget = GameObject.FindGameObjectWithTag("Player");
		if (propsectiveTarget != null && Vector3.Distance(transform.position, propsectiveTarget.transform.position) <= aggroDistance)
		{
			playerTarget = propsectiveTarget;
		}
		
		if (playerTarget != null)
		{
			transform.LookAt(playerTarget.transform.position);
			transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
		}
	}
	
	public Vector3 GetDesiredDirection ()
	{
		if (playerTarget == null)
			return Vector3.zero;
		
		Vector3 v = Vector3.MoveTowards(transform.position, playerTarget.transform.position, 1);
		v.Normalize();
		print(v);
		
		return v;
	}
	
	public float GetInputX ()
	{
		return 0f;
	}
	
	public float GetInputY ()
	{
		return 1f;
	}
	
	public bool IsWalking ()
	{
		return false;
	}
	
	public bool IsJumping ()
	{
		return false;
	}
}
