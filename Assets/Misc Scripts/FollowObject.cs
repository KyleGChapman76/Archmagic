using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour
{
	public GameObject objectToFollow;

	// Update is called once per frame
	void Update ()
	{
		transform.position = objectToFollow.transform.position;
	}
}
