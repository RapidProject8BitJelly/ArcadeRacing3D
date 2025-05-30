using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    private List<Checkpoint> checkpoints = new();
    [SerializeField] private GameObject checkpointNode;
    
    private void Awake()
    {
        foreach (Transform child in checkpointNode.transform)
        {
            checkpoints.Add(child.gameObject.GetComponent<Checkpoint>());
        }
    }
    
    private void OnEnable()
    {
        CheckpointSystemEvents.GetCheckpointsList += GetCheckpointsList;
    }

    private void OnDisable()
    {
        CheckpointSystemEvents.GetCheckpointsList -= GetCheckpointsList;
    }

    private List<Checkpoint> GetCheckpointsList()
    {
        return checkpoints;
    }
    
    public static class CheckpointSystemEvents
    {
        public static Func<List<Checkpoint>> GetCheckpointsList;
    }
}
