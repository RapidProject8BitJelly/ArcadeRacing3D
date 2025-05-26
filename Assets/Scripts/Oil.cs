using System.Collections;
using System.Collections.Generic;
using Mirror.BouncyCastle.Crypto;
using UnityEngine;

public class Oil : MonoBehaviour
{
    [SerializeField] private float maxSpeedMultiplier;
    [SerializeField] private float maxSpeedLockDuration;
    private CarController carController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            carController = other.gameObject.GetComponent<CarController>();
            carController.maxSpeedMultiplier = maxSpeedMultiplier;
            StartCoroutine(WaitToRestoreMaxSpeed());
        }
    }

    private IEnumerator WaitToRestoreMaxSpeed()
    {
        yield return new WaitForSeconds(maxSpeedLockDuration);
        carController.maxSpeedMultiplier = 1;
    }
}
