using System;
using UnityEngine;

public class StartPoints : MonoBehaviour
{
    private void OnEnable()
    {
        StartPointsEvents.SetStartPoints += SetStartPositions;
    }

    private void OnDisable()
    {
        StartPointsEvents.SetStartPoints -= SetStartPositions;
    }

    private void SetStartPositions(GameObject[] cars)
    {
        for (int i = 0; i < cars.Length; i++)
        {
            cars[i].transform.position = transform.GetChild(i).position;
        }
    }

    public static class StartPointsEvents
    {
        public static Action<GameObject[]> SetStartPoints;
    }
}
