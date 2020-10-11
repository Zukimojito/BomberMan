using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

public class SetMusic : MonoBehaviour
{
    public AudioMixer Mixer;

    public void SetVolume(float SliderValue)
    {
        Mixer.SetFloat("MusicGameVol", Mathf.Log10(SliderValue) * 20);
    }
}
