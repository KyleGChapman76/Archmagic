// original by Eric Haines (Eric5h5)
// adapted by @torahhorse
// http://wiki.unity3d.com/index.php/FPSWalkerEnhanced

using UnityEngine;
using System.Collections;

public class FPDPhysics: MonoBehaviour
{
	public FPDInput fpdInput;
	public SimpleEnemyAI enemyAI;
	public CharacterController controller;

	public float walkSpeed = 6.0f;
    public float runSpeed = 10.0f;
	public float sprintSpeed = 14f;
 
    public float jumpSpeed = 4.0f;
    public float gravity = 10.0f;
    
    public float mass = 5f;
    public float airDragPercent = 1;
    public float groundDragPercent = 5;
	public float slipperyDragPercent = 1;
    public float maxAirSpeed = 20f;
    public float knockbackFactor = 1f;
    public float controlFactor = 1f;
    public float controlFactorTimer = 0;
	public int knockedBackTimer;
    
	// Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
	public float antiBumpFactor = .75f;
    
	// Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
	public int antiBunnyHopFactor = 0;
	
	// If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed; otherwise it's about 1.4 times faster
	private bool limitDiagonalSpeed = true;
 
    // Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
    private float fallingDamageThreshold = 10.0f;
    
    //the ratio of how much horizontal momentum should be retained when the character jumps
	private float horizontalJumpMomentumRatio = .9f;
 
    // If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
    public bool slideWhenOverSlopeLimit = false;
 
    // If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
    public bool slideOnTaggedObjects = false;
 
    public float slideSpeed = 5.0f;
    
	public float slideLimit;
 
    // the degree to which the object can accelerate directly in midair
    public float airControl = 0f;

	public bool floating = false;

	private Vector3 moveDirection;
    private bool grounded = false;
    private float speed;
	private float jumpAmount; //from .001 to 1
	private float inputX;
	private float inputY;
    private float fallStartLevel;
    private bool falling;
 
    private float rayDistance;
    private int jumpTimer;
    
    private Vector3 velocity;
    
    private float multiplicativeMovementSpeed = 1.0f;
 
	private void Start()
    {
        speed = walkSpeed;
        jumpTimer = antiBunnyHopFactor;
    }
 
	private void FixedUpdate()
	{
		// Move the controller, and set grounded true or false depending on whether we're standing on something
		CollisionFlags movementResult = controller.Move(velocity * Time.deltaTime);
        grounded = (movementResult & CollisionFlags.Below) != 0;
		bool hitCeiling = (movementResult & CollisionFlags.Above) != 0;

		//get input balues either from the player or from AI
		if (fpdInput != null)
		{
			inputX = fpdInput.GetInputX();
			inputY = fpdInput.GetInputY();
			speed = fpdInput.IsWalking() ? walkSpeed : runSpeed;
			speed = fpdInput.IsSprinting() ? sprintSpeed : speed;
			speed *= Mathf.Max(0f, multiplicativeMovementSpeed);
			jumpAmount = fpdInput.IsJumping() ? 1 : 0;
		}
		else if (enemyAI != null)
		{
			inputX = enemyAI.GetInputX();
			inputY = enemyAI.GetInputY();
			speed = enemyAI.IsWalking() ? walkSpeed : runSpeed;
			speed *= Mathf.Max(0f, multiplicativeMovementSpeed);
			jumpAmount = enemyAI.JumpAmount();
		}
		else
		{
			fpdInput = GetComponent<FPDInput>();
			inputX = 0;
			inputY = 0;
			speed = 0;
			jumpAmount = 0;
		}
		
		if (controlFactorTimer > 0)
		{
			controlFactorTimer -= Time.deltaTime;
			if (controlFactorTimer <= 0)
			{
				print("Resetting control factor.");
				controlFactor = 1f;
			}
		}
		
		// If both horizontal x any z movement are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
		float inputModifyFactor = (inputX != 0.0f && inputY != 0.0f && limitDiagonalSpeed)? .7071f : 1.0f;
		
		//physics
		moveDirection = new Vector3(inputX * inputModifyFactor, -antiBumpFactor, inputY * inputModifyFactor);

		if (grounded)
		{
			// If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
			if (falling)
			{
				falling = false;
				if (transform.position.y < fallStartLevel - fallingDamageThreshold)
					FallingDamageAlert(fallStartLevel - transform.position.y);
			}

			// Jump! But only if the jump button has been released and player has been grounded for a given number of frames
			if (jumpAmount > 0)
			{
				velocity.x *= horizontalJumpMomentumRatio;
				velocity.y = jumpSpeed * jumpAmount;
				velocity.z *= horizontalJumpMomentumRatio;
			}
			else
			{
				velocity.y = 0;
			}
		}
		else
		{
			if (floating)
			{
				if (jumpAmount != 0)
				{
					velocity.y += jumpSpeed * jumpAmount * Time.deltaTime;
				}
				velocity *= (1-airDragPercent/100f);
            }
			else if (!falling) // If we stepped over a cliff or something, set the height at which we started falling
			{
				falling = true;
				fallStartLevel = transform.position.y;
			}

			if (hitCeiling)
			{
				velocity.y *= 0f;
            }
		}
		
		//transform the moveDirection vector to the local axes
		moveDirection = transform.TransformDirection(moveDirection);

		if (!grounded)
		{
			velocity.y -= gravity * Time.deltaTime;
		}

		if (grounded) //grounded movement mechanics
		{
			Vector2 horizVelocity = new Vector2(velocity.x, velocity.z);

			if (controlFactor >= 1f)
			{
				velocity.x = moveDirection.x * speed;
				velocity.z = moveDirection.z * speed;
			}
			else
			{
				velocity.x += moveDirection.x * speed * controlFactor * controlFactor;
				velocity.z += moveDirection.z * speed * controlFactor * controlFactor;

				float drag = slipperyDragPercent * controlFactor;
				velocity.x *= (1 - drag / 100f);
				velocity.z *= (1 - drag / 100f);
			}
		}
		else if (airControl > 0) //air movement mechanics
		{
			Vector2 horizVelocity = new Vector2(velocity.x, velocity.z);
			speed *= airControl * .1f;

			if (controlFactor >= 1f)
			{
				velocity.x += moveDirection.x * speed;
				velocity.z += moveDirection.z * speed;
			}
			else
			{
				velocity.x += moveDirection.x * speed * controlFactor * controlFactor;
				velocity.z += moveDirection.z * speed * controlFactor * controlFactor;

				float drag = slipperyDragPercent * controlFactor;
				velocity.x *= (1 - drag / 100f);
				velocity.z *= (1 - drag / 100f);
			}
		}
		else //air-strafing mechanics
		{
			Vector3 horizVel = new Vector3(velocity.x, 0, velocity.z);
			Vector3 horizAccel = new Vector3(moveDirection.x, 0, moveDirection.z);

			Vector3 proj = Vector3.Project(horizVel, horizAccel);
			if (proj.magnitude <= maxAirSpeed)
			{
				velocity.x += moveDirection.x;
				velocity.z += moveDirection.z;
			}
		}
    }

	public Vector3 GetVelocity ()
	{
		return velocity;
	}

	public void SetVelocity (Vector3 newVel)
	{
		velocity = newVel;
    }
    
    private void OnControllerColliderHit (ControllerColliderHit hit)
    {
		if (hit.collider.GetComponent<Rigidbody>() != null)
		{
			hit.collider.GetComponent<Rigidbody>().AddForceAtPosition(hit.moveDirection, hit.point);
		}
    }
 
    // If falling damage occured, this is the place to do something about it
    private void FallingDamageAlert (float fallDistance)
    {
        //print ("Ouch! Fell " + fallDistance + " units!");
    }
    
	public void Knockback (Vector3 force)
	{
		velocity += force*knockbackFactor;
	}
	
	public void SetControlFactor (float controlFactor)
	{
		if (controlFactor < 0f || controlFactor > 1f)
		{
			print("Error! Trying to set Control Factor to an invalid value " + controlFactor);
			return;
		}
			
		print(controlFactor);
		this.controlFactor = controlFactor;
		controlFactorTimer = 3;
	}

	public void ChangeMultiplicativeMovementSpeed(float change)
	{
		multiplicativeMovementSpeed += change;
	}

	public float GetMultiplicativeMovementSpeed()
	{
		return multiplicativeMovementSpeed;
	}

	public void ResetMultiplicativeMovementSpeed()
	{
		multiplicativeMovementSpeed = 1f;
    }
}