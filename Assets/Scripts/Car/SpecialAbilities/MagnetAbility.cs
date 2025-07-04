using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MagnetAbility", menuName = "ScriptableObjects/Abilities/MagnetAbility", order = 2)]
public class MagnetAbility : SpecialAbility
{
    [Header("Magnet Settings")]
    [SerializeField] private float magnetCooldown = 5f;
    [SerializeField] private float magnetDuration = 2f;
    [SerializeField] private float magnetStrength = 20f;
    [SerializeField] private float magnetRadius = 2f;
    [SerializeField] private float magnetTurnOffDistance = 2f;

    private bool _isOnCooldown = false;
    
    private void OnValidate()
    {
        _isOnCooldown = false;
    }

    public override void ActivateAbility(CarType carType)
    {
        if (_isOnCooldown)
            return;

        carType.StartCoroutine(UseMagnet(carType));
        carType.StartCoroutine(CountCooldown());
    }

    private IEnumerator UseMagnet(CarType carType)
    {
        _isOnCooldown = true;
        
        Collider[] colliders = Physics.OverlapSphere(carType.transform.position, magnetRadius);
        Collider nearestCar = null;
        float minDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            if (collider.gameObject == carType.gameObject)
                continue;
            if (collider.gameObject.layer != LayerMask.NameToLayer("Player"))
                continue;

            float distance = Vector3.Distance(carType.transform.position, collider.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestCar = collider;
            }
        }

        if (nearestCar != null)
        {
            Rigidbody targetRb = nearestCar.attachedRigidbody;
            if (targetRb != null)
            {
                float timer = 0f;
                
                targetRb.linearVelocity = Vector3.zero;
                
                float distance = Vector3.Distance(carType.transform.position, nearestCar.transform.position);
                
                while (timer < magnetDuration && distance > magnetTurnOffDistance)
                {
                    distance = Vector3.Distance(carType.transform.position, nearestCar.transform.position);
                    Vector3 direction = (carType.transform.position - targetRb.position).normalized;
                    targetRb.AddForce(direction * magnetStrength, ForceMode.Acceleration);
                    
                    targetRb.linearVelocity *= 0.5f;

                    timer += Time.deltaTime;
                    yield return null;
                }
            }
        }

        yield return null;
    }

    private IEnumerator CountCooldown()
    {
        yield return new WaitForSeconds(magnetCooldown);
        _isOnCooldown = false;
    }
}