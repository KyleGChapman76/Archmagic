using UnityEngine;
using System.Collections;

public class SpellInput : MonoBehaviour
{
	public SpellHandler spellHandler;

	private void Start()
	{
		spellHandler = GetComponent<SpellHandler>();
	}

	private void Update ()
	{
		Keyboard();
		Mouse();
	}
	
	private void Keyboard ()
	{
		for (int i = 1;i<=4;i++)
		{
			if (Input.GetAxis("WeaponSwitch" + i) > 0)
			{
				spellHandler.ReportSwitch(i-1);
			}
		}
	}
	
	private void Mouse ()
	{
		if (Input.GetAxis("Fire1") == 1)
		{
			spellHandler.ReportActivate(); //activate the currently selected spell
		}

		float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
		if (scrollWheel > 0)
		{
			spellHandler.ReportSwitchForwards();
		}
		else if (scrollWheel < 0)
		{
			spellHandler.ReportSwitchBackwards();
		}
	}
	
}
