using UnityEngine;
using System.Collections;

public class SpellBook : MonoBehaviour
{
	public static SpellBook[] FindBooks ()
	{
		return GameObject.FindObjectsOfType<SpellBook>();
	}
	
	public static Spell FindSpell (int[] signature)
	{
		foreach (SpellBook book in FindBooks())
		{
			Spell spell = book.FindSpellInBook(signature);
			if (spell != null)
				return spell;
		}
		return null;
	}
	
	public Spell FindSpellInBook (int[] signature)
	{
		foreach (Spell spell in GetComponents<Spell>())
		{
			if (Signature.Matches(signature, spell.signature))
				return spell;
		}
		return null;
	}
}
