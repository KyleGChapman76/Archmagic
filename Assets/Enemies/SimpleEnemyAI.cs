using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public Pulsate emissionHandler;
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
	public float maxDistanceToSeeGround;
    public float minimumHoverHeight;
	public float maximumHoverHeight;
	private float hoverHeight;
	private float distanceFromGround;
    public float movementSpeedDecayFactor;

	public Spell spellCastAtPlayer;

	public float timeBetweenCasts;
	public float percentTimeCasting;
    private float spellTimer;

	public float enemyTooCloseDistance;

	private GameObject spellInstantiation;

	private void Start()
	{
		emissionHandler = GetComponent<Pulsate>();
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
				hoverHeight = (minimumHoverHeight + maximumHoverHeight) / 2f;

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
				hoverHeight = playerTarget.transform.position.y - (transform.position.y - distanceFromGround) + 2f;
				hoverHeight = Mathf.Clamp(hoverHeight, minimumHoverHeight, maximumHoverHeight);

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
				else if (spellTimer > (timeBetweenCasts * percentTimeCasting) && spellInstantiation)
				{
					Destroy(spellInstantiation);
				}
			}
			else if (spellInstantiation)
			{
				Destroy(spellInstantiation);
			}
        }

		//get distance from ground
		Ray ray = new Ray(transform.position, Vector3.down);
		RaycastHit hitInfo2;
		if (Physics.Raycast(ray, out hitInfo2, maxDistanceToSeeGround, groundMask))
		{
			distanceFromGround = hitInfo2.distance;
		}
	}

	//strafe away from other enemies
	public float GetInputX ()
	{
		GameObject[] otherEnemies = GameObject.FindGameObjectsWithTag("Enemy");

		float closestDistance = float.MaxValue;
		GameObject closestEnemy = null;

		foreach (GameObject obj in otherEnemies)
		{
			if (obj == gameObject)
				continue;

			Vector3 horizPos = new Vector3(transform.position.x, 0, transform.position.z);
			Vector3 objHorizPos = new Vector3(obj.transform.position.x, 0, obj.transform.position.z);
			float distanceBetween = Vector3.Distance(horizPos, objHorizPos);
            if (distanceBetween < enemyTooCloseDistance)
			{
				if (closestEnemy == null || distanceBetween < closestDistance)
				{
					closestEnemy = obj;
					closestDistance = distanceBetween;
                }
            }
		}

		if (closestEnemy != null)
		{
			Vector3 horizPos = new Vector3(transform.position.x, 0, transform.position.z);
			Vector3 objHorizPos = new Vector3(closestEnemy.transform.position.x, 0, closestEnemy.transform.position.z);
			Vector3 vectorToClosestEnemy = objHorizPos - horizPos;

			float angleLeft = Vector3.Angle(vectorToClosestEnemy, transform.rotation * Vector3.left);
			float angleRight = Vector3.Angle(vectorToClosestEnemy, transform.rotation * Vector3.right);

			float strafingSpeed = (angleLeft < angleRight ? -1f:1f) * (closestDistance - enemyTooCloseDistance);
			strafingSpeed = Mathf.Clamp(strafingSpeed, 0, 1f);
			return strafingSpeed;
		}

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
			return .5f;
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
		if (controller.velocity.y < -5f)
		{
			return 1f;
		}

        if (distanceFromGround < hoverHeight)
		{
			return GetComponent<FPDPhysics>().getMultiplicativeMovementSpeed();
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
