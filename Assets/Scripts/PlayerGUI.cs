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
        //chooseCarPanel.ChooseCar(info.carID);
        //carCustomization.ChooseColor(info.colorIndex);
        //carCustomization.ChooseAccessories(info.accessoriesIndex);
    }

    private void ClickReadyButton()
    {
        NetworkClient.Send(new ReadyToMatchMessage
        {
            playerIndex = player.playerIndex
        });
    }

    public void SetPlayerCar(PlayerInfo info)
    {
        chooseCarPanel.UpdateCarView(info.carID);
        carCustomization.UpdateCarView(info.colorIndex, info.accessoriesIndex);
    }

    public void UpdatePlayerCar()
    {
        NetworkClient.Send(new UpdatePlayerCarChoiceMessage
        {
            carIndex = chooseCarPanel.currentCar,
            colourIndex = carCustomization.currentColorIndex,
            accessoriesIndex = carCustomization.currentAccessoriesIndex,
            playerIndex = player.playerIndex
        });
    }
}