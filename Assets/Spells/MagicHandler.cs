using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class MagicHandler : MonoBehaviour
{
	public Animator playerArm1Animator;
	public Animator playerArm2Animator;

	//references to the spells that could currently be cast with the left hand, right hand, and both
	private Spell spellToCast1;
	private Spell spellToCast2;
	private Spell spellToCastCombo;

	GameObject aoeIndicator;

	public GameObject spellPanel;
	private GameObject[] spellSlots;

	public bool fireEnabled;
	public bool airEnabled;
	public bool waterEnabled;
	public bool earthEnabled;

	private bool[] elements;

	private int selectedElement1;
	private int selectedElement2;
	public Color selectedColor = new Color(135f / 255f, 242f / 255f, 139f / 252f);
	public Color doubleSelectedColor = new Color(105f / 255f, 212f / 255f, 109f / 252f);

	private bool spell1BeingCast;
	private bool spell2BeingCast;

	private GameObject spell1Instantiation;
	private GameObject spell2Instantiation;
	private GameObject spellComboInstantiation;

	public Mana manaHandler;
	private float manaTimer;
	private float timeBetweenMana;

	private void Start()
	{
		spellSlots = new GameObject[ScrollInventory.inventorySize];
		int count = 0;
		foreach (Transform child in spellPanel.transform)
		{
			spellSlots[count] = child.gameObject;
			count++;
		}

		selectedElement1 = 1;
		selectedElement2 = 2;

		GameObject indicatorPrefab = (GameObject)Resources.Load("AOEIndicator");
		aoeIndicator = (GameObject)Instantiate(indicatorPrefab);
		aoeIndicator.GetComponent<Renderer>().enabled = false;

		manaTimer = 0;
		timeBetweenMana = 5;
	}

	private void Update()
	{
		elements = new bool[] { fireEnabled, airEnabled, waterEnabled, earthEnabled };

		FindCurrentSpells();

		MagicInput();
		
		//activate combo spell
		if (spell1BeingCast && spell2BeingCast && !spellComboInstantiation && spellToCastCombo)
		{
			Destroy(spell1Instantiation);
			Destroy(spell2Instantiation);
			bool upgraded = selectedElement1 == selectedElement2;
			spellComboInstantiation = TargetAndCast(spellToCastCombo, upgraded);
			if (spellToCastCombo.continuouslyActivated)
				timeBetweenMana = 1f / spellToCastCombo.manaCost;
        }
		//activate spell 1
		if (spell1BeingCast && !spell2BeingCast && !spell1Instantiation && spellToCast1)
		{
			Destroy(spell2Instantiation);
			Destroy(spellComboInstantiation);
			spell1Instantiation = TargetAndCast(spellToCast1, false);
			if (spellToCast1.continuouslyActivated)
				timeBetweenMana = 1f / spellToCast1.manaCost;
		}
		//activate spell 2
		if (!spell1BeingCast && spell2BeingCast && !spell2Instantiation && spellToCast2)
		{
			Destroy(spell1Instantiation);
			Destroy(spellComboInstantiation);
			spell2Instantiation = TargetAndCast(spellToCast1, false);
			if (spellToCast2.continuouslyActivated)
				timeBetweenMana = 1f / spellToCast2.manaCost;
		}

		//deactivate spells
		if ((!spell1BeingCast || !spell2BeingCast) && spellComboInstantiation)
		{
			print("Deactivating combo spell");
			Destroy(spellComboInstantiation);
        }
		else if (!spell1BeingCast && spell1Instantiation)
		{
			print("Deactivating spell 1");
			Destroy(spell1Instantiation);
		}
		else if (!spell2BeingCast && spell2Instantiation)
		{
			print("Deactivating spell 2");
			Destroy(spell2Instantiation);
		}

		if (spell1Instantiation || spell2Instantiation || spellComboInstantiation)
			manaTimer += Time.deltaTime;
		else
			manaTimer = 0;

		if (manaTimer >= timeBetweenMana)
		{
			manaTimer = 0;
			manaHandler.SpendMana(1);
		}

		SpellUI();
	}

	//get mouse and keyboard input about magic
	private void MagicInput()
	{
		for (int i = 1; i <= 4; i++)
		{
			if (Input.GetButtonDown("WeaponSwitch" + i))
			{
				int elementSelected = i;
				if (elements[elementSelected-1])
				{
					if (selectedElement1 == elementSelected)
						selectedElement2 = elementSelected;
					else
						selectedElement1 = elementSelected;
				}
			}
		}

		//right arm animation
		if (Input.GetAxis("Fire2") == 1)
		{
			playerArm1Animator.SetBool("ArmInput", true);
			if (playerArm1Animator.GetCurrentAnimatorStateInfo(0).IsName("CastingAnimation"))
			{
				if (playerArm1Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !playerArm1Animator.IsInTransition(0))
				{
					spell1BeingCast = true;
				}
			}
		}
		else
		{
			spell1BeingCast = false;
            if (playerArm1Animator.GetBool("ArmInput"))
			{
				playerArm1Animator.SetBool("ArmInput", false);
			}
		}

		//left arm animation
		if (Input.GetAxis("Fire1") == 1)
		{
			playerArm2Animator.SetBool("ArmInput", true);
			if (playerArm2Animator.GetCurrentAnimatorStateInfo(0).IsName("CastingAnimation"))
			{
				if (playerArm2Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !playerArm2Animator.IsInTransition(0))
				{
					spell2BeingCast = true;
				}
			}
		}
		else
		{
			spell2BeingCast = false;
			if (playerArm2Animator.GetBool("ArmInput"))
			{
				playerArm2Animator.SetBool("ArmInput", false);
			}
		}

		float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
		if (scrollWheel > 0)
		{
			int slotSelected = selectedElement1 + 1;
			if (slotSelected >= 4)
				slotSelected -= 4;

			if (elements[slotSelected])
			{
				selectedElement1 = slotSelected;
			}
			
        }
		else if (scrollWheel < 0)
		{
			int slotSelected = selectedElement1 - 1;
			if (slotSelected >= 4)
				slotSelected -= 4;

			if (elements[slotSelected])
			{
				selectedElement1 = slotSelected;
			}
		}
	}

	private void FindCurrentSpells()
	{
		if (!spell1BeingCast)
			spellToCast1 = SpellBook.FindSpell(new int[] { selectedElement1});
		if (!spell2BeingCast)
			spellToCast2 = SpellBook.FindSpell(new int[] { selectedElement2});
		if (!spell1BeingCast || !spell2BeingCast)
		{
			if (selectedElement1 == selectedElement2)
				spellToCastCombo = SpellBook.FindSpell(new int[] { selectedElement1 });
			else
				spellToCastCombo = SpellBook.FindSpell(new int[] { selectedElement1, selectedElement2 });
		}
	}

	//show targeting UI and also get data for targeting spells
	private GameObject TargetAndCast(Spell spellForTargeting, bool upgraded)
	{
		//for projectile targeting
		Vector3 targetingDirection;

		//for area targeting
		float targetingRadius;

		targetingDirection = Vector3.zero;

		RaycastHit targetingPoint = new RaycastHit();
		aoeIndicator.GetComponent<Renderer>().enabled = false;

		GameObject targetingUnit = null;

		bool validTarget = false;

		SphereConversion sphereConverter = (SphereConversion)GetComponent<SphereConversion>();
		Vector3 forward = sphereConverter.Convert(1f);

		SpellTargetingType type = spellForTargeting.GetTargetType();
		if (type.Equals(SpellTargetingType.Projectile))
		{
			targetingDirection = forward;
			validTarget = true;
		}
		else if (type.Equals(SpellTargetingType.Area))
		{
			RaycastHit[] hits = Physics.RaycastAll(transform.GetChild(0).transform.position, forward, spellForTargeting.GetBaseRange()).OrderBy(h => h.distance).ToArray();
			for (int i = 0; i < hits.Length; i++)
			{
				RaycastHit hit = hits[i];
				int result = spellForTargeting.IsValidTarget(transform, hit);
				if (result == 1)
				{
					targetingPoint = hit;
					validTarget = true;
					aoeIndicator.GetComponent<Renderer>().enabled = true;

					//set the indicaotrs scale, rotation, and position
					aoeIndicator.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z) + hit.normal * .1f;
					float radius = spellForTargeting.GetIndicatorRadius();
					aoeIndicator.transform.localScale = new Vector3(radius * 2, aoeIndicator.transform.localScale.y, radius * 2);
					aoeIndicator.transform.rotation = Quaternion.LookRotation(hit.normal);
					Vector3 eulers = aoeIndicator.transform.rotation.eulerAngles;
					aoeIndicator.transform.rotation = Quaternion.Euler(new Vector3(eulers.x + 90, eulers.y, eulers.z));

					break;
				}
				else if (result == -1)
					break;
			}
		}
		else if (type.Equals(SpellTargetingType.Unit))
		{
			RaycastHit[] hits = Physics.RaycastAll(transform.position + Vector3.up, forward, spellForTargeting.GetBaseRange()).OrderBy(h => h.distance).OrderBy(h => h.distance).ToArray();
			for (int i = 0; i < hits.Length; i++)
			{
				RaycastHit hit = hits[i];
				int result = spellForTargeting.IsValidTarget(transform, hit);
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

		if (targetingUnit != null)
			targetingUnit.GetComponent<Renderer>().material.SetColor("_OutlineColor", Color.black);

		if (validTarget)
		{
			return ActivateSpell(spellForTargeting, targetingDirection, targetingPoint.point, targetingUnit, upgraded);
		}

		return null;
	}

	public void DeactivateSpells()
	{
		if (spell1Instantiation)
			Destroy(spell1Instantiation);
		if (spell2Instantiation)
			Destroy(spell2Instantiation);
		if (spellComboInstantiation)
			Destroy(spellComboInstantiation);
	}

	private void SpellUI()
	{
		//set the color and text options of the spell bar
		for (int i = 0; i < elements.Length; i++)
		{
			GameObject spellSlot = spellSlots[i];
			Image spellSlotImage = spellSlot.GetComponent<Image>();
			Text spellSlotText = spellSlot.GetComponentInChildren<Text>();

			bool selected1 = i == selectedElement1-1;
			bool selected2 = i == selectedElement2-1;

			if (elements[i])
			{
				spellSlotText.color = Color.black;
				spellSlotText.fontSize = 18;
				if (selected1 && selected2)
					spellSlotImage.color = doubleSelectedColor;
				else
					spellSlotImage.color = selectedColor;
			}
			else
			{
				spellSlotText.text = "";
				spellSlotText.color = Color.grey;
				spellSlotText.fontSize = 16;
				spellSlotImage.color = Color.grey;
			}

			if (selected1 || selected2)
			{
				spellSlotImage.fillCenter = true;
			}
			else
			{
				spellSlotImage.fillCenter = false;
			}
		}
	}

	public GameObject ActivateSpell(Spell spellBeingActivated, Vector3 targetingDirection, Vector3 targetingPoint, GameObject targetingUnit, bool upgrade)
	{
		if (spellBeingActivated)
		{
			GameObject spellInstantiation = null;

			SpellTargetingType type = spellBeingActivated.GetTargetType();
			int[] allocatedPoints = new int[3];
			if (upgrade)
				allocatedPoints = new int[3]{1,1,1};

			switch (type)
			{
				case SpellTargetingType.Projectile:
					spellInstantiation = spellBeingActivated.Activate(this.gameObject, allocatedPoints, targetingDirection);
					break;
				case SpellTargetingType.Area:
					spellInstantiation = spellBeingActivated.Activate(this.gameObject, allocatedPoints, targetingPoint);
					break;
				case SpellTargetingType.Unit:
					spellInstantiation = spellBeingActivated.Activate(this.gameObject, allocatedPoints, targetingUnit);
					break;
			}

			if (spellInstantiation != null)
			{
				print("Successfully created spell instantiation!");
			}
			else
			{
				manaHandler.SpendMana(spellBeingActivated.GetManaCost());
            }
			return spellInstantiation;
		}
		return null;
	}
}