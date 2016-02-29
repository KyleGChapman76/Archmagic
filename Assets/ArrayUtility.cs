using UnityEngine;
using System.Collections;

public class ArrayUtility : MonoBehaviour
{
	public static bool Contains (string[] strings, string str)
	{
		foreach (string s in strings)
			if (s.Equals(str)) return true;
		return false;
	}
}
