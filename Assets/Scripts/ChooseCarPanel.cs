using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class ChooseCarPanel : MonoBehaviour
{
   #region Variables

   [SerializeField] private GameObject carNode;
   [SerializeField] private Button nextCarButton;
   [SerializeField] private Button previousCarButton;
   [SerializeField] private Button rotateButton;
   [SerializeField] private CarCustomization carCustomization;
   [SerializeField] private PlayerGUI playerGUI;
   [SerializeField] private SetCarInfo setCarInfo;
   
   [SerializeField] private int rotationAngle;
   
   private GameObject currentCar;
   private CanvasController canvasController;
   private int currentCarIndex;
   private int currentRotation = 90;

   #endregion
   
   private void Awake()
   {
      ChooseCar(0);
      canvasController = FindObjectOfType<CanvasController>();
   }
   
   private void OnEnable()
   {
      nextCarButton.onClick.AddListener(() => ChooseCar(1));
      nextCarButton.onClick.AddListener(RequestCarCustomization);
      previousCarButton.onClick.AddListener(() => ChooseCar(-1));
      previousCarButton.onClick.AddListener(RequestCarCustomization);
      rotateButton.onClick.AddListener(RotateCar);
   }

   private void OnDisable()
   {
      nextCarButton.onClick.RemoveAllListeners();
      previousCarButton.onClick.RemoveAllListeners();
      rotateButton.onClick.RemoveAllListeners();
   }

   #region  Car Selection

   [ClientCallback]
   private void ChooseCar(int value)
   {
      if(currentCarIndex+value < 4 && currentCarIndex+value >= 0) currentCarIndex += value;
      else if (currentCarIndex + value >= 4) currentCarIndex = 0;
      else if (currentCarIndex + value < 0) currentCarIndex = 3;
      
      SetCarRef();
      carCustomization.SetCurrentCar(currentCar);
   }

   public void UpdateCarView(int value)
   {
      currentCarIndex = value;
      SetCarRef();
      carCustomization.currentCarAccessories = currentCar.GetComponent<CarType>().GetCarAccessories();
      carCustomization.currentCar = currentCar;
   }

   private void SetCarRef()
   {
      for (int i = 0; i < carNode.transform.childCount; i++)
      {
         if (i == currentCarIndex)
         {
            currentCar = carNode.transform.GetChild(i).gameObject;
            carNode.transform.GetChild(i).gameObject.SetActive(true);
         }
         else carNode.transform.GetChild(i).gameObject.SetActive(false);
      }
      
      setCarInfo.UpdateCarInfo(currentCar.GetComponent<CarType>().GetCarParameters());
   }
   
   private void RequestCarCustomization()
   {
      canvasController.RequestCarCustomization(currentCarIndex, 0, 0);
   }

   #endregion
   
   [ClientCallback]
   private void RotateCar()
   {
      if (currentRotation + rotationAngle <= 360) currentRotation += rotationAngle;
      else currentRotation = 0;
      
      currentCar.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
   }
}
