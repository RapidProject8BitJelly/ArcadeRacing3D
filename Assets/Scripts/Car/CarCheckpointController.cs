using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CarCheckpointController : MonoBehaviour
{
    [SerializeField] private RaceProgressTracker raceProgressTracker;
    [SerializeField] private CarCon carController;
    [SerializeField] private TempPlayerInfo tempPlayerInfo;
    public PlayerHUD playerHUD;
    
    private class MyCheckpoint
    {
        public Checkpoint checkpoint;
        public bool isVisited;
    }

    private List<MyCheckpoint> myCheckpoints = new();
    private MyCheckpoint _currentCheckpoint;
    private PlayersInputActions _playerInputActions;
    
    public int currentLap = 1;
    private const int LAPS = 3;
    private int buttonPressedBy = 0;
    
    private bool _isLastCheckpoint = false;

    private bool hasFinishedRace = false;
    private void OnEnable()
    {
        _playerInputActions = new PlayersInputActions();
        _playerInputActions.Player1.BackToCheckpoint.performed += context => { buttonPressedBy = 1; ResetPlayerPosition(context); };
        _playerInputActions.Player2.BackToCheckpoint.performed += context => { buttonPressedBy = 0; ResetPlayerPosition(context); };
        _playerInputActions.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.Player1.BackToCheckpoint.performed -= ResetPlayerPosition;
        _playerInputActions.Player2.BackToCheckpoint.performed -= ResetPlayerPosition;
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
        if (tempPlayerInfo.PlayerNumber == (PlayerNumbers)buttonPressedBy)
        {
            transform.root.gameObject.transform.position = _currentCheckpoint.checkpoint.teleportPosition.transform.position;
            carController.SetNewRotation(-_currentCheckpoint.checkpoint.transform.rotation.eulerAngles.y);
            HideBackMessage();
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
            raceProgressTracker.IncreaseLapCounter();
            IncreaseLapCounter();
        }
        else if (currentLap == LAPS && !hasFinishedRace)
        {
            hasFinishedRace = true;
            SetFinishText();
            //CmdRequestSpectateLeader();
            raceProgressTracker.hasFinishedRace = true;

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
        playerHUD.SetAnnouncementBoardText("Back to checkpoint");
        //TargetShowBackMessage(connectionToClient);
    }
    
    private void HideBackMessage()
    {
        playerHUD.SetAnnouncementBoardText("");
    }
    
    private void IncreaseLapCounter()
    {
        playerHUD.IncreaseLapCounter(currentLap, LAPS);
        //_matchController.lapCounterText.text = "Lap: " + currentLap + "/" + LAPS;
    }
    
    private void SetFinishText()
    {
        playerHUD.SetAnnouncementBoardText("FINISH");
    }
    
}
