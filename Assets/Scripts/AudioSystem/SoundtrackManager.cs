using System;
using System.Collections;
using UnityEngine;

public class SoundtrackManager : MonoBehaviour
{
    public SoundtrackData[] soundtracks;
    
    public static SoundtrackManager instance;
    
    private AudioSource _musicAudioSource;
    private AudioSource _ambientAudioSource;
    
    private SoundtrackData _currentMusicData;
    private SoundtrackData _currentAmbientData;
    
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
        
        _musicAudioSource = gameObject.AddComponent<AudioSource>();
        _ambientAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySoundtrack(Soundtrack soundtrackName)
    {
        SoundtrackData soundtrackData = Array.Find(soundtracks, soundtrack => soundtrack.name == soundtrackName);

        if (soundtrackData.isAmbient)
        {
            _ambientAudioSource.volume = soundtrackData.volume;
            _ambientAudioSource.pitch = soundtrackData.pitch;
            _ambientAudioSource.loop = soundtrackData.loop;
            _currentAmbientData = soundtrackData;
        
            StartCoroutine(FadeIn(_ambientAudioSource, soundtrackData.soundtrackClip, soundtrackData.fadeInSoundtrackSeconds));
        }
        else
        {
            _musicAudioSource.volume = soundtrackData.volume;
            _musicAudioSource.pitch = soundtrackData.pitch;
            _musicAudioSource.loop = soundtrackData.loop;
            _currentMusicData = soundtrackData;
        
            StartCoroutine(FadeIn(_musicAudioSource, soundtrackData.soundtrackClip, soundtrackData.fadeInSoundtrackSeconds));
        }
    }

    public void StopMusic()
    {
        StartCoroutine(FadeOut(_musicAudioSource, _currentMusicData.fadeOutSoundtrackSeconds));
    }

    public void StopAmbient()
    {
        StartCoroutine(FadeOut(_ambientAudioSource, _currentAmbientData.fadeOutSoundtrackSeconds));
    }
    
    private IEnumerator FadeIn(AudioSource audioSource, AudioClip audioClip, float fadeInSeconds)
    {
        if (audioClip == null)
        {
            Debug.LogError($"No audio clip set in music data: {_currentMusicData.name}");
            yield return null;
        }

        audioSource.clip = audioClip;
        audioSource.volume = 0;
        audioSource.Play();
        float timeElapsed = 0;

        while (_musicAudioSource.volume < 1)
        {
            audioSource.volume = Mathf.Lerp(0, 1, timeElapsed / fadeInSeconds);
            timeElapsed += Time.deltaTime;

            yield return null;
        }
    }

    private IEnumerator FadeOut(AudioSource audioSource, float fadeOutSeconds)
    {
        float timeElapsed = 0;

        while (audioSource.volume > 0)
        {
            audioSource.volume = Mathf.Lerp(1, 0, timeElapsed / fadeOutSeconds);
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        audioSource.Stop();
    }
}