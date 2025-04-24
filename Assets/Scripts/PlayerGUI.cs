using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour
{
    public Text playerName;

    [ClientCallback]
    public void SetPlayerInfo(PlayerInfo info)
    {
        playerName.text = info.playerName;
        playerName.color = info.ready ? Color.green : Color.red;
    }
}