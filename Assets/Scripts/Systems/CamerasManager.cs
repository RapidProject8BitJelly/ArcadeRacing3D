using System;
using Cinemachine;
using UnityEngine;

public class CamerasManager : MonoBehaviour
{
    [SerializeField] private GameObject[] cameras;
    [SerializeField] private CinemachineVirtualCamera[] virtualCameras;

    private void OnEnable()
    {
        CamerasManagerEvents.SetPlayersCameras += SetPlayersCameras;
    }

    private void OnDisable()
    {
        CamerasManagerEvents.SetPlayersCameras -= SetPlayersCameras;
    }

    private void SetPlayersCameras(GameObject[] player)
    {
        for (int i = 0; i < player.Length; i++)
        {
            cameras[i].transform.SetParent(player[i].transform);
            player[i].GetComponent<CarCon>().virtualCamera = virtualCameras[i];
            player[i].GetComponent<CarCon>().SetupPlayerCamera();
            if (i == 1)
            {
                DevModeManager.DevModeManagerEvents.SetPlayerToDisable(player[i]);
            }
        }
    }

    public static class CamerasManagerEvents
    {
        public static Action<GameObject[]> SetPlayersCameras;
    }
}
