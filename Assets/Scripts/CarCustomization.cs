using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CarCustomization : MonoBehaviour
{
    [SerializeField] private Image colorImage;
    [SerializeField] private Color[] colors;
    [SerializeField] private Button nextColorButton;
    [SerializeField] private Button previousColorButton;
    [SerializeField] private GameObject carNode;
    
    [SerializeField] private TextMeshProUGUI accessoriesText;
    [SerializeField] private Button nextAccessoriesButton;
    [SerializeField] private Button previousAccessoriesButton;
    
    private int currentColorIndex;
    private int currentAccessoriesIndex;
    public GameObject currentCarAccessories;
    public GameObject currentCar;

    private void OnEnable()
    {
        nextColorButton.onClick.AddListener(() => ChooseColor(1));
        previousColorButton.onClick.AddListener(() => ChooseColor(-1));
        nextAccessoriesButton.onClick.AddListener(() => ChooseAccessories(1));
        previousAccessoriesButton.onClick.AddListener(() => ChooseAccessories(-1));
    }

    private void ChooseColor(int value)
    {
        if (currentColorIndex + value < colors.Length && currentColorIndex + value >= 0) currentColorIndex += value;
        else if (currentColorIndex + value >= colors.Length) currentColorIndex = 0;
        else if(currentColorIndex + value < 0) currentColorIndex = colors.Length - 1;

        colorImage.color = colors[currentColorIndex];
        currentCar.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = colors[currentColorIndex];
    }

    public void SetCurrentCar(GameObject car)
    {
        currentCar = car;
        currentCarAccessories = currentCar.transform.GetChild(currentCar.transform.childCount - 1).gameObject;
        ChooseColor(-currentColorIndex);
        ChooseAccessories(-currentAccessoriesIndex);
    }

    private void ChooseAccessories(int value)
    {
        if (currentAccessoriesIndex + value < currentCarAccessories.transform.childCount && currentAccessoriesIndex + value >= 0) 
            currentAccessoriesIndex += value;
        else if(currentAccessoriesIndex + value >= currentCarAccessories.transform.childCount) currentAccessoriesIndex = 0;
        else if (currentAccessoriesIndex + value < 0) currentAccessoriesIndex = 3;
        
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
}
