using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Music Parameters")] [SerializeField]
    private AudioSource _music;

    [SerializeField] private float _fadeInMusicSeconds;
    [SerializeField] private float _fadeOutMusicSeconds;
    [SerializeField] private List<AudioClip> _musicClips;

    [Header("Ambient Parameters")] [SerializeField]
    private AudioSource _ambient;

    [SerializeField] private float _fadeInAmbientSeconds;
    [SerializeField] private float _fadeOutAmbientSeconds;
    [SerializeField] private List<AudioClip> _ambientClips;

    public static MusicManager instance;
    
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
        
        _music.clip = _musicClips[Random.Range(0, _musicClips.Count)];
        _ambient.clip = _ambientClips[Random.Range(0, _ambientClips.Count)];
    }

    private void Start()
    {
        _music.volume = 0;
        _ambient.volume = 0;

        PlayMusic();
        PlayAmbient();
    }

    public void PlayMusic()
    {
        StartCoroutine(FadeIn(_music, _fadeInMusicSeconds));
    }

    public void StopMusic()
    {
        StartCoroutine(FadeOut(_music, _fadeOutMusicSeconds));
    }

    public void PlayAmbient()
    {
        StartCoroutine(FadeIn(_ambient, _fadeInAmbientSeconds));
    }

    public void StopAmbient()
    {
        StartCoroutine(FadeOut(_ambient, _fadeOutAmbientSeconds));
    }
    
    private IEnumerator FadeIn(AudioSource audioSource, float fadeInSeconds)
    {
        if (audioSource.clip == null)
        {
            Debug.LogError($"No audio clip set in source: {audioSource.gameObject.name}", audioSource.gameObject);
            yield return null;
        }

        audioSource.Play();
        float timeElapsed = 0;

        while (audioSource.volume < 1)
        {
            audioSource.volume = Mathf.Lerp(0, 1, timeElapsed / fadeInSeconds);
            timeElapsed += Time.deltaTime;

            yield return null;
        }
    }

    private IEnumerator FadeOut(AudioSource audioSource, float fadeOutSeconds)
    {
        if (audioSource.clip == null)
        {
            Debug.LogError($"No audio clip set in source: {audioSource.gameObject.name}", audioSource.gameObject);
            yield return null;
        }

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