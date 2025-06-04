using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oil : MonoBehaviour
{
    [SerializeField] private float maxSpeedMultiplier;
    [SerializeField] private float maxSpeedLockDuration;
    [Tooltip("How fast should the car slow down")]
    [SerializeField] private float dampingMultiplier;
    
    private List<GameObject> _slowedPlayers = new();
    
    private const float DefaultMaxSpeedMultiplier = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SlowDownPlayerCar(other.gameObject);
        }
    }
    
    private void SlowDownPlayerCar(GameObject player)
    {
        if(_slowedPlayers.Contains(player)) return;
        
        _slowedPlayers.Add(player);
        
        var carController = player.gameObject.GetComponent<CarController>();
        
        if(carController == null) return;
        
        carController.maxSpeedMultiplier = maxSpeedMultiplier;
        carController.dampingMultiplier = dampingMultiplier;
        StartCoroutine(WaitToRestoreMaxSpeed(player));
    }
    
    private IEnumerator WaitToRestoreMaxSpeed(GameObject player)
    {
        yield return new WaitForSeconds(maxSpeedLockDuration);
        
        var carController = player.gameObject.GetComponent<CarController>();
        carController.maxSpeedMultiplier = DefaultMaxSpeedMultiplier;
        
        var playerCarSettings = player.gameObject.GetComponent<PlayerCarSettings>();
        
        if(playerCarSettings != null)
        {
            carController.dampingMultiplier = playerCarSettings.dampingMultiplier;
        }

        _slowedPlayers.Remove(player);
    }
}