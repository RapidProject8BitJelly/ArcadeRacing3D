using UnityEngine;

public enum PlayerNumbers
{
    Player1 = 0,
    Player2 = 1
}

public class TempPlayerInfo : MonoBehaviour
{
    public PlayerNumbers PlayerNumber;

    public void SetPlayerNumber(int playerNumber)
    {
        PlayerNumber = (PlayerNumbers)playerNumber;
    }

    public PlayerNumbers GetPlayerNumber()
    {
        return PlayerNumber;
    }
}
