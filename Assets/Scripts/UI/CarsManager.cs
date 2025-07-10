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
        
        for (int i = 0; i < playerCars.Length; i++)
        {
            SceneManager.MoveGameObjectToScene(playerCars[i], currentScene);
        }
        
        StartPoints.StartPointsEvents.SetStartPoints(playerCars);
        playerCars = null;
    }
    
}
