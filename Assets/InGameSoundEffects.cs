using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class InGameSoundEffects : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider volumeSlider;

    public void ChangeVolume()
    {
        mixer.SetFloat("Effects", volumeSlider.value);
    }
}
