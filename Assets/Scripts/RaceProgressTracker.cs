using System;
using Mirror;
using UnityEngine;

public class RaceProgressTracker : NetworkBehaviour
{
    
    private RacePath racePath;
    private float progress = 0f;
    private int _currentLap;
    public float NormalizedProgress => progress / racePath.GetPathLength() + _currentLap;
    [Range(0f, 3f)]
    [SerializeField] private float _debugProgress; // tylko do podglądu

    private NetworkConnection _ownerConnection;
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        
        _ownerConnection = connectionToClient;
    }
    private void Start()
    {
        racePath = RacePath.Instance;
        RaceManager.Instance.racers.Add(this);
    }

    private void Update()
    {
        progress = racePath.GetDistanceAlongPath(transform.position);
        _debugProgress = NormalizedProgress;
    }

    public void IncreaseLapCounter()
    {
        _currentLap++;
        Debug.Log(_currentLap);
    }
}

