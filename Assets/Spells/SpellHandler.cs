using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class SpellHandler : MonoBehaviour {

	public ItemInventory itemInventory;
	
	public ScrollInventory scrollInventory;
	
	private int frozenTime;
	
	private bool validTarget;
	
	//for projectile targeting
	private Vector3 targetingDirection;
	
	//for area targeting
	private RaycastHit targetingPoint;
	private float targetingRadius;
	private GameObject aoeIndicator;
	
	//for unit targeting
	private GameObject targetingUnit;

	public GameObject spellPanel;
	private GameObject[] spellSlots;
	
	private void Start ()
	{
		GameObject indicatorPrefab = (GameObject)Resources.Load("AOEIndicator");
		aoeIndicator = (GameObject)Instantiate(indicatorPrefab);
		aoeIndicator.GetComponent<Renderer>().enabled = false;
		frozenTime = 0;

		spellSlots = new GameObject[ScrollInventory.inventorySize];
        int count = 0;
		foreach (Transform child in spellPanel.transform)
		{
			spellSlots[count] = child.gameObject;
			count++;
		}
    }
	
	private void Update ()
	{
		//show targeting UI and also get data for targeting spells
		if (scrollInventory.GetSelectedScroll() != null)
		{
			targetingDirection = Vector3.zero;
			
			targetingPoint = new RaycastHit();
			aoeIndicator.GetComponent<Renderer>().enabled = false;
			
			if (targetingUnit != null)
				targetingUnit.GetComponent<Renderer>().material.SetColor("_OutlineColor", Color.black);
			
			targetingUnit = null;
			
			validTarget = false;
			
			SphereConversion sphereConverter = (SphereConversion)GetComponent<SphereConversion>();
			Vector3 forward = sphereConverter.Convert(1f);
			
			SpellTargetingType type = scrollInventory.GetSelectedScroll().GetTargetType();
			if (type.Equals(SpellTargetingType.Projectile))
			{
				targetingDirection = forward;
				validTarget = true;
			}
			else if (type.Equals(SpellTargetingType.Area))
			{
				RaycastHit[] hits = Physics.RaycastAll(transform.GetChild(0).transform.position,forward,scrollInventory.GetSelectedScroll().GetRange()).OrderBy(h=>h.distance).ToArray();
				for (int i = 0;i<hits.Length;i++)
				{
					RaycastHit hit = hits[i];
					int result = scrollInventory.GetSelectedScroll().IsValidTarget(transform,hit);
					if (result == 1)
					{
						targetingPoint = hit;
						validTarget = true;
						aoeIndicator.GetComponent<Renderer>().enabled = true;
						
						//set the indicaotrs scale, rotation, and position
						aoeIndicator.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z) + hit.normal*.1f;
						float radius = scrollInventory.GetSelectedScroll().GetIndicatorRadius();
						aoeIndicator.transform.localScale = new Vector3 (radius*2, aoeIndicator.transform.localScale.y, radius*2);
						aoeIndicator.transform.rotation = Quaternion.LookRotation(hit.normal);
						Vector3 eulers = aoeIndicator.transform.rotation.eulerAngles;
						aoeIndicator.transform.rotation = Quaternion.Euler(new Vector3(eulers.x+90, eulers.y, eulers.z));
						
						break;
					}
					else if (result == -1)
						break;
				}
			}
			else if (type.Equals(SpellTargetingType.Unit))
			{
				RaycastHit[] hits = Physics.RaycastAll(transform.position+Vector3.up,forward, scrollInventory.GetSelectedScroll().GetRange()).OrderBy(h=>h.distance).OrderBy(h=>h.distance).ToArray();
				for (int i = 0;i<hits.Length;i++)
				{
					RaycastHit hit = hits[i];
					int result = scrollInventory.GetSelectedScroll().IsValidTarget(transform,hit);
					if (result == 1)
					{
						targetingUnit = hit.collider.gameObject;
						validTarget = true;
						targetingUnit.GetComponent<Renderer>().material.SetColor("_OutlineColor", Color.green);
						break;
					}
					if (result == -1)
						break;
				}
			}
		}

		//set the color and text options of the spell bar
		int selectedIndex = scrollInventory.GetSelectedIndex();
		for (int i = 0;i < ScrollInventory.inventorySize;i++)
		{
			GameObject spellSlot = spellSlots[i];
            Image spellSlotImage = spellSlot.GetComponent<Image>();
			Text spellSlotText = spellSlot.GetComponentInChildren<Text>();

			Scroll scroll = scrollInventory.GetScrollAtIndex(i);

			if (scroll != null)
			{
				spellSlotText.text = scrollInventory.GetScrollAtIndex(i).GetSpell().GetName();
				spellSlotText.color = Color.black;
				spellSlotText.fontSize = 18;
				spellSlotImage.color = new Color(135f / 255f, 242f / 255f, 139f / 252f);
			}
			else
			{
				spellSlotText.text = "Empty";
				spellSlotText.color = Color.grey;
				spellSlotText.fontSize = 16;
				spellSlotImage.color = Color.grey;
			}

			if (i == selectedIndex)
			{
				spellSlotImage.fillCenter = true;
            }
			else
			{
				spellSlotImage.fillCenter = false;
			}
		}
	}
	
	private void FixedUpdate ()
	{
		if (frozenTime > 0)
			frozenTime--;
	}
	
	public void GetSpellName ()
	{
		scrollInventory.GetSelectedScrollName();
	}
	
	public ItemInventory GetInventory ()
	{
		return itemInventory;
	}
	
	public bool ReportSwitch (int index)
	{
		if (frozenTime > 0)
		{
			return false;
		}
		bool flag = scrollInventory.SetSelectedScroll(index);
		if (flag)
		{
			ReportFrozenTime(20);
		}
		return flag;
	}	
	
	public bool ReportSwitchForwards ()
	{
		return scrollInventory.SetSelectedForwards();
    }

	public bool ReportSwitchBackwards ()
	{
		return scrollInventory.SetSelectedBackwards();
	}

	public bool ReportActivate ()
	{
		if (frozenTime > 0)
		{
			return false;
		}
		if (!validTarget)
		{
			print("No valid target for current spell!");
			return false;
		}
		if (scrollInventory != null)
		{
			bool flag = false;
			
			SpellTargetingType type = scrollInventory.GetSelectedScroll().GetTargetType();
			switch(type)
			{
				case SpellTargetingType.Projectile:
					flag = scrollInventory.ReportActivate(targetingDirection);
					break;
				case SpellTargetingType.Area:
					flag = scrollInventory.ReportActivate(targetingPoint);
					break;
				case SpellTargetingType.Unit:
					flag = scrollInventory.ReportActivate(targetingUnit);
					break;
			}

			if (flag)
			{
				print("Activated spell!");
				foreach (int code in scrollInventory.GetSelectedScroll().GetSignature())
				{
					string itemName = EssenceInfo.GetItemName(code);
					itemInventory.RemoveItem(itemName);
				}
			}
			return flag;
		}
		return false;
	}
	
	public void ReportFrozenTime (int time)
	{
		frozenTime = time;
	}
}