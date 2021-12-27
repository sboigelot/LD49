using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public void OpenRaffleScreen()
	{
		SceneManager.LoadScene("Scenes/TwitchQueue", LoadSceneMode.Single);
	}

	public void OpenCounterScreen()
	{
		SceneManager.LoadScene("Scenes/TwitchCounter", LoadSceneMode.Single);
	}
}
