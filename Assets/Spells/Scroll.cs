// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections.Generic;

public class Scroll
{
	public const int MAX_POINTS = 3;
	public const int CUSTOM_VALUES = 3;

	private Spell spell;
	
	private GameObject owner;
	
	private int[] pointsAllotted;
	
	public Scroll (Spell spell, GameObject owner, int[] allocatedPoints)
	{
		this.spell = spell;
		this.owner = owner;
		this.pointsAllotted = allocatedPoints;
		int numCustoms = GetNumCustomizedValues();
		
		if (pointsAllotted.Length != numCustoms)
		{
			Debug.LogError("Given allocated points are incorrect size! Needs to be size " + numCustoms);
		}
	}
	
	public bool Activate (Vector3 targetingData)
	{
		if (spell == null)
		{
			Debug.Log("The selected scroll does not have a spell to activate!");
			return false;
		}
		SpellTargetingType type = spell.GetTargetType();
		if (!type.Equals(SpellTargetingType.Projectile))
		{
			Debug.Log("Giving the wrong data for this spell!");
			return false;
		}
		spell.Activate(owner, pointsAllotted, targetingData);
		return true;
	}
	
	public bool Activate (RaycastHit targetingData)
	{
		if (spell == null)
		{
			Debug.Log("The selected scroll does not have a spell to activate!");
			return false;
		}
		SpellTargetingType type = spell.GetTargetType();
		if (!type.Equals(SpellTargetingType.Area))
		{
			Debug.Log("Giving the wrong data for this spell!");
			return false;
		}
		spell.Activate(owner, pointsAllotted, targetingData);
		return true;
	}
	
	public bool Activate (GameObject targetingData)
	{
		if (spell == null)
		{
			Debug.Log("The selected scroll does not have a spell to activate!");
			return false;
		}
		SpellTargetingType type = spell.GetTargetType();
		if (!(type.Equals(SpellTargetingType.Null) || type.Equals(SpellTargetingType.Unit)))
		{
			Debug.Log("Giving the wrong data for this spell!");
			return false;
		}
		spell.Activate(owner, pointsAllotted, targetingData);
		return true;
	}

	public Spell GetSpell ()
	{
		return spell;
	}
	
	public string GetName ()
	{
		if (spell == null)
			return "Empty";
		string name = spell.GetName();
		if (pointsAllotted != null && pointsAllotted.Length > 0)
		{
			name = name + "(";
			for (int i = 0;i<pointsAllotted.Length;i++)
			{
				name = name + pointsAllotted[i].ToString();
				if (i != pointsAllotted.Length-1)
					name = name + ", ";
			}
			name = name + ")";
		}
		return name;
	}
	
	public int[] GetSignature ()
	{
		if (spell == null)
		{
			return new int[0] {};
		}
		return spell.GetSignature();
	}
	
	public Spell ChangeSpell (int[] signature)
	{
		Spell newSpell = SpellBook.FindSpell(signature);

		if (newSpell == null)
		{
			Debug.Log("Scroll tried to change to a null spell!");
		}
		else
		{
			Debug.Log("Scroll changed to spell " + newSpell.GetName());
			spell = newSpell;
		}
		
		return newSpell;
	}
	
	public void ChangeSpell (Spell spell)
	{
		this.spell = spell;
	}
	
	public void SetAllocatedPoints (int[] points)
	{
		int numCustoms = GetNumCustomizedValues();
		
		if (points.Length != numCustoms)
		{
			Debug.Log("Given allocated points are incorrect size! Needs to be size " + numCustoms + ", but they are of size " + points.Length);
		}
		else
			pointsAllotted = points;
	}
	
	public int[] GetAllocatedPoints ()
	{
		return pointsAllotted;
	}
	
	public CustomizedValue GetCustomizedOfIndex (int index)
	{
		if (spell == null)
			return null;
		return spell.GetCustomizedOfIndex(index);
	}
	
	public CustomizedValue[] GetCustomizedValues ()
	{
		if (spell == null)
			return null;
		return spell.GetCustomizedValues();
	}
	
	public int GetNumCustomizedValues ()
	{
		if (spell == null)
			return 0;
		return spell.GetNumCustomizedValues();
	}
	
	public SpellTargetingType GetTargetType ()
	{
		if (spell == null)
			return SpellTargetingType.Null;
		return spell.GetTargetType();
	}
	
	//-1 is failed, don't continue
	//0 is failed, but continue
	//1 is success
	public int IsValidTarget(Transform playerTransform, RaycastHit hit)
	{
		if (spell == null)
			return -1;
		return spell.IsValidTarget(playerTransform, hit, pointsAllotted);
	}
	
	public float GetRange()
	{
		if (spell == null)
			return 0f;
		return spell.GetRange(pointsAllotted);
	}
	
	public float GetIndicatorRadius ()
	{
		if (spell == null)
			return 0f;
		return spell.GetIndicatorRadius(pointsAllotted);
	}
	
	public Scroll Clone ()
	{
		return new Scroll(spell, owner, pointsAllotted);
	}
}