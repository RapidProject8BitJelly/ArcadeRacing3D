using System;
using Mirror;
using UnityEngine;

public class RaceProgressTracker : NetworkBehaviour
{
    private RacePath racePath;
    private float progress = 0f;
    public float NormalizedProgress => progress / racePath.GetPathLength();
    [Range(0f, 1f)]
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
     
   
}

