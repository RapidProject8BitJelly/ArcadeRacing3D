using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour
{
    [SerializeField] private Button readyButton;
    [SerializeField] private ChooseCarPanel chooseCarPanel;
    [SerializeField] private CarCustomization carCustomization;
    [SerializeField] private GameObject blockPanel;
    [SerializeField] private GameObject blockCarChoosePanel;
    [SerializeField] private GameObject cars;
    
    public TextMeshProUGUI playerName;
    public PlayerInfo player;
    public CanvasController canvasController;

    private void OnEnable()
    {
        canvasController = FindObjectOfType<CanvasController>();
        readyButton.onClick.AddListener(canvasController.RequestReadyChange);
    }
    
    [ClientCallback]
    public void SetPlayerInfo(PlayerInfo info)
    {
        playerName.text = info.playerName;
        playerName.color = info.ready ? Color.green : Color.red;
        player = info;
        blockPanel.SetActive(info.playerIndex != FindObjectOfType<RoomGUI>().localPlayerIndex);
        blockCarChoosePanel.SetActive(info.ready);
        SetPlayerCar(info);
    }
    
    public void SetPlayerCar(PlayerInfo info)
    {
        chooseCarPanel.UpdateCarView(info.carID);
        carCustomization.UpdateCarView(info.colorIndex, info.accessoriesIndex);
    }
}