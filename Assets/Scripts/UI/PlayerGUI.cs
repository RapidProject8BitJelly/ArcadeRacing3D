using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour
{
    [SerializeField] private Button readyButton;
    [SerializeField] private GameObject blockCarChoosePanel;
    
    public TextMeshProUGUI playerNameTMP;
    public bool isPlayerReady = false;
    public string playerName;

    private void OnEnable()
    {
        readyButton.onClick.AddListener(RequestReadyChange);
        playerNameTMP.color = isPlayerReady ? Color.green : Color.red;
    }
    
    public void SetPlayerInfo(PlayerInfo info)
    {
    }

    public void RequestReadyChange()
    {
        isPlayerReady = !isPlayerReady;
        LobbyCanvasController.LobbyCanvasControllerEvents.SetStartButtonActive();
        playerNameTMP.color = isPlayerReady ? Color.green : Color.red;
        blockCarChoosePanel.SetActive(isPlayerReady);
    }

    public void SetPlayerName(string pName)
    {
        playerName = pName;
        playerNameTMP.text = playerName;
    }
}