using System.Collections;
using UnityEngine;

public class Oil : MonoBehaviour
{
    [SerializeField] private float maxSpeedMultiplier;
    [SerializeField] private float maxSpeedLockDuration;
    [Tooltip("How fast should the car slow down")]
    [SerializeField] private float dampingMultiplier;
    
    private float defaultMaxSpeedMultiplier;
    private float defaultDampingMultiplier;
    private CarController carController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SlowDownPlayerCar(other.gameObject);
        }
    }
    
    private void SlowDownPlayerCar(GameObject player)
    {
        carController = player.gameObject.GetComponent<CarController>();
        
        defaultMaxSpeedMultiplier = carController.maxSpeedMultiplier;
        defaultDampingMultiplier = carController.dampingMultiplier;
        
        carController.maxSpeedMultiplier = maxSpeedMultiplier;
        carController.dampingMultiplier = dampingMultiplier;
        StartCoroutine(WaitToRestoreMaxSpeed());
    }
    
    private IEnumerator WaitToRestoreMaxSpeed()
    {
        yield return new WaitForSeconds(maxSpeedLockDuration);
        carController.maxSpeedMultiplier = defaultMaxSpeedMultiplier;
        carController.dampingMultiplier = defaultDampingMultiplier;
    }
}