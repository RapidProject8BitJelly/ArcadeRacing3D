using UnityEngine;

public class CarType : MonoBehaviour
{
    [SerializeField] private CarParameters carParameters;
    [SerializeField] private GameObject[] elementsToChangeColor;
    [SerializeField] private GameObject carAccessories;
    [SerializeField] private TrailRenderer[] trailsRenderer;
    [SerializeField] private ParticleSystem[] particleSystems;
    [SerializeField] private GameObject[] wheels;
    [SerializeField] private GameObject carBase;
    [SerializeField] private AudioSource carAudioSource;

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
        return carAudioSource;
    }
    
    public void UseSpecialAbility()
    {
        carParameters.SpecialAbility?.ActivateAbility();
    }
}
