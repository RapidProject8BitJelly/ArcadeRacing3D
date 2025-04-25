using System;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour
{
    [SerializeField] private Button readyButton;
    
    public TextMeshProUGUI playerName;
    public PlayerInfo player;

    private void OnEnable()
    {
        readyButton.onClick.AddListener(ClickReadyButton);
    }
    
    [ClientCallback]
    public void SetPlayerInfo(PlayerInfo info)
    {
        playerName.text = info.playerName;
        playerName.color = info.ready ? Color.green : Color.red;
        player = info;
    }

    private void ClickReadyButton()
    {
        NetworkClient.Send(new ReadyToMatchMessage
        {
            playerIndex = player.playerIndex
        });
    }
}