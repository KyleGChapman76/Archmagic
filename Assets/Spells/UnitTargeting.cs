using UnityEngine;
using System.Collections;

public class UnitTargeting : MonoBehaviour
{
	public const SpellTargetingType targetType = SpellTargetingType.Unit;
	
	public CustomizedValue targetRange;
	public float baseRange;
	
	private GameObject unit;
	
	public string[] validTags;
	
	public string[] invalidTags;
	
	public string[] validNames;
	
	public string[] invalidNames;
	
	public string[] neccesaryComponents;
	
	public string[] validComponents;
	
	public string[] forbiddenComponents;
	
	public void SetUnit (GameObject unit)
	{
		this.unit = unit;
	}
	
	public GameObject GetUnit ()
	{
		return unit;
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
	
	public bool HasValidComponents (GameObject obj)
	{
		//test if has all of the valid components
		bool hasNeccesary = true;
		foreach (string s in neccesaryComponents)
		{
			if (obj.GetComponent(s) == null)
			{
				print("Does not have neccesary component " + s);
				hasNeccesary = false;
				break;
			}	
		}
		
		if (!hasNeccesary)
			return false;
		
		//test if has any of the forbidden components
		bool hasForbidden = false;
		foreach (string s in forbiddenComponents)
		{
			if (obj.GetComponent(s) != null)
			{
				print("Has forbidden component " + s);
				hasForbidden = true;
				break;
			}	
		}
		
		if (hasForbidden)
			return false;
		
		if (validComponents.Length == 0)
			return true;
		
		//test if has any of the valid components
		foreach (string s in validComponents)
		{
			if (obj.GetComponent(s) != null)
			{
				return true;
			}	
		}
		
		print("Doesn't have valid component.");
		
		return false;
	}
}
