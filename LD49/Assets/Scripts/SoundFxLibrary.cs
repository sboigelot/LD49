using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SoundFxLibrary : MonoBehaviour
{
    public static SoundFxLibrary Instance { get; set; }

    private AudioSource AudioSource;
    
    public AudioSource SecondaryAudioSource;

    public AudioClip AudioClipClick;
    
    public AudioClip[] AudioClipRandomSample;

    private void Start() 
    {
        AudioSource = GetComponent<AudioSource>();
        Instance = this;    
    }

    public static void PlayClick()
    {
        PlaySound(Instance.AudioClipClick);
    }

    public static void PlayRandomSoundExample()
    {
        PlaySound(Instance.AudioClipRandomSample[UnityEngine.Random.Range(0, Instance.AudioClipRandomSample.Length)], true);
    }

    public static void PlaySound(AudioClip sound, bool secondary = false)
    {        
        if(sound == null)
        {
            Debug.LogWarning("Missing sound");
            return;
        }

        var audioSource = secondary ? Instance.SecondaryAudioSource : Instance.AudioSource;
        audioSource.loop = false;
        audioSource.Stop();
        audioSource.clip = sound;
        audioSource.Play();
    }
}