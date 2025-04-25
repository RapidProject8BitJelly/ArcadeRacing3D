using System;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour
{
    [SerializeField] private Button readyButton;
    
    public TextMeshProUGUI playerName;
    
    
    [ClientCallback]
    public void SetPlayerInfo(PlayerInfo info)
    {
        playerName.text = info.playerName;
        playerName.color = info.ready ? Color.green : Color.red;
    }
}