using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class RoomGUI : MonoBehaviour
{
    public GameObject playerList;
    public GameObject playerPrefab;
    public GameObject cancelButton;
    //public GameObject leaveButton;
    public Button startButton;
    public bool owner;
    public int localPlayerIndex;
    private List<GameObject> chosenCars = new();
    
    [ClientCallback]
    public void RefreshRoomPlayers(PlayerInfo[] playerInfos)
    {
        foreach (Transform child in playerList.transform)
            Destroy(child.gameObject);

        startButton.interactable = false;
        bool everyoneReady = true;

        foreach (PlayerInfo playerInfo in playerInfos)
        {
            GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            newPlayer.transform.SetParent(playerList.transform, false);
            newPlayer.GetComponent<PlayerGUI>().SetPlayerInfo(playerInfo);

            if (!playerInfo.ready)
                everyoneReady = false;
        }
        startButton.interactable = everyoneReady && owner && (playerInfos.Length >= 1);
    }

    public void RefreshPlayersCars(PlayerInfo[] playerInfos)
    {
        for (int i = 0; i < playerInfos.Length; i++)
        {
            playerList.transform.GetChild(i).GetComponent<PlayerGUI>().SetPlayerCar(playerInfos[i]);
        }
    }

    [ClientCallback]
    public void SetOwner(bool owner)
    {
        this.owner = owner;
        cancelButton.SetActive(owner);
        //leaveButton.SetActive(!owner);
    }
}