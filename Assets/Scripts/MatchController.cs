using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkMatch))]
public class MatchController : NetworkBehaviour
{
    #region Variables
    public static MatchController Instance { get; private set; }
    internal readonly SyncDictionary<NetworkIdentity, MatchPlayerData> MatchPlayerData = new SyncDictionary<NetworkIdentity, MatchPlayerData>();
    private bool _playAgain = false;
    #endregion

    #region GUI
    [Header("GUI References")]
    // References to the GUI elements
    public CanvasGroup canvasGroup;
    public Button exitButton;
    public Button playAgainButton;
    public TMP_Text lapCounterText;
    public TMP_Text infoText;

    [Header("Diagnostics")]
    [ReadOnly, SerializeField] internal CanvasController canvasController;
    #endregion

    #region Player
    [ReadOnly, SerializeField] internal NetworkIdentity player1;
    [ReadOnly, SerializeField] internal NetworkIdentity player2;

    [ReadOnly, SerializeField] internal List<NetworkIdentity> players = new List<NetworkIdentity>();
    
    [Header("Player Starting Positions")]
    public Vector3[] startingPositions = new Vector3[]
    {
        new Vector3(-6, 0, 0),
        new Vector3(6, 0, 0),
        new Vector3(-3, 0, -3),
        new Vector3(3, 0, -3)
    };

    #endregion

    #region Networking
    public override void OnStartServer()
    {
        StartCoroutine(AddPlayersToMatchController());
    }

    public override void OnStartClient()
    {
        lapCounterText.text = "Laps: 1";
        //leaderboardText.text = "Pos: 1";

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        exitButton.gameObject.SetActive(false);
        playAgainButton.gameObject.SetActive(false);
    }

    [ClientRpc]
    private void RpcStartCountdown()
    {
        StartCoroutine(StartCountdown());
    }

    [Command(requiresAuthority = false)]
    private void CmdEnablePlayerCars()
    {
        RpcEnablePlayerCars();
    }

    [ClientRpc]
    private void RpcEnablePlayerCars()
    {
        foreach (var player in MatchPlayerData)
        {
            player.Value.carController.enabled = true;
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdDisablePlayerCars()
    {
        RpcDisablePlayerCars();
    }

    [ClientRpc]
    private void RpcDisablePlayerCars()
    {
        foreach (var player in MatchPlayerData)
        {
            player.Value.carController.enabled = false;
            player.Value.carController.StopCar();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdShowWinner(NetworkIdentity winner)
    {
        RpcShowWinner(winner);
    }

    [ClientRpc]
    private void RpcShowWinner(NetworkIdentity winner)
    {
        if (winner.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            infoText.text = "Winner!";
            infoText.color = Color.blue;
        }
        else
        {
            infoText.text = "Loser!";
            infoText.color = Color.red;
        }

        exitButton.gameObject.SetActive(true);
        playAgainButton.gameObject.SetActive(true);
    }

    // Assigned in inspector to ReplayButton::OnClick
    [ClientCallback]
    public void RequestPlayAgain()
    {
        playAgainButton.gameObject.SetActive(false);
        CmdPlayAgain();
    }

    [Command(requiresAuthority = false)]
    private void CmdPlayAgain()
    {
        if (!_playAgain)
            _playAgain = true;
        else
        {
            _playAgain = false;
            RestartGame();
        }
    }

    [ServerCallback]
    private void RestartGame()
    {
        // Restart the game on the server
        NetworkIdentity[] keys = new NetworkIdentity[MatchPlayerData.Keys.Count];
        MatchPlayerData.Keys.CopyTo(keys, 0);

        foreach (NetworkIdentity identity in keys)
        {
            MatchPlayerData mpd = MatchPlayerData[identity];
            MatchPlayerData[identity] = mpd;
        }

        RpcResetPlayerPositions();
        RpcRestartGame();
        RpcStartCountdown();
    }

    [ClientRpc]
    private void RpcRestartGame()
    {
        exitButton.gameObject.SetActive(false);
        playAgainButton.gameObject.SetActive(false);
    }

    [ClientRpc]
    private void RpcResetPlayerPositions()
    {
        for (int i = 0; i < players.Count && i < startingPositions.Length; i++)
        {
            var player = players[i];
            player.transform.position = startingPositions[i];
            player.transform.rotation = Quaternion.identity;
        }
    }


    // Assigned in inspector to BackButton::OnClick
    [Client]
    public void RequestExitGame()
    {
        exitButton.gameObject.SetActive(false);
        playAgainButton.gameObject.SetActive(false);
        CmdRequestExitGame();
    }

    [Command(requiresAuthority = false)]
    private void CmdRequestExitGame(NetworkConnectionToClient sender = null)
    {
        StartCoroutine(ServerEndMatch(sender, false));
    }

    [ServerCallback]
    public void OnPlayerDisconnected(NetworkConnectionToClient conn)
    {
        if (players.Contains(conn.identity))
        {
            StartCoroutine(ServerEndMatch(conn, true));
        }
    }

    [ServerCallback]
    private IEnumerator ServerEndMatch(NetworkConnectionToClient conn, bool disconnected)
    {
        RpcExitGame();
        canvasController.OnPlayerDisconnected -= OnPlayerDisconnected;

        yield return new WaitForSeconds(0.1f);

        if (!disconnected)
        {
            foreach (var player in players)
            {
                NetworkServer.RemovePlayerForConnection(player.connectionToClient, RemovePlayerOptions.Destroy);
                CanvasController.waitingConnections.Add(player.connectionToClient);
            }
        }
        else
        {
            foreach (var player in players)
            {
                if (player.connectionToClient != conn)
                {
                    NetworkServer.RemovePlayerForConnection(player.connectionToClient, RemovePlayerOptions.Destroy);
                    CanvasController.waitingConnections.Add(player.connectionToClient);
                }
            }
        }

        yield return null;

        canvasController.SendMatchList();
        NetworkServer.Destroy(gameObject);
    }


    [ClientRpc]
    private void RpcExitGame()
    {
        canvasController.OnMatchEnded();
        //canvasController.minimap.SetActive(false);
    }
    #endregion

    #region Unity Callbacks
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        
        // Initialize the canvas controller
        canvasController = GameObject.FindObjectOfType<CanvasController>(); //TODO: DO ZMIANY
    }
    #endregion

    #region Methods
    // IEnumerator AddPlayersToMatchController()
    // {
    //     yield return null;
    //
    //     MatchPlayerData.Add(player1, new MatchPlayerData
    //     {
    //         playerIndex = CanvasController.playerInfos[player1.connectionToClient].playerIndex,
    //         carController = player1.GetComponent<CarController>()
    //     });
    //     MatchPlayerData.Add(player2, new MatchPlayerData
    //     {
    //         playerIndex = CanvasController.playerInfos[player2.connectionToClient].playerIndex,
    //         carController = player2.GetComponent<CarController>()
    //     });
    //
    //     RpcStartCountdown();
    // }
    
    IEnumerator AddPlayersToMatchController()
    {
        yield return null;

        foreach (var player in players)
        {
            if (!MatchPlayerData.ContainsKey(player) && MatchPlayerData.Count < 4)
            {
                MatchPlayerData.Add(player, new MatchPlayerData
                {
                    playerIndex = CanvasController.playerInfos[player.connectionToClient].playerIndex,
                    carController = player.GetComponent<CarController>()
                });
            }
        }

        RpcStartCountdown();
    }

    private IEnumerator StartCountdown()
    {
        infoText.text = "3";
        infoText.color = Color.white;
        yield return new WaitForSeconds(1f);

        infoText.text = "2";
        yield return new WaitForSeconds(1f);

        infoText.text = "1";
        yield return new WaitForSeconds(1f);

        infoText.text = "Start!";
        infoText.color = Color.green;

        CmdEnablePlayerCars();

        yield return new WaitForSeconds(1f);

        infoText.text = "";
    }
    
    public void ResetCarLapCounters()
    {
        /*CarLapCounter[] carLapCounters = FindObjectsOfType<CarLapCounter>();
        foreach (CarLapCounter carLapCounter in carLapCounters) carLapCounter.Reset();*/
    }
    #endregion
    
    public NetworkIdentity GetCurrentLeaderIdentity(NetworkIdentity excludePlayer)
    {
        RaceProgressTracker leaderTracker = null;
        float bestProgress = float.MinValue;

        foreach (var kvp in MatchPlayerData)
        {
            var identity = kvp.Key;
            if (identity == excludePlayer) continue;

            var tracker = identity.GetComponent<RaceProgressTracker>();
            if (tracker == null || tracker.hasFinishedRace) continue;

            if (tracker.NormalizedProgress > bestProgress)
            {
                bestProgress = tracker.NormalizedProgress;
                leaderTracker = tracker;
            }
        }

        return leaderTracker != null ? leaderTracker.GetComponent<NetworkIdentity>() : null;
    }
}
