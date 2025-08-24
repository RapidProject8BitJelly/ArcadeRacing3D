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

    public GameObject currentCarAccessories;
    public GameObject currentCar;
    
    private GameObject[] elementsToChangeColor;
    public CarColoursManager colors;
    public CarVariantColoursManager currentCarVariantColours;
    public CarColours[] carColours;

    private int currentColorIndex;
    private int currentAccessoriesIndex;
    
    #endregion

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
    }

    private void OnDisable()
    { 
        nextColorButton.onClick.RemoveAllListeners();
        previousColorButton.onClick.RemoveAllListeners();
        nextAccessoriesButton.onClick.RemoveAllListeners();
        previousAccessoriesButton.onClick.RemoveAllListeners();
    }

    #region Colour
    
    private void ChooseColor(int value)
    {
        if (currentColorIndex + value < carColours.Length && currentColorIndex + value >= 0) currentColorIndex += value;
        else if (currentColorIndex + value >= carColours.Length) currentColorIndex = 0;
        else if(currentColorIndex + value < 0) currentColorIndex = carColours.Length - 1;
    
        ChangeColor();
    }
    
    private void ChangeColor()
    {
        colorImage.color = carColours[currentColorIndex].GetIconColor();

        currentCarVariantColours.ChangeCurrentColourSet(currentColorIndex);
    }
    
    #endregion

    #region Accessories
    
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
        currentCarVariantColours = colors.ChangeCurrentCarVariant(currentAccessoriesIndex);
        carColours = currentCarVariantColours.GetColoursSet();
        colorImage.color = carColours[0].GetIconColor();
        accessoriesText.text = (currentAccessoriesIndex+1).ToString();
        for (int i = 0; i < currentCarAccessories.transform.childCount; i++)
        {
            currentCarAccessories.transform.GetChild(i).gameObject.SetActive(i == currentAccessoriesIndex);
        }
    }

    #endregion
    
    #region CarUpdate
    
    public void SetCurrentCar(GameObject car)
    {
        currentCar = car;
        currentCarAccessories = car.GetComponent<CarType>().GetCarAccessories();
        colors = car.GetComponent<CarType>().coloursManager;
        elementsToChangeColor = car.GetComponent<CarType>().GetElementsToChangeColor();
        ChooseAccessories(-currentAccessoriesIndex);
        ChooseColor(-currentColorIndex);
    }

    #endregion
}
