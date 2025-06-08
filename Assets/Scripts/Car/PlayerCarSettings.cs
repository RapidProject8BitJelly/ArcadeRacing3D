using System.Collections;
using Mirror;
using UnityEngine;

public class PlayerCarSettings : NetworkBehaviour
{
    #region Variables

    [SyncVar (hook = nameof(OnCarIDChanged))] 
    public int carID;
    [SyncVar (hook = nameof(OnColorChanged))] 
    public int colorID;
    [SyncVar(hook = nameof(OnAccessoriesChanged))]
    public int accessoriesID;
    [SyncVar(hook = nameof(OnNicknameChanged))]
    public string nickname;
    
    [SerializeField] private GameObject[] cars;
    
    private GameObject[] elementsToChangeColor;
    private CarParameters carParams;
    private GameObject carAccessories;
    private GameObject _playerCar;
    
    public GameObject[] wheels;
    public GameObject carBase;
    public TrailRenderer[] trails;
    public ParticleSystem[] particles;
    public AudioSource audioSource;
    
    public float acceleration;
    public float maxSpeed;
    public float turnFactor;
    public float driftFactor;
    public float minSpeedToShowTrails;
    public float dampingMultiplier;

    #endregion
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        StartCoroutine(DelayedSetup());
    }
    
    public override void OnStartLocalPlayer()
    {
        CmdSetPlayerCar();
    }
    
    [Command]
    public void CmdSetPlayerCar(NetworkConnectionToClient client = null)
    {
        if (isLocalPlayer)
        {
            SetupPlayerCar();
        }
        else
        {
            TargetSetPlayerAuto();
        }
    }

    [ClientRpc]
    private void TargetSetPlayerAuto()
    {
        SetupPlayerCar();
    }

    #region CarSelection

    private void ApplyCarSelection()
    {
        for (int i = 0; i < cars.Length; i++)
        {
            if (carID == i)
            {
                cars[i].SetActive(true);
                _playerCar = cars[i];
            }
            else Destroy(cars[i]);
        }

        elementsToChangeColor = _playerCar.GetComponent<CarType>().GetElementsToChangeColor();
        carParams = _playerCar.GetComponent<CarType>().GetCarParameters();
        carAccessories = _playerCar.GetComponent<CarType>().GetCarAccessories();
        _playerCar.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    
    private void OnCarIDChanged(int oldValue, int newValue)
    {
        ApplyCarSelection();
    }

    #endregion

    #region CarColor
    
    private void ApplyCarColor()
    {
        if(_playerCar == null) ApplyCarSelection();
        for (int i = 0; i < elementsToChangeColor.Length; i++)
        {
            elementsToChangeColor[i].GetComponent<Renderer>().material.color = carParams.CarColors[colorID];
        }
    }

    private void OnColorChanged(int oldValue, int newValue)
    {
        ApplyCarColor();
    }
    
    #endregion

    #region CarAccessory

    private void ApplyCarAccessories()
    {
        if(_playerCar == null) ApplyCarSelection();
        for (int i = 0; i < carAccessories.transform.childCount; i++)
        {
            if(i==accessoriesID) carAccessories.transform.GetChild(i).gameObject.SetActive(true);
            else carAccessories.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    
    private void OnAccessoriesChanged(int oldValue, int newValue)
    {
        ApplyCarAccessories();
    }

    #endregion

    #region CarSetup

    private void SetupPlayerCar()
    {
        ApplyCarSelection();
        ApplyCarColor();
        ApplyCarAccessories();
        ApplyCarParameters();
    }
    
    private void ApplyCarParameters()
    {
        trails = _playerCar.GetComponent<CarType>().GetTrailsRenderer();
        particles = _playerCar.GetComponent<CarType>().GetParticleSystems();
        wheels = _playerCar.GetComponent<CarType>().GetWheels();
        carBase = _playerCar.GetComponent<CarType>().GetCarBase();
        audioSource = _playerCar.GetComponent<CarType>().GetCarAudioSource();
        
        acceleration = carParams.Acceleration;
        maxSpeed = carParams.MaxSpeed;
        turnFactor = carParams.TurnFactor;
        driftFactor = carParams.DriftFactor;
        minSpeedToShowTrails = carParams.MinSpeedToShowTrails;
        dampingMultiplier = carParams.DampingMultiplier;
    }
    
    private IEnumerator DelayedSetup()
    {
        yield return null; 
        SetupPlayerCar();
    }

    #endregion

    private void OnNicknameChanged(string oldValue, string newValue)
    {
        if (isLocalPlayer)
        {
            GetComponent<RaceProgressTracker>().playerNickname = newValue + " (You)";
        }
        else
        {
            GetComponent<RaceProgressTracker>().playerNickname = newValue;
        }
    }
}