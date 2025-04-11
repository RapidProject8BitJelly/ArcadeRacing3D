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
    private int _laps = 1;
    private const int LAPS = 3;

    private void Awake()
    {
        _matchController = FindObjectOfType<MatchController>();
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
        _currentCheckpoint = myCheckpoints[0];
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
                if (!myCheckpoints[i - 1].isVisited) CmdShowBackMessage();
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

            _currentCheckpoint = myCheckpoints[0];
            CmdIncreaseLapCounter();
        }
        else if (_laps == LAPS)
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
        gameObject.GetComponent<CarController>().ResetRotation();
        transform.rotation = _currentCheckpoint.checkpoint.teleportPosition.transform.rotation;
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
        _matchController.lapCounterText.text = "Lap: " + _laps + "/" + LAPS;
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
