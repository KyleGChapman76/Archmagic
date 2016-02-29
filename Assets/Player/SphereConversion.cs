using UnityEngine;
using System.Collections;

public class SphereConversion : MonoBehaviour
{

	public Transform azimuthLoc;
	public Transform inclinationLoc;
	
	public Vector3 Convert (float radius)
	{
		float azimuth = GetAzimuth();
		float inclination = GetInclination();
		
		azimuth *= -1;
		azimuth += 90;
		if (azimuth < -180)
			azimuth += 360;
		azimuth = azimuth / 180 * Mathf.PI;
		
		inclination += 90;
		if (inclination >= 360)
			inclination -= 360;
		inclination = inclination / 180 * Mathf.PI;
		return new Vector3(Mathf.Sin(inclination)*Mathf.Cos(azimuth), Mathf.Cos(inclination), Mathf.Sin(inclination)*Mathf.Sin(azimuth)) * radius;
	}
	
	public float GetAzimuth ()
	{
		return azimuthLoc.eulerAngles[1];
	}
	
	public float GetInclination ()
	{
		return inclinationLoc.eulerAngles[0];
	}
}
