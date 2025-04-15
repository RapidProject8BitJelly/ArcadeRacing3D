using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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
    private PlayerInputActions _playerInputActions;
    
    public int currentLap = 1;
    private const int LAPS = 3;
    
    private bool _isLastCheckpoint = false;

    private void Awake()
    {
        _matchController = FindObjectOfType<MatchController>();
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
        CmdIncreaseLapCounter();
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
        if (isLocalPlayer)
        {
            CmdRequestReset();
        }
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
            gameObject.GetComponent<RaceProgressTracker>().IncreaseLapCounter();
            CmdIncreaseLapCounter();
        }
        else if (currentLap == LAPS)
        {
            CmdSetFinishText();
        }
    }

    [Command]
    private void CmdShowBackMessage()
    {
        TargetShowBackMessage(connectionToClient);
    }
    
    [TargetRpc]
    private void TargetShowBackMessage(NetworkConnection conn)
    {
        _matchController.infoText.text = "Wróć do poprzedniego punktu używając klawisza R.";
        _matchController.infoText.color = Color.red;
        _matchController.infoText.gameObject.SetActive(true);
    }
    
    [Command]
    private void CmdHideBackMessage()
    {
        TargetHideBackMessage(connectionToClient);
    }
    
    [TargetRpc]
    private void TargetHideBackMessage(NetworkConnection conn)
    {
        _matchController.infoText.gameObject.SetActive(false);
    }
    
    [Command]
    private void CmdRequestReset()
    {
        TargetResetPosition(connectionToClient);
    }
    
    [TargetRpc]
    private void TargetResetPosition(NetworkConnection conn)
    {
        transform.position = _currentCheckpoint.checkpoint.teleportPosition.transform.position;
        gameObject.GetComponent<CarController>().SetNewRotation(-_currentCheckpoint.checkpoint.transform.rotation.eulerAngles.y);
        CmdHideBackMessage();
    }
    
    [Command]
    private void CmdIncreaseLapCounter()
    {
        TargetIncreaseLapCounter(connectionToClient);
    }
    
    [TargetRpc]
    private void TargetIncreaseLapCounter(NetworkConnection conn)
    {
        _matchController.lapCounterText.text = "Lap: " + currentLap + "/" + LAPS;
    }

    [Command]
    private void CmdSetFinishText()
    {
        TargetSetFinishText(connectionToClient);
    }
    
    [TargetRpc]
    private void TargetSetFinishText(NetworkConnection conn)
    {
        _matchController.infoText.gameObject.SetActive(true);
        _matchController.infoText.text = "Congratulations!";
        _matchController.infoText.color = Color.red;
    }
}
