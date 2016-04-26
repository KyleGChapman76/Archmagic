using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(MagicElement))]
public class Spell : MonoBehaviour
{
	public GameObject blueprint;

	[SerializeField]
	private int[] signature;
	public int manaCost;
	
	private SpellTargetingType targetType;
	public bool continuouslyActivated;
	
	//initialize the spell by having it determine which targeting type its blueprint uses
	private void Start ()
	{
		ProjectileTargeting projectile = blueprint.GetComponent<ProjectileTargeting>();
		AreaTargeting area = blueprint.GetComponent<AreaTargeting>();
		UnitTargeting unit = blueprint.GetComponent<UnitTargeting>();

		if (projectile != null)
			targetType = SpellTargetingType.Projectile;
		else if (area != null)
			targetType = SpellTargetingType.Area;
		else if (unit != null)
			targetType = SpellTargetingType.Unit;
		else
		{
			targetType = SpellTargetingType.Null;
			print("A spell could not find its targeting type!");
		}
	}
	
	//activate while targeting a direction
	public GameObject Activate (GameObject owner, int[] points, Vector3 targetingData)
	{
		if (owner == null || points == null)
		{
			print("Given activation info is null!");
			return null;
		}
		
		//create the actual spell behavior
		GameObject obj = Instantiate(blueprint) as GameObject;
		obj.transform.parent = owner.transform;		

		//set the customized values
		int customs = GetNumCustomizedValues();
		if (customs != points.Length)
		{
			print("Given allocated points are incorrect size when trying to activate!");
			return null;
		}
		foreach (CustomizedValue v in obj.GetComponents<CustomizedValue>())
		{
			v.SetPoints(points[v.index]);
		}
		
		//give the targeting data
		obj.GetComponent<ProjectileTargeting>().SetDirection(targetingData);
		
		return obj;
	}
	
	//activate while targeting a point on the ground
	public GameObject Activate (GameObject owner, int[] points, RaycastHit targetingData)
	{
		if (owner == null || points == null)
		{
			print("Given activation info is null!");
			return null;
		}
		
		//create the actual spell behavior
		GameObject obj = (GameObject)Instantiate(blueprint);
		obj.transform.parent = owner.transform;		
		
		//set the customized values
		int customs = GetNumCustomizedValues();
		if (customs != points.Length)
		{
			print("Given allocated points are incorrect size when trying to activate!");
			return null;
		}
		foreach (CustomizedValue v in obj.GetComponents<CustomizedValue>())
		{
			v.SetPoints(points[v.index]);
		}
		
		//give the targeting data
		obj.GetComponent<AreaTargeting>().SetHit(targetingData);
		
		return obj;
	}
	
	//activate while targeting a unit
	public GameObject Activate (GameObject owner, int[] points, GameObject targetingData)
	{
		if (owner == null || points == null || targetingData == null)
		{
			print("Given activation info is null!");
			return null;
		}
		
		//create the actual spell behavior
		GameObject obj = (GameObject)Instantiate(blueprint);
		obj.transform.parent = owner.transform;		
		
		//set the customized values
		int customs = GetNumCustomizedValues();
		if (customs != points.Length)
		{
			print("Given allocated points are incorrect size when trying to activate!");
			return null;
		}
		foreach (CustomizedValue v in obj.GetComponents<CustomizedValue>())
		{
			v.SetPoints(points[v.index]);
		}
		
		//give the targeting data
		obj.GetComponent<UnitTargeting>().SetUnit(targetingData);
		
		return obj;
	}
	
	public string GetName ()
	{
		return blueprint.name;
	}
	public int GetManaCost()
	{
		return manaCost;
	}
	
	public CustomizedValue[] GetCustomizedValues ()
	{
		return blueprint.GetComponents<CustomizedValue>();
	}
	
	public int GetNumCustomizedValues ()
	{
		return blueprint.GetComponents<CustomizedValue>().Length;
	}
	
	public CustomizedValue GetCustomizedOfIndex (int index)
	{
		foreach (CustomizedValue v in GetCustomizedValues())
		{
			if (v.index == index)
				return v;
		}
		return null;
	}
	
	public SpellTargetingType GetTargetType ()
	{
		return targetType;
	}

	public int IsValidTarget(Transform playerTransform, RaycastHit hit)
	{
		if (targetType.Equals(SpellTargetingType.Area))
		{
			AreaTargeting area = (AreaTargeting)blueprint.GetComponent<AreaTargeting>();
			float distance = Vector3.Distance(playerTransform.position, hit.point);
			float range = area.GetBaseRange();

			bool inRange = distance <= range;
			if (!inRange)
			{
				//print("Can't target point since " + distance + " is further away than " + range);
				return -1;
			}

			bool inNormalVariance = area.IsValidNormal(hit.normal);
			if (!inNormalVariance)
			{
				//print("Can't target ponint since " + hit.normal " is not a valid normal for the spell.");
				return 0;
			}

			bool validTag = area.IsValidTag(hit.collider.tag);
			if (!validTag)
			{
				//print("Can't target point since " + hit.collider.tag + " is not a valid tag.");
				return 0;
			}

			bool validName = area.IsValidName(hit.collider.name);
			if (!validName)
			{
				//print("Can't target point since " + hit.collider.name + " is not a valid name.");	
				return 0;
			}

			return 1;
		}
		else if (targetType.Equals(SpellTargetingType.Unit))
		{
			UnitTargeting unit = (UnitTargeting)blueprint.GetComponent<UnitTargeting>();
			float distance = Vector3.Distance(playerTransform.position, hit.point);
			float range = unit.GetBaseRange();

			bool inRange = distance <= range;
			if (!inRange)
			{
				//print("Can't target unit since " + distance + " is further away than " + range);
				return -1;
			}

			bool validTag = unit.IsValidTag(hit.collider.tag);
			if (!validTag)
			{
				//print("Can't target unit since " + hit.collider.tag + " is not a valid tag.");
				return 0;
			}

			bool validName = unit.IsValidName(hit.collider.name);
			if (!validName)
			{
				//print("Can't target unit since " + hit.collider.name + " is not a valid name.");
				return 0;
			}

			bool validComponents = unit.HasValidComponents(hit.collider.gameObject);
			if (!validComponents)
			{
				//print("Can't target " + hit.collider.name +  " since it does not have valid components.");
				return 0;
			}

			return 1;
		}

		print("Can't test for valid target for this type of spell.");
		return -1;
	}

	public int IsValidTarget (Transform playerTransform, RaycastHit hit, int[] pointsAllotted)
	{
		if (targetType.Equals(SpellTargetingType.Area))
		{
			AreaTargeting area = (AreaTargeting)blueprint.GetComponent<AreaTargeting>();
			float distance = Vector3.Distance(playerTransform.position,hit.point);
			float range = area.GetRange(pointsAllotted);
			
			bool inRange = distance <= range;
			if (!inRange)
			{
				//print("Can't target point since " + distance + " is further away than " + range);
				return -1;
			}
			
			bool inNormalVariance = area.IsValidNormal(hit.normal);
			if (!inNormalVariance)
			{
				//print("Can't target ponint since " + hit.normal " is not a valid normal for the spell.");
				return 0;
			}
			
			bool validTag = area.IsValidTag(hit.collider.tag);
			if (!validTag)
			{
				//print("Can't target point since " + hit.collider.tag + " is not a valid tag.");
				return 0;
			}
			
			bool validName = area.IsValidName(hit.collider.name);
			if (!validName)
			{
				//print("Can't target point since " + hit.collider.name + " is not a valid name.");	
				return 0;
			}
			
			return 1;
		}
		else if (targetType.Equals(SpellTargetingType.Unit))
		{
			UnitTargeting unit = (UnitTargeting)blueprint.GetComponent<UnitTargeting>();
			float distance = Vector3.Distance(playerTransform.position,hit.point);
			float range = unit.GetRange(pointsAllotted);
			
			bool inRange = distance <= range;
			if (!inRange)
			{
				//print("Can't target unit since " + distance + " is further away than " + range);
				return -1;
			}
			
			bool validTag = unit.IsValidTag(hit.collider.tag);
			if (!validTag)
			{
				//print("Can't target unit since " + hit.collider.tag + " is not a valid tag.");
				return 0;
			}
			
			bool validName = unit.IsValidName(hit.collider.name);
			if (!validName)
			{
				//print("Can't target unit since " + hit.collider.name + " is not a valid name.");
				return 0;
			}
			
			bool validComponents = unit.HasValidComponents(hit.collider.gameObject);
			if (!validComponents)
			{
				//print("Can't target " + hit.collider.name +  " since it does not have valid components.");
				return 0;
			}

			return 1;
		}
		
		print("Can't test for valid target for this type of spell.");
		return -1;
	}

	public float GetBaseRange()
	{
		if (targetType.Equals(SpellTargetingType.Area))
		{
			AreaTargeting area = blueprint.GetComponent<AreaTargeting>();
			return area.GetBaseRange();
		}
		else if (targetType.Equals(SpellTargetingType.Unit))
		{
			UnitTargeting unit = blueprint.GetComponent<UnitTargeting>();
			return unit.GetBaseRange();
		}

		return -1f;
	}

	public float GetRange (int[] pointsAllotted)
	{
		if (targetType.Equals(SpellTargetingType.Area))
		{
			AreaTargeting area = blueprint.GetComponent<AreaTargeting>();
			return area.GetRange(pointsAllotted);
		}
		else if (targetType.Equals(SpellTargetingType.Unit))
		{
			UnitTargeting unit = blueprint.GetComponent<UnitTargeting>();
			return unit.GetRange(pointsAllotted);
		}
		
		return -1f;
	}

	public float GetIndicatorRadius()
	{
		if (targetType.Equals(SpellTargetingType.Area))
		{
			AreaTargeting area = blueprint.GetComponent<AreaTargeting>();
			return area.GetBaseRadius();
		}

		return -1f;
	}

	public float GetIndicatorRadius (int[] pointsAllotted)
	{
		if (targetType.Equals(SpellTargetingType.Area))
		{
			AreaTargeting area = blueprint.GetComponent<AreaTargeting>();
			return area.GetRadius(pointsAllotted);
		}
		
		return -1f;
	}

	public int[] GetSignature()
	{
		return signature;
	}
}