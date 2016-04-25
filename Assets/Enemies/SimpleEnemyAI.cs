using UnityEngine;
using System.Collections;

public class SimpleEnemyAI : MonoBehaviour
{
	public CharacterController controller;

	public bool aggressive;
	public float aggroDistance;
	public float approachDistance;
	public float visionRadius = .5f;
	public LayerMask visionMask;
	public float timeToLoseTarget;
	public float durationOfSearch;
	private float missingTarget;
    public PulsatingEmission emissionHandler;
	public Color nonaggroColor;
	public Color aggroColor;
	public float nonaggroPulsation;
	public float aggroPulsation;

	public float minDistanceForUseSpell;
	public float leadTargetMod;
	public float distanceLeadModDecayFactor;

	private GameObject playerTarget;
	private bool lostPlayerTarget;
    private float distanceToPlayer;

	public LayerMask groundMask;
	public float hoverHeight;
	public float movementSpeedDecayFactor;

	public Spell spellCastAtPlayer;

	public float timeBetweenCasts;
	private float spellTimer;

	private GameObject spellInstantiation;

	private void Start()
	{
		emissionHandler = GetComponent<PulsatingEmission>();
	}
	
	private void Update ()
	{
		//set emission and light handler properties
		if (playerTarget == null && !lostPlayerTarget)
		{
			emissionHandler.colorOfEmission = nonaggroColor;
			emissionHandler.pulsatingPeriod = nonaggroPulsation;
		}
		else
		{
			emissionHandler.colorOfEmission = aggroColor;
			emissionHandler.pulsatingPeriod = aggroPulsation;
		}

		//try to find player if havent targeted yet
		if (aggressive)
		{
			if (!playerTarget)
			{
				GameObject propsectiveTarget = GameObject.FindGameObjectWithTag("Player");
				if (propsectiveTarget != null)
				{
					Ray rayToTarget = new Ray(transform.position, propsectiveTarget.transform.position - transform.position);
					RaycastHit hitInfo;
					if ((Physics.SphereCast(rayToTarget, visionRadius, out hitInfo, aggroDistance)) && (hitInfo.collider.gameObject == propsectiveTarget))
					{
						playerTarget = propsectiveTarget;
						lostPlayerTarget = false;
						spellTimer = timeBetweenCasts/2f;
					}
				}
			}
			else if (!lostPlayerTarget)
			{
				Ray rayToTarget = new Ray(transform.position, playerTarget.transform.position - transform.position);
				RaycastHit hitInfo;
				if (!((Physics.SphereCast(rayToTarget, visionRadius, out hitInfo, aggroDistance)) && (hitInfo.collider.gameObject == playerTarget)))
				{
					missingTarget += Time.deltaTime;
				}

				if (missingTarget > timeToLoseTarget)
				{
					missingTarget = 0;
					playerTarget = null;
					lostPlayerTarget = true;
                }
			}

			if (lostPlayerTarget)
			{
				missingTarget += Time.deltaTime;
				if (missingTarget > durationOfSearch)
				{
					lostPlayerTarget = false;
					missingTarget = 0f;
                }
			}
		}

		//look towards the player
		if (playerTarget != null)
		{
			transform.LookAt(playerTarget.transform.position);
			transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
			distanceToPlayer = Vector3.Distance(playerTarget.transform.position, transform.position);
		}

		//spell casting logic
		spellTimer += Time.deltaTime;
        if (playerTarget == null)
		{
			spellTimer = 0;
			if (spellInstantiation)
				Destroy(spellInstantiation);
		}
		else
		{
			if (distanceToPlayer <= minDistanceForUseSpell)
			{
				if (spellTimer > timeBetweenCasts)
				{
					spellTimer = 0;
					float modifiedLeadTargetMod = leadTargetMod * Mathf.Pow(distanceToPlayer, distanceLeadModDecayFactor);
                    Vector3 playerLeadPoint = playerTarget.transform.position + playerTarget.GetComponent<FPDPhysics>().GetVelocity() * modifiedLeadTargetMod;
					spellInstantiation = ActivateSpell((playerLeadPoint - transform.position).normalized, playerLeadPoint, playerTarget, false);
				}
				else if (spellTimer > timeBetweenCasts /2f && spellInstantiation)
				{
					Destroy(spellInstantiation);
				}
			}
			else if (spellInstantiation)
			{
				Destroy(spellInstantiation);
			}
        }
	}

	public float GetInputX ()
	{
		return 0f;
	}
	
	public float GetInputY ()
	{
		if (playerTarget)
		{
			float yInput = 1f - Mathf.Pow((aggroDistance - distanceToPlayer) / (aggroDistance - approachDistance), movementSpeedDecayFactor);
			if (distanceToPlayer < approachDistance)
			{
				yInput = -1f / 5 - .2f * Mathf.Sqrt(approachDistance - distanceToPlayer);
			}
			return yInput;
		}
		else if (lostPlayerTarget)
		{
			return 1f;
		}
		else
		{
			return 0f;
		}
	}
	
	public bool IsWalking ()
	{
		return lostPlayerTarget;
	}
	
	public float JumpAmount ()
	{
		RaycastHit[] downwardsRaycast = Physics.RaycastAll(transform.position, Vector3.down, groundMask);
		if (downwardsRaycast.Length > 0)
		{
			RaycastHit hit1 = downwardsRaycast[0];
			if (hit1.distance < hoverHeight || controller.velocity.y < -5f)
			{
				//float jumpAmount = Mathf.Pow((hoverHeight - hit1.distance)/hoverHeight, 1/2f);
				//jumpAmount = Mathf.Clamp(jumpAmount,0,1);
				return 1f;
			}
			else if (hit1.distance > 2 * hoverHeight || controller.velocity.y > 5f)
			{
				//float jumpAmount = -Mathf.Pow((hit1.distance - hoverHeight)/hoverHeight, 1/2f);
				//jumpAmount = Mathf.Clamp(jumpAmount, 0, 1);
				return -1f;
			}
		}
		return 0f;
	}

	public GameObject ActivateSpell(Vector3 targetingDirection, Vector3 targetingPoint, GameObject targetingUnit, bool upgrade)
	{
		if (spellCastAtPlayer)
		{
			GameObject spellInstantiation = null;

			SpellTargetingType type = spellCastAtPlayer.GetTargetType();
			int[] allocatedPoints = new int[3];
			if (upgrade)
				allocatedPoints = new int[3] { 1, 1, 1 };

			switch (type)
			{
				case SpellTargetingType.Area:
					spellInstantiation = spellCastAtPlayer.Activate(this.gameObject, allocatedPoints, targetingPoint);
					break;
				case SpellTargetingType.Unit:
					spellInstantiation = spellCastAtPlayer.Activate(this.gameObject, allocatedPoints, targetingUnit);
					break;
				case SpellTargetingType.Projectile:
					spellInstantiation = spellCastAtPlayer.Activate(this.gameObject, allocatedPoints, targetingDirection);
					break;
			}

			if (spellInstantiation != null)
			{
				print("Successfully created enemy spell instantiation!");
			}
			else
			{
				print("Couldn't create enemy spell instantiation!");
			}

			return spellInstantiation;
		}
		return null;
	}
}
