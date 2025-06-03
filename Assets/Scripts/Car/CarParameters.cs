using UnityEngine;

[CreateAssetMenu(fileName = "CarParameters", menuName = "ScriptableObjects/CarParameters", order = 1)]
public class CarParameters : ScriptableObject
{
    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float turnFactor;
    [SerializeField] private float driftFactor;
    [SerializeField] private float minSpeedToShowTrails;
    
    [SerializeField] private SpecialAbility specialAbility;
    [SerializeField] private Color[] carColors;
    
    public float Acceleration => acceleration;
    public float MaxSpeed => maxSpeed;
    public float TurnFactor => turnFactor;
    public float DriftFactor => driftFactor;
    public float MinSpeedToShowTrails => minSpeedToShowTrails;
    
    public SpecialAbility SpecialAbility => specialAbility;
    public Color[] CarColors => carColors;
}
