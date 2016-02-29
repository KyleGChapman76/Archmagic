using UnityEngine;
using System.Collections;

public class Signature : MonoBehaviour
{
    public static bool Matches (int[] codes1, int[] codes2)
    {
        return isEqArrays(codes1, codes2);
    }
    
    private static bool inArray (int[] array, int el)
    {
        for (int i = 0;i<array.Length;i++)
        {
            if (array[i] == el)
                return true;
        }
        return false;
    }
    
    private static bool isEqArrays (int[] arr1, int[] arr2)
    {
    	if (arr1 == null)
    		return arr2 == null;
        if (arr1.Length != arr2.Length)
        {
            return false;
        }
        for (int i = 0; i < arr1.Length; i++)
        {
            if (!inArray(arr2,arr1[i]))
                return false;
        }
        return true;
    }
    
    public static int[] Trim (int[] array)
    {
    	int size = 0;
    	for (int i = 0;i<array.Length;i++)
    		if (array[i] != 0)
    			size++;
    	int[] trimmed = new int[size];
    	
    	int count = 0;
		for (int i = 0;i<array.Length;i++)
		{
			if (array[i] != 0)
			{
				trimmed[count] = array[i];
				count++;
			}
		}
        return trimmed;
    }
    
    public static void PrintSignature (int[] array)
    {
    	for (int i = 0;i<array.Length;i++)
    		print(array[i].ToString());
    }
}