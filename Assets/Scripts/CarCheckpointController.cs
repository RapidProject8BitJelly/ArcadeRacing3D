using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CarCheckpointController : NetworkBehaviour
{
    private class MyCheckpoint
    {
        public Checkpoint checkpoint;
        public bool isVisited;
    }

    private List<MyCheckpoint> myCheckpoints = new();
    private MatchController _matchController;
    private MyCheckpoint _currentCheckpoint;
    private Quaternion _currentRotation;

    private int _laps = 1;
    private const int LAPS = 3;

    private void Awake()
    {
        _matchController = FindObjectOfType<MatchController>();
        _currentRotation = transform.rotation;
    }

    private void Start()
    {
        SetMyCheckpoints();
        CmdIncreaseLapCounter();
    }

    private void Update()
    {
        if (isLocalPlayer && Input.GetKeyDown(KeyCode.R))
        {
            CmdRequestReset();
        }
    }

    private void SetMyCheckpoints()
    {
        List<Checkpoint> checkpoints = CheckpointSystem.CheckpointSystemEvents.GetCheckpointsList();
        foreach (var checkpoint in checkpoints)
        {
            MyCheckpoint myCheckpoint = new MyCheckpoint();
            myCheckpoint.checkpoint = checkpoint;
            myCheckpoint.isVisited = false;
            myCheckpoints.Add(myCheckpoint);
        }
    }

    public void CheckPointVisited(Checkpoint checkpoint)
    {
        for (int i = 0; i < myCheckpoints.Count; i++)
        {
            if (i == 0 && myCheckpoints[i].checkpoint == checkpoint)
            {
                myCheckpoints[i].isVisited = true;
                _currentCheckpoint = myCheckpoints[i];
            }
            if (i != 0 && myCheckpoints[i].checkpoint == checkpoint)
            {
                if (!myCheckpoints[i - 1].isVisited) TargetShowBackMessage();
                else
                {
                    if (myCheckpoints[i].checkpoint.isFinish)
                    {
                        CrossedFinishLine();
                    }
                    else
                    {
                        myCheckpoints[i].isVisited = true;
                        _currentCheckpoint = myCheckpoints[i];
                    }
                }

                break;
            }
        }
    }

    private void CrossedFinishLine()
    {
        if (_laps < LAPS)
        {
            _laps++;
            foreach (MyCheckpoint checkpoint in myCheckpoints)
            {
                checkpoint.isVisited = false;
            }
            CmdIncreaseLapCounter();
        }
    }

    [TargetRpc]
    private void TargetShowBackMessage()
    {
        _matchController.infoText.text = "Wróć do poprzedniego punktu używając klawisza R.";
        _matchController.infoText.color = Color.red;
        _matchController.infoText.gameObject.SetActive(true);
    }

    [TargetRpc]
    private void TargetHideBackMessage(NetworkConnection conn)
    {
        _matchController.infoText.gameObject.SetActive(false);
    }

    [TargetRpc]
    private void ClientResetPosition(NetworkConnection conn)
    {
        transform.position = _currentCheckpoint.checkpoint.teleportPosition.transform.position;
        gameObject.GetComponent<Rigidbody>().MoveRotation(_currentRotation);
        CmdHideBackMessage();
    }

    [TargetRpc]
    private void TargetIncreaseLapCounter(NetworkConnection conn)
    {
        _matchController.lapCounterText.text = "Lap: " + _laps + "/" + LAPS;
    }

    [Command]
    private void CmdIncreaseLapCounter()
    {
        TargetIncreaseLapCounter(connectionToClient);
    }
    
    [Command]
    private void CmdRequestReset()
    {
        ClientResetPosition(connectionToClient);
    }

    [Command]
    private void CmdHideBackMessage()
    {
        TargetHideBackMessage(connectionToClient);
    }
}
