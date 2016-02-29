using UnityEngine;
using System.Collections;

public class CustomizedValue : MonoBehaviour
{
	public int index;
	public string description;
	public float baseValue;
	public float growthValue;
	
	private int pointsInvested;
	
	public void Start ()
	{
		if (GetComponents<CustomizedValue>().Length > Scroll.CUSTOM_VALUES)
			Debug.LogError("Too many custom values!");
	}
	
	public bool SetPoints (int points)
	{
		if (points > Scroll.MAX_POINTS || points < 0)
		{
			print("That's too many points!");
			return false;
		}
		else
		{
			pointsInvested = points;
			return true;
		}
	}
	
	public int GetPoints ()
	{
		return pointsInvested;
	}
	
	public float GetValue ()
	{
		return baseValue + growthValue * pointsInvested;
	}
	
	public float GetValue (int points)
	{
		return baseValue + growthValue * points;
	}
}
