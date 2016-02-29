using UnityEngine;
using System.Collections;

public class EssenceInfo : MonoBehaviour {

	public static string[] essenceNames = {"Fire", "Water", "Earth", "Air", "Light", "Dark", "Energy", "Death"};

	public int code;

	public string GetItemName ()
	{
		return essenceNames[code-1];
	}
	
	public static string GetItemName (int code)
	{
		return essenceNames[code-1];
	}
}
