using UnityEngine;
using System.Collections;

public class FPDInput : MonoBehaviour
{
	public bool enableWalking = false;

	//whether or not input is disabled (while in a menu or somesuch)
	private bool inputDisabled;
	
	private bool isWalking;
	private bool isJumping;
	private float inputX;
	private float inputY;

	public void Start ()
	{
	}
	
	public void Update ()
	{
		isWalking = Input.GetButton("Walk");
		isJumping = Input.GetButton("Jump");
		inputX = Input.GetAxis("Horizontal");
		inputY = Input.GetAxis("Vertical");
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
