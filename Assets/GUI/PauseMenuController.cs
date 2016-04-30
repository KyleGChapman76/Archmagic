using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour {

	private bool pauseMenuEnabled;
	public GameObject pauseScreen;
	public string mainMenuName;

	private void Start ()
	{
		pauseMenuEnabled = false;
        Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update ()
	{
		if (Input.GetButtonDown("Pause"))
		{
			pauseMenuToggle();
		}
	}

	public void pauseMenuToggle()
	{
		if (pauseMenuEnabled)
		{
			disablePauseMenu();
		}
		else
		{
			enablePauseMenu();
		}
	}

	public void disablePauseMenu()
	{
		pauseMenuEnabled = false;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		Time.timeScale = 1.0f;
		pauseScreen.SetActive(false);
	}

	public void enablePauseMenu()
	{
		pauseMenuEnabled = true;
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
		Time.timeScale = 0f;
		pauseScreen.SetActive(true);
	}

	public void exitToMenu()
	{
		SceneManager.LoadScene(mainMenuName);
	}
}
