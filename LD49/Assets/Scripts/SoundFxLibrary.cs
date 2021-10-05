using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SoundFxLibrary : MonoBehaviour
{
    public static SoundFxLibrary Instance { get; set; }

    private AudioSource AudioSource;
    
    public AudioSource SecondaryAudioSource;

    public AudioClip AudioClipClick;
    
    public AudioClip[] BrakeBarrel;
    public AudioClip[] BrakeCargo;
    public AudioClip[] BreakBag;
    public AudioClip[] CollideBag;
    public AudioClip[] CollideBarrel;
    public AudioClip[] CollideCrate;
    public AudioClip[] BubblePop;
    public AudioClip[] EventGravity;
    public AudioClip[] TrainWhisle;
    public AudioClip[] LastTrain;
    public AudioClip[] ForcePush;
    public AudioClip[] Vortex;

    public AudioClip[] TrainRun;

    public AudioClip[] pop;
    public AudioClip[] UIClick;

    public AudioSource[] AudioSources;
    public int AudioSourceIndex = -1;

    private void Start() 
    {
        AudioSource = GetComponent<AudioSource>();
        Instance = this;    
    }

    public static void PlayClick()
    {
        PlaySound(Instance.AudioClipClick);
    }

    public static AudioSource PlayRandom(AudioClip[] audioClipRandomSample, bool secondary = false)
    {
        return PlaySound(audioClipRandomSample[Random.Range(0, audioClipRandomSample.Length)], secondary);
    }

    public static AudioSource PlaySound( AudioClip sound, bool secondary = false)
    {        
        if(sound == null)
        {
            Debug.LogWarning("Missing sound");
            return null;
        }

        Instance.AudioSourceIndex = (Instance.AudioSourceIndex+1) % (Instance.AudioSources.Length-1);

        var audioSource = Instance.AudioSources[Instance.AudioSourceIndex];
        //var audioSource = secondary ? Instance.SecondaryAudioSource : Instance.AudioSource;
        audioSource.loop = false;
        audioSource.Stop();
        audioSource.clip = sound;
        audioSource.Play();

        return audioSource;
    }
}