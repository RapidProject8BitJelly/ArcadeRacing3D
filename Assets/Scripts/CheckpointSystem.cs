using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    [SerializeField] private List<Checkpoint> checkpoints;
    [SerializeField] private TMP_Text backText;

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
        public static Action<Checkpoint> CanBeCheckpointVisited;
        public static Func<List<Checkpoint>> GetCheckpointsList;
    }
}
