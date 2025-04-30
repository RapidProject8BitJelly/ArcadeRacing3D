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
   [SerializeField] private Button rotateButton;
   [SerializeField] private CarCustomization carCustomization;
   [SerializeField] private PlayerGUI playerGUI;
   [SerializeField] private int rotationAngle;
   public int currentCar;
   public GameObject currentCarGameObject;
   public PlayerInfo playerInfo;
   public int currentRotation = 90;

   private void Start()
   {
      playerInfo = playerGUI.player;
   }
   
   private void OnEnable()
   {
      nextCarButton.onClick.AddListener(() => ChooseCar(1));
      nextCarButton.onClick.AddListener(CheckIfGoodPlayer);
      previousCarButton.onClick.AddListener(() => ChooseCar(-1));
      previousCarButton.onClick.AddListener(CheckIfGoodPlayer);
      rotateButton.onClick.AddListener(RotateCar);
      rotateButton.onClick.AddListener(CheckIfGoodPlayer);
   }

   [ClientCallback]
   public void ChooseCar(int value)
   {
      if(currentCar+value < 4 && currentCar+value >= 0) currentCar += value;
      else if (currentCar + value >= 4) currentCar = 0;
      else if (currentCar + value < 0) currentCar = 3;
      
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

   public void UpdateCarView(int value, int rotationAngleValue)
   {
      for (int i = 0; i < carNode.transform.childCount; i++)
      {
         if (i == value)
         {
            currentCarGameObject = carNode.transform.GetChild(i).gameObject;
            carNode.transform.GetChild(i).gameObject.SetActive(true);
            carCustomization.currentCar = currentCarGameObject;
            carCustomization.currentCarAccessories = currentCarGameObject.transform
               .GetChild(currentCarGameObject.transform.childCount - 1).gameObject;
            currentCarGameObject.transform.rotation = Quaternion.Euler(0, rotationAngleValue, 0);
         }
         else carNode.transform.GetChild(i).gameObject.SetActive(false);
      }
   }

   [ClientCallback]
   public void RotateCar()
   {
      if (currentRotation + rotationAngle <= 360) currentRotation += rotationAngle;
      else currentRotation = 0;
      
      currentCarGameObject.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
   }
   
   private void CheckIfGoodPlayer()
   {
      FindObjectOfType<CanvasController>().RequestCarCustomization(currentCar, 0, 0, currentRotation);
   }
}
