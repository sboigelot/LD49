using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndLevelScreen : MonoBehaviour
{
	public GameObject LevelSelectScreenPrefab;

	public Text TitleText;
	public Text ScoreText;

	public Color WinColor;
	public Color LooseColor;

	public Image Cog1;
	public Image Cog2;
	public Image Cog3;

	public void Open(Level level, int endScore)
	{
		bool win = endScore > level.TargetScores[0];

		TitleText.text = win ? "Congratulation! You won!" : "Hooooo... Better luck next time!";
		ScoreText.text = "You scored " + endScore + " points";
		Cog1.color = win ? WinColor : LooseColor;

		Cog2.color = endScore > level.TargetScores[1] ? WinColor : LooseColor;
		Cog3.color = endScore > level.TargetScores[2] ? WinColor : LooseColor;
	}

	public void RestartLevel()
	{
		GameInfo.CurrentLevel = GameManager.Instance.CurrentLevel;
		SceneManager.LoadScene("Scenes/TrainStation", LoadSceneMode.Single);
	}

	public void Close()
	{
		Instantiate(LevelSelectScreenPrefab, transform.parent);
		Destroy(gameObject);
	}
}
