using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject[] _playersCars;
    [SerializeField] private PlayerHUD[] _playerHUD;
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
            
            _playersCars[i].GetComponentInChildren<CarCheckpointController>().enabled = true;
            _playersCars[i].GetComponentInChildren<CarCheckpointController>().playerHUD = _playerHUD[i];
            
        }
    }

    private void SetPlayersCars(GameObject[] playersCars)
    {
        _playersCars = playersCars;
        EndGamePanel.EndGamePanelEvents.SetPlayersCount(_playersCars.Length);
        for(int i = 0; i < _playersCars.Length;i++)
        {
            _playersCars[i].GetComponent<RaceProgressTracker>().enabled = true;
            gameObject.GetComponent<RaceManager>().racers.Add(_playersCars[i].GetComponent<RaceProgressTracker>());
        }
    }

    public static class GameManagerEvents
    {
        public static Action<GameObject[]> SetPlayersCars;
        public static Action EnablePlayersCars;
    }
}
