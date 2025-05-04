using UnityEngine;

public class CarType : MonoBehaviour
{
    [SerializeField] private CarParameters carParameters;
    [SerializeField] private GameObject[] elementsToChangeColor;
    [SerializeField] private GameObject carAccessories;

    public CarParameters GetCarParameters()
    {
        return carParameters;
    }
    
    public GameObject[] GetElementsToChangeColor()
    {
        return elementsToChangeColor;
    }

    public GameObject GetCarAccessories()
    {
        return carAccessories;
    }
    
    public void UseSpecialAbility()
    {
        carParameters.SpecialAbility?.ActivateAbility();
    }
}
