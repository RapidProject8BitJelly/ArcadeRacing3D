using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestAbility2", menuName = "ScriptableObjects/Abilities/TestAbility2", order = 1)]
public class TestAbility2 : SpecialAbility
{
    [SerializeField] private string abilityName = "TestAbility2";
    public string AbilityName => abilityName;
    public override void ActivateAbility()
    {
        Debug.Log("Activate " + abilityName);
    }
}
