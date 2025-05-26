using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelsManager : MonoBehaviour
{
    private int barrelID = 0;

    private void Start()
    {
        SetBarrelsID();
    }

    private void SetBarrelsID()
    {
        foreach (Transform child in transform)
        {
            child.GetComponentInChildren<Barrel>().barrelID = "Barrel" + barrelID.ToString();
            barrelID++;
        }
    }
}
