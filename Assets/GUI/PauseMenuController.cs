using UnityEngine;
using System.Collections;

public class PauseMenuController : MonoBehaviour {

	private bool pauseMenuEnabled;
	public GameObject pauseScreen;

	void Start ()
	{
		pauseMenuEnabled = false;
        Cursor.lockState = CursorLockMode.Locked;
	}

	void Update ()
	{
		if (Input.GetButtonDown("Pause"))
		{
			if (pauseMenuEnabled)
			{
				pauseMenuEnabled = false;
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				Time.timeScale = 1.0f;
				pauseScreen.SetActive(false);
			}
			else
			{
				pauseMenuEnabled = true;
				Cursor.lockState = CursorLockMode.Confined;
				Cursor.visible = true;
				Time.timeScale = 0f;
				pauseScreen.SetActive(true);

			}
		}

	}
}
