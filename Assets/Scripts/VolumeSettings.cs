using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    public AudioMixer EffectsMixer;
    public AudioMixer MusicMixer;

    public void SetSoundLevel(float sliderValue)
    {
        EffectsMixer.SetFloat("Sound", Mathf.Log10(sliderValue) * 20);
    }

    public void SetMusicLevel(float sliderValue)
    {
        MusicMixer.SetFloat("Music", Mathf.Log10(sliderValue) * 20);
    }
}
