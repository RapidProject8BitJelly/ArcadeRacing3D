using UnityEngine;

public class GetCarElements : MonoBehaviour
{
    [SerializeField] private GameObject carBase;
    [SerializeField] private GameObject[] carWheels;

    public GameObject GetCarBase()
    {
        return carBase;
    }

    public GameObject[] GetCarWheels()
    {
        return carWheels;
    }
}
