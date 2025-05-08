using System.Collections;
using Mirror;
using UnityEngine;

public class PlayerCarSettings : NetworkBehaviour
{
    [SyncVar (hook = nameof(OnCarIDChanged))] public int carID = -1;
    [SyncVar (hook = nameof(OnColorChanged))] public int colorID = -1;
    [SyncVar (hook = nameof(OnAccessoriesChanged))] public int accessoriesID = -1;
    
    private GameObject _playerCar;
    [SerializeField] private CarController _carController;
    public GameObject[] wheels;
    public GameObject carBase;
    public TrailRenderer[] trails;
    public ParticleSystem[] particles;
    public AudioSource audioSource;

    [SerializeField] private GameObject[] _cars;

    private GameObject[] elementsToChangeColor;
    private CarParameters carParams;
    private GameObject carAccessories;
    
    public float acceleration;
    public float maxSpeed;
    public float turnFactor;
    public float driftFactor;
    public float minSpeedToShowTrails;
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        StartCoroutine(DelayedSetup());
    }
    
    public override void OnStartLocalPlayer()
    {
        CmdSetPlayerCar(carID, colorID, accessoriesID);
    }
    
    
    [Command]
    public void CmdSetPlayerCar(int carId, int colorId, int accessoriesId, NetworkConnectionToClient client = null)
    {
        if (isLocalPlayer)
        {
            // Host – wywołujemy bez sieci
            SetupPlayerAuto(carId, colorId, accessoriesId);
        }
        else
        {
            // Klient – wyślij ClientRpc
            TargetSetPlayerAuto(carId, colorId, accessoriesId);
        }
    }

    [ClientRpc]
    private void TargetSetPlayerAuto(int carId, int colorId, int accessoriesId)
    {
        SetupPlayerAuto(carId, colorId, accessoriesId);
    }

    private void SetupPlayerAuto(int carId, int colorId, int accessoriesId)
    {
        for (int i = 0; i < _cars.Length; i++)
        {
            if (carID == i)
            {
                _cars[i].SetActive(true);
                _playerCar = _cars[i];
            }
            else _cars[i].SetActive(false);
        }

        GameObject[] elementsToChangeColor = _playerCar.GetComponent<CarType>().GetElementsToChangeColor();
        CarParameters carParams = _playerCar.GetComponent<CarType>().GetCarParameters();
        GameObject carAccessories = _playerCar.GetComponent<CarType>().GetCarAccessories();
        
        for (int i = 0; i < elementsToChangeColor.Length; i++)
        {
            elementsToChangeColor[i].GetComponent<Renderer>().material.color = carParams.CarColors[colorID];
        }

        for (int i = 0; i < carAccessories.transform.childCount; i++)
        {
            if(i==accessoriesID) carAccessories.transform.GetChild(i).gameObject.SetActive(true);
            else carAccessories.transform.GetChild(i).gameObject.SetActive(false);
        }
        
        
        //_playerCar.transform.position = new Vector3(0, 0, 0);
        _playerCar.transform.rotation = Quaternion.Euler(0, 0, 0);
        trails = _playerCar.GetComponent<CarType>().GetTrailsRenderer();
        particles = _playerCar.GetComponent<CarType>().GetParticleSystems();
        wheels = _playerCar.GetComponent<CarType>().GetWheels();
        carBase = _playerCar.GetComponent<CarType>().GetCarBase();
        
        acceleration = carParams.Acceleration;
        maxSpeed = carParams.MaxSpeed;
        turnFactor = carParams.TurnFactor;
        driftFactor = carParams.DriftFactor;
        minSpeedToShowTrails = carParams.MinSpeedToShowTrails;
        
        _carController.ApplySettingsFrom(this);
        //audioSource = _playerCar.GetComponent<CarType>().GetCarAudioSource();
    }

    private void OnCarIDChanged(int oldValue, int newValue)
    {
        for (int i = 0; i < _cars.Length; i++)
        {
            if (carID == i)
            {
                _cars[i].SetActive(true);
                _playerCar = _cars[i];
            }
            else _cars[i].SetActive(false);
        }
        elementsToChangeColor = _playerCar.GetComponent<CarType>().GetElementsToChangeColor();
        carParams = _playerCar.GetComponent<CarType>().GetCarParameters();
        carAccessories = _playerCar.GetComponent<CarType>().GetCarAccessories();
    }

    private void OnColorChanged(int oldValue, int newValue)
    {
        for (int i = 0; i < elementsToChangeColor.Length; i++)
        {
            elementsToChangeColor[i].GetComponent<Renderer>().material.color = carParams.CarColors[colorID];
        }
    }

    private void OnAccessoriesChanged(int oldValue, int newValue)
    {
        for (int i = 0; i < carAccessories.transform.childCount; i++)
        {
            if(i==accessoriesID) carAccessories.transform.GetChild(i).gameObject.SetActive(true);
            else carAccessories.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    
    private IEnumerator DelayedSetup()
    {
        yield return null; // 1 frame później, po SyncVarach
        SetupPlayerAuto(carID, colorID, accessoriesID);
    }
}