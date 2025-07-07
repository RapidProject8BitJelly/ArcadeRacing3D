using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyCanvasController : MonoBehaviour
{
   [SerializeField] private Button startButton;
   [SerializeField] private Button exitToMenuButton;
   [SerializeField] private PlayerGUI[] players;
   
   private void OnEnable()
   {
      startButton.onClick.AddListener(StartGame);
      exitToMenuButton.onClick.AddListener(ExitToMenu);
      LobbyCanvasControllerEvents.SetStartButtonActive += SetStartButtonActive;
   }

   private void OnDisable()
   {
      startButton.onClick.RemoveAllListeners();
      exitToMenuButton.onClick.RemoveAllListeners();
      LobbyCanvasControllerEvents.SetStartButtonActive -= SetStartButtonActive;
   }

   private void StartGame()
   {
      SceneManager.LoadScene("GameTest");
   }

   private void ExitToMenu()
   {
      SceneManager.LoadScene("MainMenu");
   }

   private void SetStartButtonActive()
   {
      foreach (var player in players)
      {
         if (!player.isPlayerReady)
         {
            startButton.interactable = false;
            return;
         }
      }

      startButton.interactable = true;
   }

   public static class LobbyCanvasControllerEvents
   {
      public static Action SetStartButtonActive;
   }
}
