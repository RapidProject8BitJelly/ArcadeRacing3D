using UnityEngine;

public class CarType : MonoBehaviour
{
    public CarParameters carParameters;
    public GameObject[] elementsToChangeColor;
    public GameObject carAccessories;
    public TrailRenderer[] trailsRenderer;
    public ParticleSystem[] particleSystems;
    public GameObject[] wheels;
    public GameObject carBase;
    public AudioSource audioSource;

    public float acceleration;
    public float maxSpeed;
    public float turnFactor;
    public float driftFactor;
    public float minSpeedToShowTrails;
    public float dampingMultiplier;

    private void Awake()
    {
        acceleration = carParameters.Acceleration;
        maxSpeed = carParameters.MaxSpeed;
        turnFactor = carParameters.TurnFactor;
        driftFactor = carParameters.DriftFactor;
        minSpeedToShowTrails = carParameters.MinSpeedToShowTrails;
        dampingMultiplier = carParameters.DampingMultiplier;
    }

    public CarParameters GetCarParameters()
    {
        return carParameters;
    }
    
    public GameObject[] GetElementsToChangeColor()
    {
        return elementsToChangeColor;
    }

    public GameObject GetCarAccessories()
    {
        return carAccessories;
    }

    public TrailRenderer[] GetTrailsRenderer()
    {
        return trailsRenderer;
    }

    public ParticleSystem[] GetParticleSystems()
    {
        return particleSystems;
    }

    public GameObject[] GetWheels()
    {
        return wheels;
    }

    public GameObject GetCarBase()
    {
        return carBase;
    }

    public AudioSource GetCarAudioSource()
    {
        return audioSource;
    }
    
    public void UseSpecialAbility()
    {
        carParameters.SpecialAbility?.ActivateAbility();
    }
}
