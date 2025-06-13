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
            sound.source = gameObject.AddComponent<AudioSource>();
            
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }

    public void Play(Sound soundName) 
    {
        SoundData sound = Array.Find(sounds, s => s.name == soundName);
        if (sound == null) 
        {
            return;
        }

        sound.source.clip = sound.clips[Random.Range(0, sound.clips.Count)];
        sound.source.Play();
    }
}