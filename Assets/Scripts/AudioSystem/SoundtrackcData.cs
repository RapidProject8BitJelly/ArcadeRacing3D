using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class SoundtrackData
{
    public Soundtrack name;
    
    public bool isAmbient;
    
    public AudioClip soundtrackClip;
    
    [Range(0f, 1f)]
    public float volume = 1;
 
    [Range(-3f, 3f)]
    public float pitch = 1;
    
    public bool loop = true;
    
    public float fadeInSoundtrackSeconds = 1;
    public float fadeOutSoundtrackSeconds = 1;
}