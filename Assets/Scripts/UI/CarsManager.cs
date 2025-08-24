using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarsManager : MonoBehaviour
{
    private static CarsManager instance;
    private static bool gameStarted = false;
    public GameObject[] playerCars;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SavePlayersCars(GameObject[] cars)
    {
        playerCars = cars;
        gameStarted = true;
        StartCoroutine(ReattachToScene());
    }

    private IEnumerator ReattachToScene()
    {
        yield return null;
        
        Scene currentScene = SceneManager.GetActiveScene();
        
        StartPoints.StartPointsEvents.SetStartPoints(playerCars);
        CamerasManager.CamerasManagerEvents.SetPlayersCameras(playerCars);
        
        for (int i = 0; i < playerCars.Length; i++)
        {
            SceneManager.MoveGameObjectToScene(playerCars[i], currentScene);
            playerCars[i].GetComponent<TempPlayerInfo>().SetPlayerNumber(i);
            float playerRotation = 0;
            SetCarsElements();
            playerCars[i].GetComponent<CarCon>().enabled = true;
            playerCars[i].GetComponent<CarCon>().SetNewRotation(playerRotation);
            playerCars[i].GetComponent<Rigidbody>().isKinematic = false;
            playerCars[i].GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
            playerCars[i].transform.localScale = Vector3.one;
        }
        
        playerCars = null;
    }
    
    private void SetCarsElements()
    {
        for(int i = 0; i < playerCars.Length;i++)
        {
            CarType playerCarType = playerCars[i].GetComponent<CarType>();
            
            for (int j = 0; j < playerCarType.carAccessories.transform.childCount; j++)
            {
                GameObject carModel = playerCarType.carAccessories.transform.GetChild(j).gameObject;
                if(carModel.activeSelf == false)
                {
                    Destroy(carModel);
                }
                else
                {
                    GetCarElements getCarElements;
                    getCarElements = carModel.GetComponent<GetCarElements>();
                    playerCarType.carBase = getCarElements.GetCarBase();
                    playerCarType.wheels = getCarElements.GetCarWheels();
                }
            }
            
        }
    }

}
