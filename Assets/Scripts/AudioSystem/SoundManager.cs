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
        
        foreach ( SoundData s in sounds) 
        {
            s.source = gameObject.AddComponent<AudioSource>();
            
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(Sound sound) 
    {
        SoundData s = Array.Find(sounds, s => s.name == sound);
        if (s == null) 
        {
            return;
        }

        s.source.clip = s.clips[Random.Range(0, s.clips.Count)];
        s.source.Play();
    }
}