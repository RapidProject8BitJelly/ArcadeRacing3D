using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundData
{
    public Sound name;
    
    public List<AudioClip> clips;
    
    [Range(0f, 1f)]
    public float volume = 1;
 
    [Range(-3f, 3f)]
    public float pitch = 1;
    
    [Space(20)]
    
    public bool enableLowPass;
    [Range(10, 22000)]
    public float lowPassCutoffFrequency = 5007;
    public float lowPassResonanceQ = 1;
    
    [Space(20)]
    
    public bool enableHighPass;
    [Range(10, 22000)]
    public float highPassCutoffFrequency = 5007;
    public float highPassResonanceQ = 1;

    public bool loop;

    [HideInInspector] public AudioSource source;
}