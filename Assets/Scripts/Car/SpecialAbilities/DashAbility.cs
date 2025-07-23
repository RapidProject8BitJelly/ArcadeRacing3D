using System;
using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "DashAbility", menuName = "ScriptableObjects/Abilities/DashAbility", order = 1)]
public class DashAbility : SpecialAbility
{
    [Header("Dash Settings")]
    [SerializeField] private float dashMaxSpeed;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float dashDuration;

    private bool _isOnCooldown;

    private void OnValidate()
    {
        _isOnCooldown = false;
    }

    public override void ActivateAbility(CarType carType)
    {
        if (_isOnCooldown)
        {
            return;
        }
        
        carType.StartCoroutine(UseDash(carType));
        carType.StartCoroutine(CountCooldown());
    }

    private IEnumerator UseDash(CarType carType)
    {
        _isOnCooldown = true;

        var previousMaxSpeed = carType.maxSpeed;
        carType.maxSpeed = dashMaxSpeed;
        yield return new WaitForSeconds(dashDuration);
        carType.maxSpeed = previousMaxSpeed;
    }

    private IEnumerator CountCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        _isOnCooldown = false;
    }
}
