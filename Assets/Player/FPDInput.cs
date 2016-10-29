using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class FPDInput : NetworkBehaviour
{
	public bool enableWalking = false;
	public bool enableSprinting = false;

	//whether or not input is disabled (while in a menu or somesuch)
	private bool inputDisabled;
	
	private bool isWalking;
	private bool isJumping;
	private float inputX;
	private float inputY;
	private bool isSprinting;

	public void Start ()
	{
		isWalking = false;
		isJumping = false;
		inputX = 0f;
		inputY = 0f;
		isSprinting = false;
    }
	
	public void Update ()
	{
		if (!isLocalPlayer)
			return;

		isWalking = Input.GetButton("Walk");
		isJumping = Input.GetButton("Jump");
		inputX = Input.GetAxis("Horizontal");
		inputY = Input.GetAxis("Vertical");
		isSprinting = Input.GetButton("Sprint");
	}
	
	public float GetInputX ()
	{
		if (inputDisabled)
			return 0;
		return inputX;
	}
	
	public float GetInputY ()
	{
		if (inputDisabled)
			return 0;
		return inputY;
	}
	
	public bool IsWalking ()
	{	
		if (!enableWalking || inputDisabled)
			return false;
		return isWalking;
	}

	public bool IsSprinting()
	{
		if (!enableSprinting || inputDisabled)
			return false;
		return isSprinting;
	}
	
	public bool IsJumping ()
	{	
		if (inputDisabled)
			return false;
		return isJumping;
	}

	public void DisableInput (bool flag)
	{
		inputDisabled = flag;
		foreach (MouseLook mouselook in GetComponents<MouseLook>())
		{
			mouselook.enabled = !flag;
		}
		foreach (MouseLook mouselook2 in GetComponentsInChildren<MouseLook>())
		{
			mouselook2.enabled = !flag;
		}
		MagicHandler handler = GetComponent<MagicHandler>();
		handler.enabled = !flag;	
	}
	
	public bool IsInputDisabled ()
	{
		return inputDisabled;
	}
}
