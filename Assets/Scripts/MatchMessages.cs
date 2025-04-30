using System;
using Mirror;
using UnityEngine;

/// <summary>
/// Match message to be sent to the server
/// </summary>
public struct ServerMatchMessage : NetworkMessage
{
    public ServerMatchOperation serverMatchOperation;
    public Guid matchId;
    public int test;
    public int test2;
}

/// <summary>
/// Match message to be sent to the client
/// </summary>
public struct ClientMatchMessage : NetworkMessage
{
    public ClientMatchOperation clientMatchOperation;
    public Guid matchId;
    public MatchInfo[] matchInfos;
    public PlayerInfo[] playerInfos;
    public int myPlayerIndex;
}

public struct ReadyToMatchMessage : NetworkMessage
{
    public int playerIndex;
}

public struct CheckIfGoodPlayerMessage : NetworkMessage
{
    public int playerIndex;
    public int buttonIndex;
    public int value;
    public GameObject customizationObject;
}

public struct UpdatePlayerCarMessage : NetworkMessage
{
    public Guid matchId;
    public int carIndex;
    public int colourIndex;
    public int accessoriesIndex;
    public int rotationAngle;
}

public struct CreateMatchMessage : NetworkMessage
{
    public string matchName;
    public byte maxPlayers;
}

public struct SetPlayerNickname : NetworkMessage
{
    public string nickname;
}

/// <summary>
/// Information about a match
/// </summary>
[Serializable]
public struct MatchInfo
{
    public string matchName;
    public Guid matchId;
    public byte players;
    public byte maxPlayers;
}

/// <summary>
/// Information about a player
/// </summary>
[Serializable]
public struct PlayerInfo
{
    public string playerName;
    public int playerIndex;
    public bool ready;
    public Guid matchId;
    public int carID;
    public int colorIndex;
    public int accessoriesIndex;
    public int rotationAngle;
}

[Serializable]
public struct MatchPlayerData
{
    public int playerIndex;
    public int wins;
    public CarController carController;
}

/// <summary>
/// Match operation to execute on the server
/// </summary>
public enum ServerMatchOperation : byte
{
    None,
    Create,
    Cancel,
    Start,
    Join,
    Leave,
    Ready,
    UpdateCar
}

/// <summary>
/// Match operation to execute on the client
/// </summary>
public enum ClientMatchOperation : byte
{
    None,
    List,
    Created,
    Cancelled,
    Joined,
    Departed,
    UpdateRoom,
    UpdateCars,
    Started
}