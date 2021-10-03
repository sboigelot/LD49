using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectScreen : MonoBehaviour
{
	public Transform LevelButtonGrid;

	public GameObject LevelButtonPrefab;

	public List<Level> Levels;

	public void Start()
	{
		if (Levels == null)
		{
			return;
		}

		if (LevelButtonGrid.childCount > 0)
		{
			return;
		}

		foreach (var level in Levels)
		{
			CreateLevelButton(level);
		}		
	}

	public void CreateLevelButton(Level level)
	{
		var levelButton = Instantiate(LevelButtonPrefab, LevelButtonGrid);
		levelButton.GetComponent<LevelButton>().Level = level;
	}

	public void Quit()
	{
		Destroy(gameObject);
	}
}
