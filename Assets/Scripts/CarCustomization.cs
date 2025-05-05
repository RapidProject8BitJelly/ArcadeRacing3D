using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CarCustomization : MonoBehaviour
{
    [SerializeField] private Image colorImage;
    
    [SerializeField] private Button nextColorButton;
    [SerializeField] private Button previousColorButton;
    [SerializeField] private GameObject carNode;
    
    [SerializeField] private TextMeshProUGUI accessoriesText;
    [SerializeField] private Button nextAccessoriesButton;
    [SerializeField] private Button previousAccessoriesButton;
    
    [SerializeField] private PlayerGUI playerGUI;

    public int currentColorIndex;
    public int currentAccessoriesIndex;
    public GameObject currentCarAccessories;
    public GameObject currentCar;
    public bool isGoodPlayer;

    private GameObject[] elementsToChangeColor;
    private Color[] colors;

    private void Awake()
    {
        colors = carNode.transform.GetChild(0).GetComponent<CarType>().GetCarParameters().CarColors;
    }

    private void OnEnable()
    {
        nextColorButton.onClick.AddListener(() => ChooseColor(1));
        nextColorButton.onClick.AddListener(() => CheckIfGoodPlayer(1, 0));
        previousColorButton.onClick.AddListener(() => ChooseColor(-1));
        previousColorButton.onClick.AddListener(() => CheckIfGoodPlayer(-1, 0));
        nextAccessoriesButton.onClick.AddListener(() => ChooseAccessories(1));
        nextAccessoriesButton.onClick.AddListener(() => CheckIfGoodPlayer(1, 1));
        previousAccessoriesButton.onClick.AddListener(() => ChooseAccessories(-1));
        previousAccessoriesButton.onClick.AddListener(() => CheckIfGoodPlayer(-1, 1));
    }

    [ClientCallback]
    public void ChooseColor(int value)
    {
        if (currentColorIndex + value < colors.Length && currentColorIndex + value >= 0) currentColorIndex += value;
        else if (currentColorIndex + value >= colors.Length) currentColorIndex = 0;
        else if(currentColorIndex + value < 0) currentColorIndex = colors.Length - 1;

        colorImage.color = colors[currentColorIndex];

        for (int i = 0; i < elementsToChangeColor.Length; i++)
        {
            elementsToChangeColor[i].GetComponent<MeshRenderer>().material.color = colors[currentColorIndex];
        }
    }

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
    
    [ClientCallback]
    public void ChooseAccessories(int value)
    {
        if (currentAccessoriesIndex + value < currentCarAccessories.transform.childCount && currentAccessoriesIndex + value >= 0) 
            currentAccessoriesIndex += value;
        else if(currentAccessoriesIndex + value >= currentCarAccessories.transform.childCount) currentAccessoriesIndex = 0;
        else if (currentAccessoriesIndex + value < 0) currentAccessoriesIndex = 2;
        
        accessoriesText.text = (currentAccessoriesIndex+1).ToString();
        for (int i = 0; i < currentCarAccessories.transform.childCount; i++)
        {
            if (i == currentAccessoriesIndex)
            {
                currentCarAccessories.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                currentCarAccessories.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void UpdateCarView(int colourIndex, int accessoriesIndex)
    {
        colors = currentCar.GetComponent<CarType>().GetCarParameters().CarColors;
        colorImage.color = colors[colourIndex];
        currentCar.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = colors[colourIndex];
        
        accessoriesText.text = (accessoriesIndex+1).ToString();
        for (int i = 0; i < currentCarAccessories.transform.childCount; i++)
        {
            if (i == accessoriesIndex)
            {
                currentCarAccessories.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                currentCarAccessories.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private void CheckIfGoodPlayer(int value, int buttonIndex)
    {
        FindObjectOfType<CanvasController>().RequestCarCustomization(-1, currentColorIndex, currentAccessoriesIndex);
    }
}
