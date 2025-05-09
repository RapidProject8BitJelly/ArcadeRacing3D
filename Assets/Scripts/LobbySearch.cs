using System;
using TMPro;
using UnityEngine;

public class LobbySearch : MonoBehaviour
{
    [SerializeField] private GameObject lobbyList;
    [SerializeField] private TMP_InputField searchInput;
    
    private void OnEnable()
    {
        searchInput.onValueChanged.AddListener(SearchLobby);
    }

    private void SearchLobby(string value)
    {
        for (int i = 0; i < lobbyList.transform.childCount; i++)
        {
            string lobbyName = lobbyList.transform.GetChild(i).GetComponent<MatchGUI>().matchName.text;
            
            if (lobbyName.StartsWith(value, StringComparison.OrdinalIgnoreCase))
            {
                lobbyList.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                lobbyList.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}