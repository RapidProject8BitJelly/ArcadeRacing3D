using System.Collections;
using System.Collections.Generic;
using Mirror.BouncyCastle.Crypto;
using UnityEngine;

public class Oil : MonoBehaviour
{
    [SerializeField] private float maxSpeedMultiplier;
    [SerializeField] private float maxSpeedLockDuration;
    private CarController carController;
    
    public void SlowDownPlayerCar(Collider player)
    {
        Debug.Log("Slowing down player car");
        carController = player.gameObject.GetComponent<CarController>();
        carController.maxSpeedMultiplier = maxSpeedMultiplier;
        StartCoroutine(WaitToRestoreMaxSpeed());
    }
    
    private IEnumerator WaitToRestoreMaxSpeed()
    {
        yield return new WaitForSeconds(maxSpeedLockDuration);
        carController.maxSpeedMultiplier = 1;
    }
}
