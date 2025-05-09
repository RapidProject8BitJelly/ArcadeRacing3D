using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CarCustomization : MonoBehaviour
{
    #region Variables

    [SerializeField] private Image colorImage;
    [SerializeField] private GameObject carNode;
    [SerializeField] private TextMeshProUGUI accessoriesText;
    [SerializeField] private Button nextColorButton;
    [SerializeField] private Button previousColorButton;
    [SerializeField] private Button nextAccessoriesButton;
    [SerializeField] private Button previousAccessoriesButton;
    
    [SerializeField] private PlayerGUI playerGUI;

    public GameObject currentCarAccessories;
    public GameObject currentCar;
    
    private GameObject[] elementsToChangeColor;
    public Color[] colors;
    
    private CanvasController canvasController;
    
    private int currentColorIndex;
    private int currentAccessoriesIndex;
    
    #endregion
    private void Awake()
    {
        colors = carNode.transform.GetChild(0).GetComponent<CarType>().GetCarParameters().CarColors;
        canvasController = FindObjectOfType<CanvasController>();
    }

    private void OnEnable()
    {
        AddButtonCallbacks(nextColorButton, () => ChooseColor(1));
        AddButtonCallbacks(previousColorButton, () => ChooseColor(-1));
        AddButtonCallbacks(nextAccessoriesButton, () => ChooseAccessories(1));
        AddButtonCallbacks(previousAccessoriesButton, () => ChooseAccessories(-1));
    }
    
    private void AddButtonCallbacks(Button button, UnityAction action)
    {
        button.onClick.AddListener(action);
        button.onClick.AddListener(RequestCarCustomization);
    }

    private void OnDisable()
    { 
        nextColorButton.onClick.RemoveAllListeners();
        previousColorButton.onClick.RemoveAllListeners();
        nextAccessoriesButton.onClick.RemoveAllListeners();
        previousAccessoriesButton.onClick.RemoveAllListeners();
    }

    #region Colour
    
    [ClientCallback]
    private void ChooseColor(int value)
    {
        if (currentColorIndex + value < colors.Length && currentColorIndex + value >= 0) currentColorIndex += value;
        else if (currentColorIndex + value >= colors.Length) currentColorIndex = 0;
        else if(currentColorIndex + value < 0) currentColorIndex = colors.Length - 1;
    
        ChangeColor();
    }
    
    private void ChangeColor()
    {
        colorImage.color = colors[currentColorIndex];

        foreach (var obj in elementsToChangeColor)
        {
            obj.GetComponent<MeshRenderer>().material.color = colors[currentColorIndex];
        }
    }
    
    #endregion

    #region Accessories

    [ClientCallback]
    private void ChooseAccessories(int value)
    {
        if (currentAccessoriesIndex + value < currentCarAccessories.transform.childCount && currentAccessoriesIndex + value >= 0) 
            currentAccessoriesIndex += value;
        else if(currentAccessoriesIndex + value >= currentCarAccessories.transform.childCount) currentAccessoriesIndex = 0;
        else if (currentAccessoriesIndex + value < 0) currentAccessoriesIndex = currentCarAccessories.transform.childCount-1;
        
        ChangeAccessories();
    }
    
    private void ChangeAccessories()
    {
        accessoriesText.text = (currentAccessoriesIndex+1).ToString();
        for (int i = 0; i < currentCarAccessories.transform.childCount; i++)
        {
            currentCarAccessories.transform.GetChild(i).gameObject.SetActive(i == currentAccessoriesIndex);
        }
    }

    #endregion
    
    #region CarUpdate
    
    [ClientCallback]
    public void SetCurrentCar(GameObject car)
    {
        currentCar = car;
        currentCarAccessories = car.GetComponent<CarType>().GetCarAccessories();
        colors = car.GetComponent<CarType>().GetCarParameters().CarColors;
        elementsToChangeColor = car.GetComponent<CarType>().GetElementsToChangeColor();
        ChooseColor(-currentColorIndex);
        ChooseAccessories(-currentAccessoriesIndex);
    }
    
    public void UpdateCarView(int colourIndex, int accessoriesIndex)
    {
        currentColorIndex = colourIndex;
        currentAccessoriesIndex = accessoriesIndex;
        colors = currentCar.GetComponent<CarType>().GetCarParameters().CarColors;
        elementsToChangeColor = currentCar.GetComponent<CarType>().GetElementsToChangeColor();
        colorImage.color = colors[colourIndex];
        
        ChangeColor();
        ChangeAccessories();
    }
    
    private void RequestCarCustomization()
    {
        canvasController.RequestCarCustomization(-1, currentColorIndex, currentAccessoriesIndex);
    }

    #endregion
}
