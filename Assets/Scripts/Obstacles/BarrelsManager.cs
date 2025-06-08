using UnityEngine;

public class BarrelsManager : MonoBehaviour
{
    private int barrelID = 0;

    private void Awake()
    {
        SetBarrelsID();
    }

    private void SetBarrelsID()
    {
        foreach (Transform child in transform)
        {
            child.GetComponentInChildren<Barrel>().barrelID = "Barrel" + barrelID;
            barrelID++;
        }
    }
}
