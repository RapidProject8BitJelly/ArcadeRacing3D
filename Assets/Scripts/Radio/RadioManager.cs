using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RadioManager : MonoBehaviour
{
    [Header("Stacje Radiowe")]
    public List<RadioStation> stations = new List<RadioStation>();

    [Header("Ustawienia")]
    public AudioSource audioSource;

    [Header("UI")]
    public Button nextStationButton;
    public Button previousStationButton;
    public TMP_Text stationNameText;

    private int currentStationIndex = 0;
    private float stationStartTime; // Gdzie była stacja
    private float lastStationStopTime; // kiedy wylączono
    private bool isPlaying = false;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        nextStationButton.onClick.AddListener(NextStation);
        previousStationButton.onClick.AddListener(PreviousStation);

        PlayCurrentStation();
    }

    private void Update()
    {
        if (!audioSource.isPlaying && isPlaying)
        {
            PlayCurrentStation(); // jeśli utwór się skończy, odtwarzaj dalej
        }
    }

    private void NextStation()
    {
        SaveStationState();
        currentStationIndex = (currentStationIndex + 1) % stations.Count;
        PlayCurrentStation();
    }

    private void PreviousStation()
    {
        SaveStationState();
        currentStationIndex = (currentStationIndex - 1 + stations.Count) % stations.Count;
        PlayCurrentStation();
    }

    private void PlayCurrentStation()
    {
        if (stations.Count == 0) return;

        RadioStation station = stations[currentStationIndex];

        if (station.tracks.Count == 0)
        {
            Debug.LogWarning("Stacja " + station.stationName + " nie ma utworów!");
            return;
        }

        float elapsedTime = Time.time - lastStationStopTime;
        float totalPlayTime = stationStartTime + elapsedTime;

        AudioClip clip = GetClipAtTime(station, totalPlayTime, out float clipStartTime);

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.time = clipStartTime;
        audioSource.Play();

        if (stationNameText != null)
            stationNameText.text = station.stationName;

        isPlaying = true;
    }

    private AudioClip GetClipAtTime(RadioStation station, float time, out float clipStartTime)
    {
        float accumulatedTime = 0f;
        foreach (var clip in station.tracks)
        {
            if (time < accumulatedTime + clip.length)
            {
                clipStartTime = time - accumulatedTime;
                return clip;
            }
            accumulatedTime += clip.length;
        }

        // jeśli przeszliśmy wszystkie, to od nowa
        clipStartTime = 0f;
        return station.tracks[0];
    }

    private void SaveStationState()
    {
        if (!audioSource.isPlaying) return;

        float currentTrackTime = audioSource.time;
        float pastTracksDuration = 0f;

        for (int i = 0; i < stations[currentStationIndex].tracks.Count; i++)
        {
            if (stations[currentStationIndex].tracks[i] == audioSource.clip)
            {
                break;
            }
            pastTracksDuration += stations[currentStationIndex].tracks[i].length;
        }

        stationStartTime = pastTracksDuration + currentTrackTime;
        lastStationStopTime = Time.time;
        isPlaying = false;
    }
    
    public void StopRadio()
    {
        SaveStationState();
        audioSource.Stop();
    }
}
