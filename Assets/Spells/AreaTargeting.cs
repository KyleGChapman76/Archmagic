using UnityEngine;
using System.Collections;

public class AreaTargeting : MonoBehaviour
{
	public const SpellTargetingType targetType = SpellTargetingType.Area;
	
	public CustomizedValue targetRange;
	public float baseRange;
	
	public CustomizedValue targetRadius;
	public float baseRadius;
	
	public float maxAngleFromUp;
	
	private RaycastHit hit;
	
	public string[] validTags;
	
	public string[] invalidTags;
	
	public string[] validNames;
	
	public string[] invalidNames;
	
	public void SetHit (RaycastHit hit)
	{
		this.hit = hit;
	}
	
	public RaycastHit GetHit ()
	{
		return hit;
	}

	public float GetBaseRange()
	{
		return baseRange;
	}

	public float GetRange (int[] pointsAllotted)
	{
		if (targetRange == null)
			return baseRange;
		
		return targetRange.GetValue(pointsAllotted[targetRange.index]);
	}

	public float GetBaseRadius()
	{
		return baseRadius;
	}

	public float GetRadius (int[] pointsAllotted)
	{
		if (targetRadius == null)
			return baseRadius;
		
		return targetRadius.GetValue(pointsAllotted[targetRadius.index]);
	}
	
	public bool IsValidTag (string tag)
	{
		return (ArrayUtility.Contains(validTags,tag) || validTags.Length == 0)
			&&  !ArrayUtility.Contains(invalidTags,tag);
	}
	
	public bool IsValidName (string name)
	{
		return (ArrayUtility.Contains(validNames,name) || validNames.Length == 0)
			&&  !ArrayUtility.Contains(invalidNames,name);
	}
	
	public bool IsValidNormal (Vector3 normal)
	{
		float angle = Vector3.Angle(Vector3.up, normal);
		return maxAngleFromUp > 180 || maxAngleFromUp < 0 || angle <= maxAngleFromUp;
	}
}
