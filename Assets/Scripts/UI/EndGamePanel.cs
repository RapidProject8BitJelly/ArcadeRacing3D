using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TextMeshProUGUI rankingText;

    private List<string> _playersNames = new();
    private int _playersCount;

    private void OnEnable()
    {
        playAgainButton.onClick.AddListener(PlayAgain);
        backToMenuButton.onClick.AddListener(BackToMenu);
        EndGamePanelEvents.AddPlayerToRanking += AddPlayerToRanking;
        EndGamePanelEvents.SetPlayersCount += SetPlayersCount;
        EndGamePanelEvents.GetFinishedPlayersCount += GetFinishedPlayersCount;
    }

    private void OnDisable()
    {
        playAgainButton.onClick.RemoveAllListeners();
        backToMenuButton.onClick.RemoveAllListeners();
        EndGamePanelEvents.AddPlayerToRanking -= AddPlayerToRanking;
        EndGamePanelEvents.SetPlayersCount -= SetPlayersCount;
        EndGamePanelEvents.GetFinishedPlayersCount -= GetFinishedPlayersCount;
    }

    private void PlayAgain()
    {

    }

    private void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void ShowEndPanel()
    {
        endPanel.SetActive(true);
        string finishText = "";
        for(int i = 0; i< _playersNames.Count; i++)
        {
            finishText += (i + 1) + ". " + _playersNames[i];
            if(i < _playersNames.Count - 1 )
            {
                finishText += "\n";
            }
        }

        rankingText.text = finishText;
    }

    private void AddPlayerToRanking(string playerNickname)
    {
        _playersNames.Add(playerNickname);
        if(_playersNames.Count == _playersCount)
        {
            ShowEndPanel();
        }
    }

    private void SetPlayersCount(int count)
    {
        _playersCount = count; 
    }

    private int GetFinishedPlayersCount()
    {
        return _playersNames.Count; 
    }

    public static class EndGamePanelEvents
    {
        public static Action<string> AddPlayerToRanking;
        public static Action<int> SetPlayersCount;
        public static Func<int> GetFinishedPlayersCount;
    }
}
