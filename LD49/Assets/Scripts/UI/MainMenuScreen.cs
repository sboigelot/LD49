using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
	public Transform ScreenPlaceholder;

	public GameObject LevelSelectScreenPrefab;

	public GameObject OptionScreenPrefab;

	public GameObject CreditScreenPrefab;

	public Image OverlayBackground;

	public Button CloseButton;

	public bool IsOverlay;

	public void Start()
	{
		if (OverlayBackground != null)
		{
			OverlayBackground.enabled = IsOverlay;
		}

		if (CloseButton != null)
		{
			CloseButton.gameObject.SetActive(IsOverlay);
		}
	}

	public void OpenSelectLevelScreen()
	{
		OpenScreen(LevelSelectScreenPrefab);
	}

	public void OpenOptionsScreen()
	{
		OpenScreen(OptionScreenPrefab);
	}

	public void OpenCreditScreen()
	{
		OpenScreen(CreditScreenPrefab);
	}

	public void Close()
	{
		Destroy(gameObject);
		Time.timeScale = 1f;
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void OpenScreen(GameObject screenPrefab)
	{
		Instantiate(screenPrefab, ScreenPlaceholder);
	}
}
