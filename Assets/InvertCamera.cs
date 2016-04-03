using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class InvertCamera : MonoBehaviour
{
	public bool invertX;
	public bool invertY;
    public Camera cameraToInvert; 

	private void Start()
	{
		if (!cameraToInvert)
			cameraToInvert = GetComponent<Camera>();
    }

	private void OnPreCull()
	{
		cameraToInvert.ResetWorldToCameraMatrix();
		cameraToInvert.ResetProjectionMatrix();
		cameraToInvert.projectionMatrix = cameraToInvert.projectionMatrix * Matrix4x4.Scale(new Vector3((invertX ? -1 : 1), (invertY ? -1 : 1), 1));
	}

	private void OnPreRender()
	{
		GL.invertCulling = true;
	}

	private void OnPostRender()
	{
		GL.invertCulling = false;
	}
}
