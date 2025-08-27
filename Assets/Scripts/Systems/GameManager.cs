using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject[] _playersCars;
    private void Start()
    {
        TrafficLights.TrafficLightsEvents.BeginCountdown();
    }

    private void OnEnable()
    {
        GameManagerEvents.SetPlayersCars += SetPlayersCars;
        GameManagerEvents.EnablePlayersCars += EnablePlayersCars;
    }

    private void OnDisable()
    {
        GameManagerEvents.SetPlayersCars -= SetPlayersCars;
        GameManagerEvents.EnablePlayersCars -= EnablePlayersCars;
    }

    private void EnablePlayersCars()
    {
        for(int i = 0; i < _playersCars.Length; i++)
        {
            _playersCars[i].GetComponent<Rigidbody>().isKinematic = false;
        }  
    }

    private void SetPlayersCars(GameObject[] playersCars)
    {
        _playersCars = playersCars;
    }

    public static class GameManagerEvents
    {
        public static Action<GameObject[]> SetPlayersCars;
        public static Action EnablePlayersCars;
    }
}
