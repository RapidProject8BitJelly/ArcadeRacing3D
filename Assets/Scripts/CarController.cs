using System;
using System.Collections;
using Cinemachine;
using DG.Tweening;
using Mirror;
using Mirror.BouncyCastle.Crypto;
using TMPro;
using UnityEngine;

public class CarController : NetworkBehaviour
{
    [SerializeField] private GameObject[] wheels;
    [SerializeField] private GameObject carBase;
    [SerializeField] private PlayerCarSettings _playerCarSettings;
    
    //[SerializeField] private float acceleration;
    private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float turnFactor;
    [SerializeField] private float driftFactor;

    [SerializeField] private float minSpeedToShowTrails;
    //[SerializeField] private TMP_Text speedText;
    [SerializeField] private TrailRenderer[] trailsRenderer;
    [SerializeField] private ParticleSystem[] particleSystem;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip[] audioClips;
    
    private float _accelerationInput;
    private float _turnInput;
    private Rigidbody _rigidbody;
    private float _rotationAngle = 0f;
    private float _previousTurnInput;
    private bool _previousIsBraking;
    
    private Coroutine _driftCoroutine;

    public CinemachineVirtualCamera virtualCamera;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            GetComponent<AudioListener>().enabled = false;
        }
        else
        {
            GetComponent<AudioListener>().enabled = true;
        }
    }

    public override void OnStartLocalPlayer()
    {
        if (virtualCamera != null)
        {
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }
        GetComponent<AudioListener>().enabled = true;
    }
    
    public void SetSpectateTarget(Transform target)
    {
        if (!isLocalPlayer || virtualCamera == null) return;

        virtualCamera.Follow = target;
        virtualCamera.LookAt = target;
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        
        _accelerationInput = Input.GetAxis("Vertical");
        _turnInput = Input.GetAxis("Horizontal");
        AddSpeed();
        Drift();
        Turn();
        float lateralVelocity;
        bool isBraking;
        bool isScreeching = IsTireScreeching(out lateralVelocity, out isBraking);

        SetTrailsRenderers(isScreeching);
        
        float speed = _rigidbody.velocity.magnitude * 3.6f;
        //speedText.SetText(Mathf.RoundToInt(speed).ToString());
    }

    private void AddSpeed()
    {
        if (_rigidbody.velocity.magnitude > maxSpeed && _accelerationInput > 0f && _accelerationInput > 0) return;
        if (_rigidbody.velocity.magnitude > maxSpeed * 0.5f && _accelerationInput < 0f && _accelerationInput < 0) return;
        
        if (_accelerationInput == 0f)
        {
            _rigidbody.drag = Mathf.Lerp(_rigidbody.drag, 0.3f, Time.fixedDeltaTime * 3);
        }
        else
        {
            _rigidbody.drag = 0f;
        }
        
        Debug.Log(_playerCarSettings.acceleration);
        Vector3 engineForce = transform.forward * (_playerCarSettings.acceleration * _accelerationInput);
        _rigidbody.AddForce(engineForce, ForceMode.Force);
    }

    private void Turn()
    {
        if (_turnInput == 0 && _previousTurnInput != 0)
        {
            foreach (var wheel in wheels)
            {
                wheel.transform.DOLocalRotate(new Vector3(90, 90, 0), 0.5f);
            }
            carBase.transform.DOLocalRotate(new Vector3(0, 90, 0), 0.5f);
        }
        else if (_turnInput != 0 && _previousTurnInput == 0 || _turnInput > 0 && _previousTurnInput < 0 || _turnInput < 0 && _previousTurnInput > 0) 
        {
            foreach (var wheel in wheels)
            {
                if (_turnInput > 0f)
                {
                    wheel.transform.DOLocalRotate(new Vector3(90, 90, -35), 0.5f);
                }
                else if (_turnInput < 0f)
                {
                    wheel.transform.DOLocalRotate(new Vector3(90, 90, 35), 0.5f);
                }
            }
        }
        float minSpeedBeforeAllowTurningFactor = (_rigidbody.velocity.magnitude / 8);
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);
        _rotationAngle -= _turnInput * turnFactor * minSpeedBeforeAllowTurningFactor;
        Quaternion deltaRotation = Quaternion.Euler(0f, -_rotationAngle, 0f);
        
        _rigidbody.MoveRotation(deltaRotation);
        _previousTurnInput = _turnInput;
    }
    private void Drift()
    {
        Vector3 forwardVelocity = transform.forward * Vector3.Dot(_rigidbody.velocity, transform.forward);
        Vector3 rightVelocity = transform.right * Vector3.Dot(_rigidbody.velocity, transform.right);

        _rigidbody.velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    private void SetTrailsRenderers(bool screeching)
    {
        CmdDrawTrails(screeching);
    }

    private bool IsTireScreeching(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = Vector3.Dot(_rigidbody.velocity, transform.right);
        isBraking = false;

        if (_accelerationInput < 0 && Vector3.Dot(_rigidbody.velocity, transform.forward) > 0f 
                                   && _rigidbody.velocity.magnitude > minSpeedToShowTrails)
        {
            isBraking = true;
        }

        if (Mathf.Abs(lateralVelocity) > 3f)
        {
            isBraking = true;
        }

        if (!_previousIsBraking && isBraking)
        {
            CmdPlayAudio();
        }
        else if (_previousIsBraking && !isBraking)
        {
            CmdPlayStopDriftAudio();
        }
        
        _previousIsBraking = isBraking;
        return isBraking;
    }
    
    public void SetNewRotation(float rotationAngle)
    {
        _rotationAngle = rotationAngle;
    }

    [Command]

    private void CmdPlayAudio()
    {
        ClientPlayDriftAudio();
    }

    [ClientRpc]
    private void ClientPlayDriftAudio()
    {
        _driftCoroutine = StartCoroutine(PlayDriftAudio());
    }
    
    private IEnumerator PlayDriftAudio()
    {
        audioSource.clip = audioClips[0];
        audioSource.Play();
        yield return new WaitForSeconds(audioClips[0].length);
        audioSource.clip = audioClips[1];
        audioSource.Play();
        audioSource.loop = true;
        _driftCoroutine = null;
    }

    [Command]
    private void CmdPlayStopDriftAudio()
    {
        ClientPlayStopDriftAudio();
    }

    [ClientRpc]
    private void ClientPlayStopDriftAudio()
    {
        audioSource.Stop();
        if (_driftCoroutine != null)
        {
            StopCoroutine(_driftCoroutine);
            _driftCoroutine = null;
        }
        audioSource.clip = audioClips[2];
        audioSource.loop = false;
        audioSource.Play();
    }

    [Command]
    private void CmdDrawTrails(bool screeching)
    {
        ClientDrawTrails(screeching);
    }

    [ClientRpc]
    private void ClientDrawTrails(bool screeching)
    {
        for (int i = 0; i < 2; i++)
        {
            if (trailsRenderer[i] != null)
            {
                trailsRenderer[i].emitting = screeching;
            }
            if (particleSystem[i] != null)
            {
                if(!particleSystem[i].gameObject.activeSelf) particleSystem[i].gameObject.SetActive(true);
                var emission = particleSystem[i].emission;
                emission.enabled = screeching;
            }
        }
    }
    
    public void StopCar()
    {
        //StartCoroutine(StopCarCoroutine());
    }
    
    public void ApplySettingsFrom(PlayerCarSettings settings)
    {
        acceleration = settings.acceleration;
        maxSpeed = settings.maxSpeed;
        turnFactor = settings.turnFactor;
        driftFactor = settings.driftFactor;
        minSpeedToShowTrails = settings.minSpeedToShowTrails;
        Debug.Log(settings.acceleration);

        Debug.Log("Zastosowano parametry z PlayerCarSettings w CarController.");
    }
    
    //
    // IEnumerator StopCarCoroutine()
    // {
    //     yield return new WaitForSeconds(0.5f);
    //     
    //     if (_rb != null)
    //     {
    //         _rb.velocity = Vector2.zero;
    //         _rb.angularVelocity = 0f;
    //     }
    //     _accelerationInput = 0f;
    //     _velocityVsUp = 0f;
    //     _rotationAngle = 0f;
    //     _steeringInput = 0f;
    //     
    //     CmdStopAllParticlesAndTrailRenderers();
    // }
}