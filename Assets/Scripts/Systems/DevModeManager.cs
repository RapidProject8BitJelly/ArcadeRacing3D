using System;
using UnityEngine;

public class DevModeManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    
    public bool debugMode = false;
    private GameObject playerToDisable;

    private void OnEnable()
    {
        DevModeManagerEvents.GetDevModeState += GetDevModeState;
        DevModeManagerEvents.SetPlayerToDisable += SetPlayerToDisable;
    }

    private void OnDisable()
    {
        DevModeManagerEvents.GetDevModeState -= GetDevModeState;
        DevModeManagerEvents.SetPlayerToDisable -= SetPlayerToDisable;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            debugMode = !debugMode;
            if(debugMode) ActivateDevMode();
            else DisableDevMode();
        }
    }

    private bool GetDevModeState()
    {
        return debugMode;
    }

    private void ActivateDevMode()
    {
        playerToDisable.SetActive(false);
        mainCamera.rect = new Rect(0f, 0f, 1f, 1f);
    }

    private void DisableDevMode()
    {
        playerToDisable.SetActive(true);
        mainCamera.rect = new Rect(0f, 0f, 0.5f, 1f);
    }

    private void SetPlayerToDisable(GameObject player)
    {
        playerToDisable = player;
    }
    
    public static class DevModeManagerEvents
    {
        public static Func<bool> GetDevModeState;
        public static Action<GameObject> SetPlayerToDisable;
    }
}
