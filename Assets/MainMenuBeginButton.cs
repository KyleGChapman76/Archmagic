using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuBeginButton : MonoBehaviour
{
	public string firstLevel;

	public void MoveToFirstLevel()
	{
		SceneManager.LoadScene(firstLevel);
	}
}
