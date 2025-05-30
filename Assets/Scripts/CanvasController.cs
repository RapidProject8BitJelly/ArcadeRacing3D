using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
    {
        /// <summary>
        /// Match Controllers listen for this to terminate their match and clean up
        /// </summary>
        public event Action<NetworkConnectionToClient> OnPlayerDisconnected;

        /// <summary>
        /// Cross-reference of client that created the corresponding match in openMatches below
        /// </summary>
        internal static readonly Dictionary<NetworkConnectionToClient, Guid> playerMatches = new Dictionary<NetworkConnectionToClient, Guid>();

        /// <summary>
        /// Open matches that are available for joining
        /// </summary>
        internal static readonly Dictionary<Guid, MatchInfo> openMatches = new Dictionary<Guid, MatchInfo>();

        /// <summary>
        /// Network Connections of all players in a match
        /// </summary>
        internal static readonly Dictionary<Guid, HashSet<NetworkConnectionToClient>> matchConnections = new Dictionary<Guid, HashSet<NetworkConnectionToClient>>();

        /// <summary>
        /// Player informations by Network Connection
        /// </summary>
        internal static readonly Dictionary<NetworkConnection, PlayerInfo> playerInfos = new Dictionary<NetworkConnection, PlayerInfo>();

        /// <summary>
        /// Network Connections that have neither started nor joined a match yet
        /// </summary>
        internal static readonly List<NetworkConnectionToClient> waitingConnections = new List<NetworkConnectionToClient>();

        /// <summary>
        /// GUID of a match the local player has created
        /// </summary>
        internal Guid localPlayerMatch = Guid.Empty;

        /// <summary>
        /// GUID of a match the local player has joined
        /// </summary>
        internal Guid localJoinedMatch = Guid.Empty;

        /// <summary>
        /// GUID of a match the local player has selected in the Toggle Group match list
        /// </summary>
        internal Guid selectedMatch = Guid.Empty;

        // Used in UI for "Player #"
        int playerIndex = 1;

        [Header("GUI References")]
        public GameObject matchList;
        public GameObject matchPrefab;
        public GameObject matchControllerPrefab;
        public Button createButton;
        public Button joinButton;
        public GameObject lobbyView;
        public GameObject roomView;
        public RoomGUI roomGUI;
        public ToggleGroup toggleGroup;
        public List<GameObject> players;
        public PlayerNicknamePanel playerNicknamePanel;
        
        private string matchName;
        private byte maxPlayers;

        private int currentPlayer;
        //public GameObject minimap; 
        
        private Vector3[] startingPositions = new Vector3[]
        {
            new Vector3(-4, 0, 0),
            new Vector3(4, 0, 0),
            new Vector3(-4, 0, -4),
            new Vector3(4, 0, -4),
        };


        // RuntimeInitializeOnLoadMethod -> fast playmode without domain reload
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void ResetStatics()
        {
            playerMatches.Clear();
            openMatches.Clear();
            matchConnections.Clear();
            playerInfos.Clear();
            waitingConnections.Clear();
        }

        #region UI Functions

        // Called from several places to ensure a clean reset
        //  - MatchNetworkManager.Awake
        //  - OnStartServer
        //  - OnStartClient
        //  - OnClientDisconnect
        //  - ResetCanvas
        internal void InitializeData()
        {
            playerMatches.Clear();
            openMatches.Clear();
            matchConnections.Clear();
            waitingConnections.Clear();
            localPlayerMatch = Guid.Empty;
            localJoinedMatch = Guid.Empty;
        }

        // Called from OnStopServer and OnStopClient when shutting down
        void ResetCanvas()
        {
            InitializeData();
            lobbyView.SetActive(false);
            roomView.SetActive(false);
            gameObject.SetActive(false);
        }

        #endregion

        #region Button Calls

        /// <summary>
        /// Called from <see cref="MatchGUI.OnToggleClicked"/>
        /// </summary>
        /// <param name="matchId"></param>
        [ClientCallback]
        public void SelectMatch(Guid matchId)
        {
            if (matchId == Guid.Empty)
            {
                selectedMatch = Guid.Empty;
                joinButton.interactable = false;
            }
            else
            {
                if (!openMatches.Keys.Contains(matchId))
                {
                    joinButton.interactable = false;
                    return;
                }

                selectedMatch = matchId;
                MatchInfo infos = openMatches[matchId];
                joinButton.interactable = infos.players < infos.maxPlayers && playerNicknamePanel.playerNickname.Length > 0;
            }
        }

        /// <summary>
        /// Assigned in inspector to Create button
        /// </summary>
        [ClientCallback]
        public void RequestCreateMatch()
        {
            NetworkClient.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Create });
        }

        /// <summary>
        /// Assigned in inspector to Cancel button
        /// </summary>
        [ClientCallback]
        public void RequestCancelMatch()
        {
            if (localPlayerMatch == Guid.Empty) return;

            NetworkClient.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Cancel });
        }

        /// <summary>
        /// Assigned in inspector to Join button
        /// </summary>
        [ClientCallback]
        public void RequestJoinMatch()
        {
            if (selectedMatch == Guid.Empty) return;

            NetworkClient.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Join, matchId = selectedMatch });
        }

        /// <summary>
        /// Assigned in inspector to Leave button
        /// </summary>
        [ClientCallback]
        public void RequestLeaveMatch()
        {
            if (localJoinedMatch == Guid.Empty) return;

            NetworkClient.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Leave, matchId = localJoinedMatch });
        }
        
        /// <summary>
        /// Assigned in inspector to Start button
        /// </summary>
        [ClientCallback]
        public void RequestStartMatch()
        {
            if (localPlayerMatch == Guid.Empty) return;

            NetworkClient.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Start });
        }

        public void RequestReadyChange()
        {
            if (localPlayerMatch == Guid.Empty && localJoinedMatch == Guid.Empty) return;
            Guid matchId = localPlayerMatch == Guid.Empty ? localJoinedMatch : localPlayerMatch;
            
            NetworkClient.Send(new ServerMatchMessage { serverMatchOperation = ServerMatchOperation.Ready, matchId = matchId });
        }

        public void RequestCarCustomization(int carIndex, int colourIndex, int accessoriesIndex)
        {
            if (localPlayerMatch == Guid.Empty && localJoinedMatch == Guid.Empty) return;
            Guid matchId = localPlayerMatch == Guid.Empty ? localJoinedMatch : localPlayerMatch;
            
            NetworkClient.Send(new UpdatePlayerCarMessage {matchId = matchId, carIndex = carIndex, 
                colourIndex = colourIndex, accessoriesIndex = accessoriesIndex});
        }
        
        /// <summary>
        /// Called from <see cref="newScripts.MatchController.RpcExitGame"/>
        /// </summary>
        [ClientCallback]
        public void OnMatchEnded()
        {
            localPlayerMatch = Guid.Empty;
            localJoinedMatch = Guid.Empty;
            ShowLobbyView();
        }

        #endregion

        #region Server & Client Callbacks

        // Methods in this section are called from MatchNetworkManager's corresponding methods

        [ServerCallback]
        internal void OnStartServer()
        {
            InitializeData();
            NetworkServer.RegisterHandler<ServerMatchMessage>(OnServerMatchMessage);
            NetworkServer.RegisterHandler<CreateMatchMessage>(OnCreateLobbyMessage);
            NetworkServer.RegisterHandler<SetPlayerNickname>(OnSetPlayerNickname);
            NetworkServer.RegisterHandler<ReadyToMatchMessage>(OnReadyToMatchMessage);
            NetworkServer.RegisterHandler<UpdatePlayerCarMessage>(OnUpdatePlayerCar);
        }

        private void OnReadyToMatchMessage(NetworkConnectionToClient conn, ReadyToMatchMessage msg)
        {
            if (playerInfos[conn].playerIndex != msg.playerIndex) return;
            if (localPlayerMatch == Guid.Empty && localJoinedMatch == Guid.Empty) return;

            Guid matchId = localPlayerMatch == Guid.Empty ? localJoinedMatch : localPlayerMatch;

            OnServerPlayerReady(conn, matchId);
        }

        private void OnUpdatePlayerCar(NetworkConnectionToClient conn, UpdatePlayerCarMessage msg)
        {
            OnServerCarUpdate(conn, msg.matchId, msg.carIndex, msg.colourIndex, msg.accessoriesIndex);
        }
        
        private void OnSetPlayerNickname(NetworkConnectionToClient conn, SetPlayerNickname msg)
        {
            var playerInfo = playerInfos[conn];
            playerInfo.playerName = msg.nickname;
            playerInfos[conn] = playerInfo;
        }

        [ServerCallback]
        internal void OnServerReady(NetworkConnectionToClient conn)
        {
            waitingConnections.Add(conn);
            playerInfos.Add(conn, new PlayerInfo { playerIndex = this.playerIndex, ready = false, rotationAngle = 90});
            playerIndex++;

            SendMatchList();
        }

        [ServerCallback]
        internal IEnumerator OnServerDisconnect(NetworkConnectionToClient conn)
        {
            // Invoke OnPlayerDisconnected on all instances of MatchController
            OnPlayerDisconnected?.Invoke(conn);

            if (playerMatches.TryGetValue(conn, out Guid matchId))
            {
                playerMatches.Remove(conn);
                openMatches.Remove(matchId);

                foreach (NetworkConnectionToClient playerConn in matchConnections[matchId])
                {
                    PlayerInfo _playerInfo = playerInfos[playerConn];
                    _playerInfo.ready = false;
                    _playerInfo.matchId = Guid.Empty;
                    playerInfos[playerConn] = _playerInfo;
                    playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Departed });
                }
            }

            foreach (KeyValuePair<Guid, HashSet<NetworkConnectionToClient>> kvp in matchConnections)
                kvp.Value.Remove(conn);

            PlayerInfo playerInfo = playerInfos[conn];
            if (playerInfo.matchId != Guid.Empty)
            {
                if (openMatches.TryGetValue(playerInfo.matchId, out MatchInfo matchInfo))
                {
                    matchInfo.players--;
                    openMatches[playerInfo.matchId] = matchInfo;
                }

                HashSet<NetworkConnectionToClient> connections;
                if (matchConnections.TryGetValue(playerInfo.matchId, out connections))
                {
                    PlayerInfo[] infos = connections.Select(playerConn => playerInfos[playerConn]).ToArray();

                    foreach (NetworkConnectionToClient playerConn in matchConnections[playerInfo.matchId])
                        if (playerConn != conn)
                            playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.UpdateRoom, playerInfos = infos, myPlayerIndex = playerInfos[conn].playerIndex });
                }
            }

            SendMatchList();

            yield return null;
        }

        [ServerCallback]
        internal void OnStopServer()
        {
            ResetCanvas();
        }

        [ClientCallback]
        internal void OnClientConnect()
        {
            playerInfos.Add(NetworkClient.connection, new PlayerInfo { playerIndex = this.playerIndex, ready = false, rotationAngle = 90});
        }

        [ClientCallback]
        internal void OnStartClient()
        {
            InitializeData();
            ShowLobbyView();
            createButton.gameObject.SetActive(true);
            joinButton.gameObject.SetActive(true);
            NetworkClient.RegisterHandler<ClientMatchMessage>(OnClientMatchMessage);
        }

        private void OnCreateLobbyMessage(NetworkConnectionToClient conn, CreateMatchMessage msg)
        {
            matchName = msg.matchName;
            maxPlayers = msg.maxPlayers;
        }

        [ClientCallback]
        internal void OnClientDisconnect()
        {
            InitializeData();
        }

        [ClientCallback]
        internal void OnStopClient()
        {
            ResetCanvas();
        }

        #endregion

        #region Server Match Message Handlers

        [ServerCallback]
        void OnServerMatchMessage(NetworkConnectionToClient conn, ServerMatchMessage msg)
        {
            switch (msg.serverMatchOperation)
            {
                case ServerMatchOperation.None:
                    {
                        Debug.LogWarning("Missing ServerMatchOperation");
                        break;
                    }
                case ServerMatchOperation.Create:
                    {
                        OnServerCreateMatch(conn);
                        break;
                    }
                case ServerMatchOperation.Cancel:
                    {
                        OnServerCancelMatch(conn);
                        break;
                    }
                case ServerMatchOperation.Join:
                    {
                        OnServerJoinMatch(conn, msg.matchId);
                        break;
                    }
                case ServerMatchOperation.Leave:
                    {
                        OnServerLeaveMatch(conn, msg.matchId);
                        break;
                    }
                case ServerMatchOperation.Ready:
                    {
                        OnServerPlayerReady(conn, msg.matchId);
                        break;
                    }
                case ServerMatchOperation.Start:
                    {
                        OnServerStartMatch(conn);
                        break;
                    }
            }
        }

        [ServerCallback]
        void OnServerCreateMatch(NetworkConnectionToClient conn)
        {
            if (playerMatches.ContainsKey(conn)) return;

            Guid newMatchId = Guid.NewGuid();
            matchConnections.Add(newMatchId, new HashSet<NetworkConnectionToClient>());
            matchConnections[newMatchId].Add(conn);
            playerMatches.Add(conn, newMatchId);
            openMatches.Add(newMatchId, new MatchInfo { matchName = matchName, matchId = newMatchId, 
                maxPlayers = maxPlayers, players = 1 });
            
            PlayerInfo playerInfo = playerInfos[conn];
            playerInfo.ready = false;
            playerInfo.matchId = newMatchId;
            playerInfos[conn] = playerInfo;

            PlayerInfo[] infos = matchConnections[newMatchId].Select(playerConn => playerInfos[playerConn]).ToArray();
            
            conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Created, matchId = newMatchId, playerInfos = infos, myPlayerIndex = playerInfos[conn].playerIndex });

            SendMatchList();
        }

        [ServerCallback]
        void OnServerCancelMatch(NetworkConnectionToClient conn)
        {
            if (!playerMatches.ContainsKey(conn)) return;

            conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Cancelled });

            Guid matchId;
            if (playerMatches.TryGetValue(conn, out matchId))
            {
                playerMatches.Remove(conn);
                openMatches.Remove(matchId);

                foreach (NetworkConnectionToClient playerConn in matchConnections[matchId])
                {
                    PlayerInfo playerInfo = playerInfos[playerConn];
                    playerInfo.ready = false;
                    playerInfo.matchId = Guid.Empty;
                    playerInfos[playerConn] = playerInfo;
                    playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Departed });
                }

                SendMatchList();
            }
        }

        [ServerCallback]
        void OnServerJoinMatch(NetworkConnectionToClient conn, Guid matchId)
        {
            if (!matchConnections.ContainsKey(matchId) || !openMatches.ContainsKey(matchId)) return;

            MatchInfo matchInfo = openMatches[matchId];
            matchInfo.players++;
            openMatches[matchId] = matchInfo;
            matchConnections[matchId].Add(conn);

            PlayerInfo playerInfo = playerInfos[conn];
            playerInfo.ready = false;
            playerInfo.matchId = matchId;
            playerInfos[conn] = playerInfo;

            PlayerInfo[] infos = matchConnections[matchId].Select(playerConn => playerInfos[playerConn]).ToArray();
            SendMatchList();

            conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Joined, matchId = matchId, playerInfos = infos, myPlayerIndex = playerInfos[conn].playerIndex });

            foreach (NetworkConnectionToClient playerConn in matchConnections[matchId])
                playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.UpdateRoom, playerInfos = infos });
        }

        [ServerCallback]
        void OnServerLeaveMatch(NetworkConnectionToClient conn, Guid matchId)
        {
            MatchInfo matchInfo = openMatches[matchId];
            matchInfo.players--;
            openMatches[matchId] = matchInfo;

            PlayerInfo playerInfo = playerInfos[conn];
            playerInfo.ready = false;
            playerInfo.matchId = Guid.Empty;
            playerInfos[conn] = playerInfo;

            foreach (KeyValuePair<Guid, HashSet<NetworkConnectionToClient>> kvp in matchConnections)
                kvp.Value.Remove(conn);

            HashSet<NetworkConnectionToClient> connections = matchConnections[matchId];
            PlayerInfo[] infos = connections.Select(playerConn => playerInfos[playerConn]).ToArray();

            foreach (NetworkConnectionToClient playerConn in matchConnections[matchId])
                playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.UpdateRoom, playerInfos = infos, myPlayerIndex = playerInfos[conn].playerIndex });

            SendMatchList();

            conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Departed });
        }

        [ServerCallback]
        void OnServerPlayerReady(NetworkConnectionToClient conn, Guid matchId)
        {
            PlayerInfo playerInfo = playerInfos[conn];
            playerInfo.ready = !playerInfo.ready;
            playerInfos[conn] = playerInfo;

            HashSet<NetworkConnectionToClient> connections = matchConnections[matchId];
            PlayerInfo[] infos = connections.Select(playerConn => playerInfos[playerConn]).ToArray();

            foreach (NetworkConnectionToClient playerConn in matchConnections[matchId])
                playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.UpdateRoom, playerInfos = infos });
        }

        [ServerCallback]
        void OnServerCarUpdate(NetworkConnectionToClient conn, Guid matchId, int carIndex, int colourIndex, int accessoriesIndex)
        {
            PlayerInfo playerInfo = playerInfos[conn];
            if(carIndex >= 0) playerInfo.carID = carIndex; 
            if(colourIndex >= 0) playerInfo.colorIndex = colourIndex;
            if(accessoriesIndex >= 0) playerInfo.accessoriesIndex = accessoriesIndex;
            playerInfos[conn] = playerInfo;
            HashSet<NetworkConnectionToClient> connections = matchConnections[matchId];
            PlayerInfo[] infos = connections.Select(playerConn => playerInfos[playerConn]).ToArray();

            foreach (NetworkConnectionToClient playerConn in matchConnections[matchId])
                playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.UpdateCars, playerInfos = infos });
        }

        [ServerCallback]
        void OnServerStartMatch(NetworkConnectionToClient conn)
        {
            if (!playerMatches.ContainsKey(conn)) return;

            Guid matchId;
            int carIndex = 0;
            if (playerMatches.TryGetValue(conn, out matchId))
            {
                GameObject matchControllerObject = Instantiate(matchControllerPrefab);
                matchControllerObject.GetComponent<NetworkMatch>().matchId = matchId;
                NetworkServer.Spawn(matchControllerObject);

                if(players.Count > 0) players.Clear();
                
                MatchController matchController = matchControllerObject.GetComponent<MatchController>();

                HashSet<NetworkConnectionToClient> connections = matchConnections[matchId];
                PlayerInfo[] infos = connections.Select(playerConn => playerInfos[playerConn]).ToArray();
                    
                int playerPositionCount = 0;
                foreach (NetworkConnectionToClient playerConn in matchConnections[matchId])
                {
                    PlayerInfo playerInfo = playerInfos[playerConn];
                    playerConn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.Started, playerInfos = infos});
                    
                    GameObject player = Instantiate(NetworkManager.singleton.playerPrefab);
                    player.GetComponent<NetworkMatch>().matchId = matchId;
                    players.Add(player);
                    
                    // int playerInt = matchController.player1 == null ? 0 : 1; // Pierwszy gracz to 0, drugi to 1
                    // if (playerInt < startingPositions.Length)
                    // {
                    //     player.transform.position = startingPositions[playerInt];
                    //     player.transform.rotation = Quaternion.Euler(0,0,0);
                    // }
                    // else
                    // {
                    //     Debug.LogWarning("Brak wystarczającej liczby pozycji startowych dla graczy.");
                    // }
                    player.transform.position = startingPositions[playerPositionCount];
                    playerPositionCount++;
                    player.transform.rotation = Quaternion.Euler(0,0,0);
                    
                    NetworkServer.AddPlayerForConnection(playerConn, player);

                    PlayerCarSettings playerCarSettings = player.GetComponent<PlayerCarSettings>();
                    
                    playerCarSettings.carID = playerInfo.carID;
                    playerCarSettings.colorID = playerInfo.colorIndex;
                    playerCarSettings.accessoriesID = playerInfo.accessoriesIndex;
                    playerCarSettings.nickname = playerInfo.playerName;
                    playerCarSettings.CmdSetPlayerCar();
                    
                    // if (matchController.player1 == null)
                    // {
                    //     matchController.player1 = playerConn.identity;
                    // }
                    // else
                    // {
                    //     matchController.player2 = playerConn.identity;
                    // }
                    matchController.players.Add(playerConn.identity);
                    
                    //playerCarSettings.CmdSetPlayerCar(playerInfo.carID, playerInfo.colorIndex, playerInfo.accessoriesIndex, playerConn);
                    
                    /* Reset ready state for after the match. */
                    playerInfo.ready = false;
                    playerInfos[playerConn] = playerInfo;
                    carIndex++;
                }

                playerMatches.Remove(conn);
                openMatches.Remove(matchId);
                matchConnections.Remove(matchId);
                SendMatchList();

                OnPlayerDisconnected += matchController.OnPlayerDisconnected;
            }
        }
        
        /// <summary>
        /// Sends updated match list to all waiting connections or just one if specified
        /// </summary>
        /// <param name="conn"></param>
        [ServerCallback]
        internal void SendMatchList(NetworkConnectionToClient conn = null)
        {
            if (conn != null)
                conn.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.List, matchInfos = openMatches.Values.ToArray() });
            else
                foreach (NetworkConnectionToClient waiter in waitingConnections)
                    waiter.Send(new ClientMatchMessage { clientMatchOperation = ClientMatchOperation.List, matchInfos = openMatches.Values.ToArray() });
        }

        #endregion

        #region Client Match Message Handler

        [ClientCallback]
        void OnClientMatchMessage(ClientMatchMessage msg)
        {
            switch (msg.clientMatchOperation)
            {
                case ClientMatchOperation.None:
                    {
                        Debug.LogWarning("Missing ClientMatchOperation");
                        break;
                    }
                case ClientMatchOperation.List:
                    {
                        openMatches.Clear();
                        foreach (MatchInfo matchInfo in msg.matchInfos)
                        {
                            openMatches.Add(matchInfo.matchId, matchInfo);
                        }

                        RefreshMatchList();
                        break;
                    }
                case ClientMatchOperation.Created:
                    {
                        localPlayerMatch = msg.matchId;
                        ShowRoomView();
                        roomGUI.localPlayerIndex = msg.myPlayerIndex;
                        roomGUI.RefreshRoomPlayers(msg.playerInfos);
                        roomGUI.SetOwner(true);
                        break;
                    }
                case ClientMatchOperation.Cancelled:
                    {
                        localPlayerMatch = Guid.Empty;
                        ShowLobbyView();
                        break;
                    }
                case ClientMatchOperation.Joined:
                    {
                        localJoinedMatch = msg.matchId;
                        ShowRoomView();
                        roomGUI.localPlayerIndex = msg.myPlayerIndex;
                        roomGUI.RefreshRoomPlayers(msg.playerInfos);
                        roomGUI.SetOwner(false);
                        break;
                    }
                case ClientMatchOperation.Departed:
                    {
                        localJoinedMatch = Guid.Empty;
                        ShowLobbyView();
                        break;
                    }
                case ClientMatchOperation.UpdateRoom:
                    {
                        roomGUI.RefreshRoomPlayers(msg.playerInfos);
                        break;
                    }
                case ClientMatchOperation.UpdateCars:
                    {
                        roomGUI.RefreshPlayersCars(msg.playerInfos);
                        break;
                    }
                case ClientMatchOperation.Started:
                    {
                        //minimap.SetActive(true);
                        SavePlayersCars(msg.playerInfos);
                        lobbyView.SetActive(false);
                        roomView.SetActive(false);
                        break;
                    }
            }
        }

        [ClientCallback]
        void ShowLobbyView()
        {
            lobbyView.SetActive(true);
            roomView.SetActive(false);

            foreach (Transform child in matchList.transform)
                if (child.gameObject.GetComponent<MatchGUI>().GetMatchId() == selectedMatch)
                {
                    Toggle toggle = child.gameObject.GetComponent<Toggle>();
                    toggle.isOn = true;
                }
        }

        [ClientCallback]
        void ShowRoomView()
        {
            lobbyView.SetActive(false);
            roomView.SetActive(true);
        }

        [ClientCallback]
        void SavePlayersCars(PlayerInfo[] playerInfos)
        {
            for (int i = 0; i < players.Count; i++)
            {
                var obj = players[i];

                if (obj == null || obj.Equals(null))
                {
                    continue;
                }

                if (!obj.TryGetComponent<PlayerCarSettings>(out var settings))
                {
                    continue;
                }

                var info = playerInfos[i];
                settings.carID = info.carID;
                settings.colorID = info.colorIndex;
                settings.accessoriesID = info.accessoriesIndex;
                settings.nickname = info.playerName;
            }
        }

        [ClientCallback]
        void RefreshMatchList()
        {
            foreach (Transform child in matchList.transform)
                Destroy(child.gameObject);

            joinButton.interactable = false;
            
            List<MatchInfo> matchInfos = openMatches.OrderByDescending(pair => pair.Value.maxPlayers - pair.Value.players).Select(pair => pair.Value).ToList();
            
            foreach (MatchInfo matchInfo in matchInfos)
            {
                GameObject newMatch = Instantiate(matchPrefab, Vector3.zero, Quaternion.identity);
                newMatch.transform.SetParent(matchList.transform, false);
                newMatch.GetComponent<MatchGUI>().SetMatchInfo(matchInfo);

                Toggle toggle = newMatch.GetComponent<Toggle>();
                toggle.group = toggleGroup;
                if (matchInfo.matchId == selectedMatch)
                    toggle.isOn = true;
            }
            
        }

        #endregion
    }