using UnityEngine;
using System.Collections;

public class ScrollInventory : MonoBehaviour
{
	public const int inventorySize = 4;

	private Scroll[] scrolls;
	
	private Scroll selectedScroll;
	private int selectedIndex;
	
	private void Start ()
	{
		scrolls = new Scroll[inventorySize];
		
		if (SpellBook.FindBooks().Length == 0)
		{
			print("No spellbook exists in the current map!");
			return;
		}
		SetDefaultSpells(inventorySize);
		selectedScroll = scrolls[0];
	}

	public void SetDefaultSpells (int number)
	{
		for (int i = 0; i < number; i++)
		{
			Spell spell = SpellBook.FindSpell(new int[] { i + 1 });
			AddScroll(new Scroll(spell, gameObject, new int[spell.GetNumCustomizedValues()]));
		}
	}
	
	public bool AddScroll ( Scroll adding  )
	{
		for (int i = 0;i<scrolls.Length;i++)
		{
			if (scrolls[i] == null)
			{
				scrolls[i] = adding;
				return true;
			}
		}
		return false;
	}
	
	public Scroll GetSelectedScroll ()
	{
		return selectedScroll;
	}
	
	public bool SetSelectedScroll (int index)
	{
		selectedIndex = index;
		selectedScroll =  GetScrollAtIndex(index);
		print(index);
		return (selectedScroll != null);
	}

	public bool SetSelectedForwards()
	{
		int newIndex = selectedIndex;
		int count = 0;
		do
		{
			newIndex++;
			count++;
			if (newIndex >= inventorySize)
				newIndex -= 8;
        } while (!SetSelectedScroll(newIndex) && count < inventorySize);

		return true;
	}

	public bool SetSelectedBackwards()
	{
		int newIndex = selectedIndex;
		int count = 0;
		do
		{
			newIndex--;
			count++;
			if (newIndex < 0)
				newIndex = inventorySize-1;
		} while (!SetSelectedScroll(newIndex) && count < inventorySize);

		return true;
	}

	public int GetSelectedIndex()
	{
		return selectedIndex;
	}

	public bool ReportActivate (Vector3 targetingData)
	{
		if (selectedScroll == null)
		{
			print("The selected scroll is not valid.");
			return false;
		}
		SpellTargetingType type = selectedScroll.GetTargetType();
		if (!type.Equals(SpellTargetingType.Projectile))
		{
			print("Giving the wrong data for this spell!");
			return false;
		}
		selectedScroll.Activate(targetingData);
		return true;
	}
	
	public bool ReportActivate (RaycastHit targetingData)
	{
		if (selectedScroll == null)
		{
			print("The selected scroll is not valid.");
			return false;
		}
		SpellTargetingType type = selectedScroll.GetTargetType();
		if (!type.Equals(SpellTargetingType.Area))
		{
			print("Giving the wrong data for this spell!");
			return false;
		}
		selectedScroll.Activate(targetingData);
		return true;
	}
	
	public bool ReportActivate (GameObject targetingData)
	{
		if (selectedScroll == null)
		{
			print("The selected scroll is not valid.");
			return false;
		}
		SpellTargetingType type = selectedScroll.GetTargetType();
		if (!(type.Equals(SpellTargetingType.Null) || type.Equals(SpellTargetingType.Unit)))
		{
			print("Giving the wrong data for this spell!");
			return false;
		}
		selectedScroll.Activate(targetingData);
		return true;
	}
	
	public Scroll GetScrollAtIndex (int index)
	{
		if (index < 0 || index > scrolls.Length -1)
			return null;
		return scrolls[index];
	}
	
	public void SetScrollAtIndex (int index, Scroll newScroll)
	{
		if (index < 0 || index > scrolls.Length -1)
			return;
		scrolls[index] = newScroll;
	}
	
	public string GetSelectedScrollName ()
	{
		if (selectedScroll != null)
			return selectedScroll.GetName();
		else
			return "No Scroll Held";
	}
	
	public string[] GetScrollNames ()
	{
		string[] names = new string[scrolls.Length];
		for (int i = 0;i<scrolls.Length;i++)
		{
			if (scrolls[i] == null)
			{
				names[i] = "";
				continue;
			}
			names[i] = scrolls[i].GetName();
		}
		return names;
	}
}