using UnityEngine;
using System.Collections;

public class RotateHand : MonoBehaviour
{
	public int torqueX;
	public int torqueY;
	public int torqueZ;
	public Rigidbody rb;

	private float startingXRot;
	private float startingYRot;
	private float startingZRot;

	private void Start()
	{
		Vector3 startingRot = transform.rotation.eulerAngles;
		startingXRot = startingRot.x;
		startingYRot = startingRot.y;
		startingZRot = startingRot.z;
	}

	private void Update ()
	{
		rb.angularVelocity = new Vector3(torqueX, torqueY, torqueZ);
		rb.rotation = Quaternion.Euler(new Vector3(torqueX != 0 ? rb.rotation.x : startingXRot, torqueY != 0 ? rb.rotation.y : startingYRot, torqueZ != 0 ? rb.rotation.z : startingZRot));
    }

}
