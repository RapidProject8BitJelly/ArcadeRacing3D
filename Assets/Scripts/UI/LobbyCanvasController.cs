using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyCanvasController : MonoBehaviour
{
   [SerializeField] private Button startButton;
   [SerializeField] private Button exitToMenuButton;
   [SerializeField] private PlayerGUI[] players;
   
   private GameObject[] playersCars = new GameObject[2];
   private CarsManager carsManager;

   private void Awake()
   {
      carsManager = FindFirstObjectByType<CarsManager>();
   }
   
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
      for (int i = 0; i < playersCars.Length; i++)
      {
         playersCars[i].transform.SetParent(null);
         DontDestroyOnLoad(playersCars[i]);
      }
      carsManager.SavePlayersCars(playersCars);
      SceneManager.LoadScene("GameTest");
   }

   private void ExitToMenu()
   {
      SceneManager.LoadScene("MainMenu");
   }

   private void SetStartButtonActive()
   {
      for (int i = 0; i < players.Length; i++)
      {
         if (!players[i].isPlayerReady)
         {
            startButton.interactable = false;
            return;
         }
         playersCars[i] = players[i].GetSelectedCar();
      }

      startButton.interactable = true;
   }

   public static class LobbyCanvasControllerEvents
   {
      public static Action SetStartButtonActive;
   }
}
