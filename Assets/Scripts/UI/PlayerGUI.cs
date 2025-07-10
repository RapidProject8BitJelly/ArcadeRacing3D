using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour
{
    [SerializeField] private Button readyButton;
    [SerializeField] private GameObject blockCarChoosePanel;
    [SerializeField] private GameObject cars;
    
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

    public GameObject GetSelectedCar()
    {
        for (int i = 0; i < cars.transform.childCount; i++)
        {
            if(cars.transform.GetChild(i).gameObject.activeSelf) return cars.transform.GetChild(i).gameObject;
        }
        return null;
    }
}