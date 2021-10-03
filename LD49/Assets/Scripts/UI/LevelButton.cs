using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public Level Level;

    public Text LevelDisplayName;

    public Image LevelDisplayImage;

    public void Start()
    {
        if (Level == null)
        {
            return;
        }

        if (LevelDisplayName != null)
        {
            LevelDisplayName.text = Level.DisplayName;
        }

        if (LevelDisplayImage != null)
        {
            LevelDisplayImage.sprite = Level.Image;
        }
    }

    public void StartLevel()
    {
        if (Level == null)
        {
            return;
        }

        GameInfo.CurrentLevel = Level;
        SceneManager.LoadScene("Scenes/TrainStation", LoadSceneMode.Single);
    }
}
