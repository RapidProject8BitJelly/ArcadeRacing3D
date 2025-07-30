using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CarCheckpointController : MonoBehaviour
{
    private class MyCheckpoint
    {
        public Checkpoint checkpoint;
        public bool isVisited;
    }

    private List<MyCheckpoint> myCheckpoints = new();
    private MyCheckpoint _currentCheckpoint;
    private PlayerInputActions _playerInputActions;
    
    public int currentLap = 1;
    private const int LAPS = 3;
    
    private bool _isLastCheckpoint = false;

    private bool hasFinishedRace = false;
    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _playerInputActions.PlayerControl.BackToCheckpoint.started += ResetPlayerPosition;
        _playerInputActions.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.PlayerControl.BackToCheckpoint.started -= ResetPlayerPosition;
        _playerInputActions.Disable();
    }

    private void Start()
    {
        SetMyCheckpoints();
        IncreaseLapCounter();
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
        _currentCheckpoint = myCheckpoints[0];
    }

    public void CheckPointVisited(Checkpoint checkpoint)
    {
        for (int i = 0; i < myCheckpoints.Count; i++)
        {
            if (i == 0 && myCheckpoints[i].checkpoint == checkpoint)
            {
                if (_isLastCheckpoint)
                {
                    CrossedFinishLine();
                    _isLastCheckpoint = false;
                    myCheckpoints[i].isVisited = true;
                    _currentCheckpoint = myCheckpoints[i];
                }
                else if (!_isLastCheckpoint && myCheckpoints[i].isVisited)
                {
                    CmdShowBackMessage();
                }
                else
                {
                    myCheckpoints[i].isVisited = true;
                    _currentCheckpoint = myCheckpoints[i];
                }
            }
            if (i != 0 && myCheckpoints[i].checkpoint == checkpoint)
            {
                if (!myCheckpoints[i - 1].isVisited) CmdShowBackMessage();
                else
                {
                    if (i == myCheckpoints.Count-1)
                    {
                        _isLastCheckpoint = true;
                    }

                    myCheckpoints[i].isVisited = true;
                    _currentCheckpoint = myCheckpoints[i];
                }

                break;
            }
        }
    }

    private void ResetPlayerPosition(InputAction.CallbackContext ctx)
    {
        transform.position = _currentCheckpoint.checkpoint.teleportPosition.transform.position;
        gameObject.GetComponent<CarCon>().SetNewRotation(-_currentCheckpoint.checkpoint.transform.rotation.eulerAngles.y);
        HideBackMessage();
    }

    private void CrossedFinishLine()
    {
        if (currentLap < LAPS)
        {
            currentLap++;
            foreach (MyCheckpoint checkpoint in myCheckpoints)
            {
                checkpoint.isVisited = false;
            }

            _currentCheckpoint = myCheckpoints[0];
            GetComponent<RaceProgressTracker>().IncreaseLapCounter();
            IncreaseLapCounter();
        }
        else if (currentLap == LAPS && !hasFinishedRace)
        {
            hasFinishedRace = true;
            SetFinishText();
            //CmdRequestSpectateLeader();
            GetComponent<RaceProgressTracker>().hasFinishedRace = true;

        }
    }
    
    // [Command]
    // private void CmdRequestSpectateLeader()
    // {
    //     var myIdentity = GetComponent<NetworkIdentity>();
    //     NetworkIdentity leader = MatchController.Instance.GetCurrentLeaderIdentity(myIdentity);
    //     if (leader == null) return;
    //
    //     TargetStartSpectatingLeader(connectionToClient, leader);
    // }
    //
    // [TargetRpc]
    // private void TargetStartSpectatingLeader(NetworkConnection conn, NetworkIdentity leader)
    // {
    //     if (leader == null) return;
    //
    //     CarController carController = GetComponent<CarController>();
    //     if (carController != null)
    //     {
    //         carController.SetSpectateTarget(leader.transform);
    //     }
    // }


    private void CmdShowBackMessage()
    {
        Debug.Log("Back to checkpoint");
        //TargetShowBackMessage(connectionToClient);
    }
    
    private void HideBackMessage()
    {
        Debug.Log("Hide Back message");
    }
    
    private void IncreaseLapCounter()
    {
        Debug.Log("Increased lap counter");
        //_matchController.lapCounterText.text = "Lap: " + currentLap + "/" + LAPS;
    }
    
    private void SetFinishText()
    {
        Debug.Log("FINISH");
    }
    
}
