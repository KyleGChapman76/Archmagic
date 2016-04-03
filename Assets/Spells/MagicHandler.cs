using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class MagicHandler : MonoBehaviour
{
	public const float UPGRADE_MANA_COST_INCREASE = 2.0f;
	public const int FIRE_ELEMENT = 1;
	public const int WATER_ELEMENT = 2;
	public const int EARTH_ELEMENT = 3;
	public const int AIR_ELEMENT = 4;

	public GameObject hand1;
	public GameObject hand2;

	public Animator playerArm1Animator;
	public Animator playerArm2Animator;

	//the fields pointing to the current hand particle emitters and the public descriptor of where and how big they should appear
	private GameObject handParticles1;
	private GameObject handParticles2;
	public Vector3 handPosition;
	public Vector3 handSize;

	//prefabs for the particle systems that are placed on the hands for each elements
	public GameObject firehandParticlePefab;
	public GameObject waterhandParticlePefab;
	public GameObject earthhandParticlePefab;
	public GameObject airhandParticlePefab;

	//references to the spells that could currently be cast with the left hand, right hand, or both
	private Spell spellToCast1;
	private string spellToCast1Name;
	private Spell spellToCast2;
	private string spellToCast2Name;
	private Spell spellToCastCombo;
	private string spellToCastComboName;

	GameObject aoeIndicator;

	public GameObject spellPanel;
	private GameObject[] spellSlots;

	//whether each element is allowed to be used or not
	public bool fireEnabled;
	public bool airEnabled;
	public bool waterEnabled;
	public bool earthEnabled;

	private bool[] elements;

	//which elements are selected in each hands, and the color constants used to color elements that are selected
	private int selectedElement1;
	private int selectedElement2;
	public Color selectedColor = new Color(135f / 255f, 242f / 255f, 139f / 252f);
	public Color doubleSelectedColor = new Color(105f / 255f, 212f / 255f, 109f / 252f);

	//whether the mouse input for the given hands is pressed or not
	private bool spell1Input;
	private bool spell2Input;
	private bool comboInput;

	//whether or not the given spell is actually being cast currently (or both for the combo spell)
	private bool spell1BeingCast;
	private bool spell2BeingCast;

	//the instantiations of the spell objects when the spell is actually being cast
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
		timeBetweenMana = 1;
	}

	private void Update()
	{
		elements = new bool[] { fireEnabled, airEnabled, waterEnabled, earthEnabled };

		FindCurrentSpells();

		//tell the animators how long they have been in their current animations
		playerArm1Animator.SetFloat("TimeInAnimation", playerArm1Animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
		playerArm2Animator.SetFloat("TimeInAnimation", playerArm2Animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

		MagicInput();

		Spell spellStarting1 = comboInput ? spellToCastCombo : spellToCast1;
		Spell spellStarting2 = comboInput ? spellToCastCombo : spellToCast2;

		//tell the animators if the player is inputting for that arm or not
		playerArm1Animator.SetBool("ArmInput", spell1Input);
		playerArm2Animator.SetBool("ArmInput", spell2Input);

		playerArm1Animator.SetBool("ContinuousSpell", spellStarting1 && spellStarting1.continuouslyActivated);

		if (playerArm1Animator.GetCurrentAnimatorStateInfo(0).IsName("CastingAnimation") && playerArm1Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
			spell1BeingCast = true;

		playerArm2Animator.SetBool("ContinuousSpell", spellStarting2 && spellStarting2.continuouslyActivated);

		if (playerArm2Animator.GetCurrentAnimatorStateInfo(0).IsName("CastingAnimation") && playerArm2Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
			spell2BeingCast = true;

		//activate combo spell
		if (spell1BeingCast && spell2BeingCast && spellComboInstantiation == null && spellToCastCombo)
		{
			print("Activating the combo spell!");
			bool upgraded = selectedElement1 == selectedElement2;

			try
			{
				spellComboInstantiation = TargetAndCast(spellToCastCombo, upgraded);
			}
			catch {}
			
			if (spellToCastCombo.continuouslyActivated)
				timeBetweenMana = 1f / spellToCastCombo.manaCost;
			else
			{
				playerArm1Animator.SetBool("ForceTransition", true);
				playerArm2Animator.SetBool("ForceTransition", true);
			}
        }
		//activate spell 1
		if (spell1BeingCast && !spell2BeingCast && spell1Instantiation == null && spellToCast1)
		{
			print("Activating spell 1!");

			try
			{
				spell1Instantiation = TargetAndCast(spellToCast1, false);
			}
            catch { }

			if (spellToCast1.continuouslyActivated)
				timeBetweenMana = 1f / spellToCast1.manaCost;
			else
				playerArm1Animator.SetBool("ForceTransition", true);
		}
		//activate spell 2
		if (!spell1BeingCast && spell2BeingCast && spell2Instantiation == null && spellToCast2)
		{
			print("Activating spell 2!");

			try
			{
				spell2Instantiation = TargetAndCast(spellToCast2, false);
			}
			catch {}


			if (spellToCast2.continuouslyActivated)
				timeBetweenMana = 1f / spellToCast2.manaCost;
			else
				playerArm2Animator.SetBool("ForceTransition", true);
		}

		if (playerArm1Animator.GetCurrentAnimatorStateInfo(0).IsName("IdleAnimation"))
			playerArm1Animator.SetBool("ForceTransition", false);

		if (playerArm2Animator.GetCurrentAnimatorStateInfo(0).IsName("IdleAnimation"))
			playerArm2Animator.SetBool("ForceTransition", false);

		//deactivate spells
		bool deactivatingSpell1 = false;
		if (playerArm1Animator.GetCurrentAnimatorStateInfo(0).IsName("ReverseCastingAnimation"))
		{
			if (spell1Instantiation)
				Destroy(spell1Instantiation);
			spell1BeingCast = false;
			deactivatingSpell1 = true;
        }

		bool deactivatingSpell2 = false;
		if (playerArm2Animator.GetCurrentAnimatorStateInfo(0).IsName("ReverseCastingAnimation"))
		{
			if (spell2Instantiation)
				Destroy(spell2Instantiation);
			spell2BeingCast = false;
			deactivatingSpell2 = true;
        }

		if (deactivatingSpell1 && deactivatingSpell2 && spellComboInstantiation)
		{
			Destroy(spellComboInstantiation);
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
		int selected1Before = selectedElement1;
		int selected2Before = selectedElement2;

		//arm animation inputs
		spell1Input = Input.GetAxis("Fire2") == 1;
		spell2Input = Input.GetAxis("Fire1") == 1;
		comboInput = spell1Input && spell2Input;

		//element selection UX
		for (int i = 1; i <= 4; i++)
		{
			if (Input.GetButtonDown("WeaponSwitch" + i))
			{
				int elementSelected = i;
				if (elements[elementSelected - 1])
				{
					if (!spell1Input && selectedElement1 != elementSelected)
					{
						selectedElement1 = elementSelected;
					}
					else if(!spell2Input)
					{
						selectedElement2 = elementSelected;
					}
				}
			}
		}

		float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
		if (scrollWheel > 0) //scrolling upwards
		{
			int slotSelected = selectedElement1;
			if (slotSelected >= 4)
				slotSelected -= 4;

			if (elements[slotSelected])
			{
				if (!spell1Input)
				{
					selectedElement1 = slotSelected;
				}
				else if (!spell2Input)
				{
					selectedElement2 = slotSelected;
				}
			}
			
        }
		else if (scrollWheel < 0) //scrolling downwards
		{
			int slotSelected = selectedElement1;
			if (slotSelected >= 4)
				slotSelected -= 4;

			if (elements[slotSelected])
			{
				if (!spell1Input)
				{
					selectedElement1 = slotSelected;
				}
				else if (!spell2Input)
				{
					selectedElement2 = slotSelected;
				}
			}
		}

		//set the correct hand particles
		if (!handParticles1 || selectedElement1 != selected1Before)
		{
			Destroy(handParticles1);
			handParticles1 = Instantiate(GetParticlesForElement(selectedElement1), Vector3.zero, Quaternion.identity) as GameObject;
			handParticles1.transform.parent = hand1.transform;
			handParticles1.transform.localPosition = handPosition;
			handParticles1.transform.localScale = handSize;
			handParticles1.transform.rotation = Quaternion.identity;
			handParticles1.layer = LayerMask.NameToLayer("ViewModelsRight");
        }
		if (!handParticles2 || selectedElement2 != selected2Before)
		{
			Destroy(handParticles2);
			handParticles2 = Instantiate(GetParticlesForElement(selectedElement2), Vector3.zero, Quaternion.identity) as GameObject;
			handParticles2.transform.parent = hand2.transform;
			handParticles2.transform.localPosition = handPosition;
			handParticles2.transform.localScale = handSize;
			handParticles2.transform.rotation = Quaternion.identity;
			handParticles2.layer = LayerMask.NameToLayer("ViewModelsLeft");
		}
	}

	private void FindCurrentSpells()
	{
		spellToCast1 = SpellBook.FindSpell(new int[] { selectedElement1});
		spellToCast1Name = spellToCast1 ? spellToCast1.GetName() : "null";
		spellToCast2 = SpellBook.FindSpell(new int[] { selectedElement2});
		spellToCast2Name = spellToCast2 ? spellToCast2.GetName() : "null";
		if (selectedElement1 == selectedElement2)
			spellToCastCombo = SpellBook.FindSpell(new int[] { selectedElement1 });
		else
			spellToCastCombo = SpellBook.FindSpell(new int[] { selectedElement1, selectedElement2 });
		spellToCastComboName = spellToCastCombo ? spellToCastCombo.GetName() : "null";
	}

	private GameObject GetParticlesForElement(int element)
	{
		switch (element)
		{
			case FIRE_ELEMENT:
				return firehandParticlePefab;
			case WATER_ELEMENT:
				return waterhandParticlePefab;
			case EARTH_ELEMENT:
				return earthhandParticlePefab;
			case AIR_ELEMENT:
				return airhandParticlePefab;
		}
		return airhandParticlePefab;
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
				int initialManaCost = spellBeingActivated.GetManaCost();
				if (upgrade)
					initialManaCost = (int)(initialManaCost * UPGRADE_MANA_COST_INCREASE);
                manaHandler.SpendMana(initialManaCost);
            }
			return spellInstantiation;
		}
		return null;
	}
}