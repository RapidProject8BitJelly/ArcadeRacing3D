using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ChooseCarPanel : MonoBehaviour
{
   [SerializeField] private GameObject carNode;
   [SerializeField] private Button nextCarButton;
   [SerializeField] private Button previousCarButton;
   [SerializeField] private CarCustomization carCustomization;
   [SerializeField] private PlayerGUI playerGUI;
   public int currentCar;
   public GameObject currentCarGameObject;
   public PlayerInfo playerInfo;

   private void Start()
   {
      playerInfo = playerGUI.player;
   }
   
   private void OnEnable()
   {
      nextCarButton.onClick.AddListener(() => ChooseCar(1));
      previousCarButton.onClick.AddListener(() => ChooseCar(-1));
   }

   [ClientCallback]
   public void ChooseCar(int value)
   {
      if(currentCar+value < 4 && currentCar+value >= 0) currentCar += value;
      else if (currentCar + value >= 4) currentCar = 0;
      else if (currentCar + value < 0) currentCar = 3;
      playerGUI.UpdatePlayerCar();
      
      for (int i = 0; i < carNode.transform.childCount; i++)
      {
         if (i == currentCar)
         {
            currentCarGameObject = carNode.transform.GetChild(i).gameObject;
            carNode.transform.GetChild(i).gameObject.SetActive(true);
            carCustomization.SetCurrentCar(currentCarGameObject);
         }
         else carNode.transform.GetChild(i).gameObject.SetActive(false);
      }
   }

   public void UpdateCarView(int value)
   {
      for (int i = 0; i < carNode.transform.childCount; i++)
      {
         if (i == value)
         {
            currentCarGameObject = carNode.transform.GetChild(i).gameObject;
            carNode.transform.GetChild(i).gameObject.SetActive(true);
         }
         else carNode.transform.GetChild(i).gameObject.SetActive(false);
      }
   }
}
