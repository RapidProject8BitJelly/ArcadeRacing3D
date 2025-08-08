using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour 
{
    public SoundData[] sounds;

    public static SoundManager instance;
        
    private void Awake() 
    {
        if (instance == null) 
        {
            instance = this;
        }
        else 
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        
        foreach ( SoundData sound in sounds) 
        {
            var audioSource = new GameObject("AudioSource - " + sound.name);
            audioSource.transform.parent = transform;
            
            sound.source = audioSource.AddComponent<AudioSource>();
            
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;

            if (sound.enableLowPass)
            {
                var lowPass = audioSource.AddComponent<AudioLowPassFilter>();
                lowPass.cutoffFrequency = sound.lowPassCutoffFrequency;
                lowPass.lowpassResonanceQ = sound.lowPassResonanceQ;
            }
            
            if (sound.enableHighPass)
            {
                var highPass = audioSource.AddComponent<AudioHighPassFilter>();
                highPass.cutoffFrequency = sound.highPassCutoffFrequency;
                highPass.highpassResonanceQ = sound.highPassResonanceQ;
            }
        }
    }

    public void Play(Sound soundName) 
    {
        SoundData sound = Array.Find(sounds, sound => sound.name == soundName);
        if (sound == null) 
        {
            return;
        }

        sound.source.clip = sound.clips[Random.Range(0, sound.clips.Count)];
        sound.source.Play();
    }
}