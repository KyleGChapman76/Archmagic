using UnityEngine;
using System.Collections;

public class EssenceMarkerGizmo : MonoBehaviour {

	void OnDrawGizmos()
	{
		Gizmos.color = Color.gray;
		Gizmos.DrawSphere (transform.position, 1);
	}
}
