using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseCarPanel : MonoBehaviour
{
   [SerializeField] private GameObject carNode;
   [SerializeField] private Button nextCarButton;
   [SerializeField] private Button previousCarButton;
   [SerializeField] private CarCustomization carCustomization;
   private int _currentCar;
   public GameObject currentCarGameObject;
   
   private void OnEnable()
   {
      nextCarButton.onClick.AddListener(() => ChooseCar(1));
      previousCarButton.onClick.AddListener(() => ChooseCar(-1));
   }

   private void ChooseCar(int value)
   {
      if(_currentCar+value < 4 && _currentCar+value >= 0) _currentCar += value;
      else if (_currentCar + value >= 4) _currentCar = 0;
      else if (_currentCar + value < 0) _currentCar = 3;
      
      for (int i = 0; i < carNode.transform.childCount; i++)
      {
         if (i == _currentCar)
         {
            currentCarGameObject = carNode.transform.GetChild(i).gameObject;
            carNode.transform.GetChild(i).gameObject.SetActive(true);
            carCustomization.SetCurrentCar(currentCarGameObject);
         }
         else carNode.transform.GetChild(i).gameObject.SetActive(false);
      }
   }
}
