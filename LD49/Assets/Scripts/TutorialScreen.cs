using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScreen : MonoBehaviour
{
    public int NextIndex;
    public bool IsLast;

    public AudioClip VoiceOver;

    // Start is called before the first frame update
    public void Start()
    {
        int index = transform.GetSiblingIndex();
        NextIndex = index + 1;
        IsLast = transform.parent.childCount <= NextIndex;
        gameObject.SetActive(index == 0);

        if (index == 0)
        {
            Open();
        }
    }

    public void Open()
    {
        gameObject.SetActive(true);
        PlayVoiceOver();
        Time.timeScale = 0f;
    }

    public void PlayVoiceOver()
    {
        if (VoiceOver == null)
        {
            return;
        }

        SoundFxLibrary.PlaySound(VoiceOver);
    }

    public void Skip()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Next()
    {
        gameObject.SetActive(false);
        if (!IsLast)
        {
            transform.parent.GetChild(NextIndex).gameObject.GetComponent<TutorialScreen>().Open();
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

}
