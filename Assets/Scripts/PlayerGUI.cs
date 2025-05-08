using System;
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
    [SerializeField] private GameObject cars;
    
    public TextMeshProUGUI playerName;
    public PlayerInfo player;
    public CanvasController canvasController;

    private void OnEnable()
    {
        canvasController = FindObjectOfType<CanvasController>();
        readyButton.onClick.AddListener(ClickReadyButton);
        //readyButton.onClick.AddListener(canvasController.RequestReadyChange);
    }
    
    [ClientCallback]
    public void SetPlayerInfo(PlayerInfo info)
    {
        playerName.text = info.playerName;
        playerName.color = info.ready ? Color.green : Color.red;
        player = info;
        blockPanel.SetActive(info.playerIndex != FindObjectOfType<RoomGUI>().localPlayerIndex);
        SetPlayerCar(info);
    }

    private void ClickReadyButton()
    {
        canvasController.RequestReadyChange(player.playerIndex);
    }

    public GameObject GetChosenCar()
    {
        for (int i = 0; i < cars.transform.childCount; i++)
        {
            if (cars.transform.GetChild(i).gameObject.activeSelf)
            {
                return cars.transform.GetChild(i).gameObject;
                //cars.transform.GetChild(i).gameObject.transform.SetParent(FindObjectOfType<CustomNetworkManager>().playerPrefab.transform);
                break;
            }
        }
        return null;
    }
    
    public void SetPlayerCar(PlayerInfo info)
    {
        chooseCarPanel.UpdateCarView(info.carID, info.rotationAngle);
        carCustomization.UpdateCarView(info.colorIndex, info.accessoriesIndex);
    }
}