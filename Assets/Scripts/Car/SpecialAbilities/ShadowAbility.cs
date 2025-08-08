using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "ShadowAbility", menuName = "ScriptableObjects/Abilities/ShadowAbility", order = 1)]
public class ShadowAbility : SpecialAbility
{
    [Header("Shadow Settings")]
    [SerializeField] private float shadowCooldown;
    [SerializeField] private float shadowDuration;

    private List<Collider> _carColliders = new List<Collider>();
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
        
        carType.StartCoroutine(UseShadow(carType));
        carType.StartCoroutine(CountCooldown());
    }

    private IEnumerator UseShadow(CarType carType)
    {
        _isOnCooldown = true;

        if (_carColliders.Count == 0)
        {
            _carColliders = carType.gameObject.GetComponentsInChildren<Collider>().ToList();
        }
        
        foreach (var carCollider in _carColliders)
        {
            carCollider.gameObject.layer = 9;
        }
        
        yield return new WaitForSeconds(shadowDuration);
        
        foreach (var carCollider in _carColliders)
        {
            carCollider.gameObject.layer = 7;
        }
    }

    private IEnumerator CountCooldown()
    {
        yield return new WaitForSeconds(shadowCooldown);
        _isOnCooldown = false;
    }
}
