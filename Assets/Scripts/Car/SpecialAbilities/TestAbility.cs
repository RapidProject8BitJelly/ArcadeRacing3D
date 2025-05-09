using UnityEngine;

[CreateAssetMenu(fileName = "TestAbility", menuName = "ScriptableObjects/Abilities/TestAbility", order = 1)]
public class TestAbility : SpecialAbility
{
    public string abilityName = "TestAbility";
    
    public string AbilityName => abilityName;
    public override void ActivateAbility()
    {
        Debug.Log("Activate " + abilityName);
    }
}
