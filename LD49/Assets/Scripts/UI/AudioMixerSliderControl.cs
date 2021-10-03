using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class AudioMixerSliderControl : MonoBehaviour 
{
    public AudioMixer MainMixer;
    
    public AudioSource TestAudioSource;

    public Text VolumeLabel;

    [Range(0f,1.2f)]
    public float InitialVolume = 1f;

    public string FloatName = "volume";
    
    public void Start() 
    {
        var slider = GetComponent<Slider>();
        slider.maxValue = 1.2f;
        slider.minValue = 0f;
		MainMixer.SetFloat(FloatName, Mathf.LerpUnclamped(-80f, 0f, InitialVolume));
        slider.value = InitialVolume;
        if(VolumeLabel!=null)
        {
		    VolumeLabel.text = InitialVolume.ToString("P0");
        }

        slider.onValueChanged.AddListener(SetVolume);
    }

	public void SetVolume(float value)
	{
        if(VolumeLabel!=null)
        {
		    VolumeLabel.text = value.ToString("P0");
        }

		MainMixer.SetFloat(FloatName, Mathf.LerpUnclamped(-80f, 0f, value));
        
        if(!TestAudioSource.isPlaying)
        {
            TestAudioSource.Play();
        }
	}
}