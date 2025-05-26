using System;
using System.Collections;
using Cinemachine;
using DG.Tweening;
using Mirror;
using TMPro;
using UnityEngine;

public class CarController : NetworkBehaviour
{
    [SerializeField] private PlayerCarSettings _playerCarSettings;

    [SerializeField] private AudioClip[] audioClips;
    
    private float _accelerationInput;
    private float _turnInput;
    private Rigidbody _rigidbody;
    private float _rotationAngle = 0f;
    private float _previousTurnInput;
    private bool _previousIsBraking;
    
    private Coroutine _driftCoroutine;

    public CinemachineVirtualCamera virtualCamera;
    public float maxSpeedMultiplier = 1;
    
    private float _pitchAngle = 0f; // używane przez AlignToGround
    private float _currentPitch = 0f;
    
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
        
        Debug.Log("Current maxspeed: " + _playerCarSettings.maxSpeed*maxSpeedMultiplier);
        
        _accelerationInput = Input.GetAxis("Vertical");
        _turnInput = Input.GetAxis("Horizontal");
        AlignToGround();
        AddSpeed();
        Drift();
        Turn();
        float lateralVelocity;
        bool isBraking;
        bool isScreeching = IsTireScreeching(out lateralVelocity, out isBraking);

        SetTrailsRenderers(isScreeching);
        
        float speed = _rigidbody.linearVelocity.magnitude * 3.6f;
        Debug.Log("current speed: " + speed.ToString("0"));
        //speedText.SetText(Mathf.RoundToInt(speed).ToString());
    }

    private void AddSpeed()
    {
        if (_rigidbody.linearVelocity.magnitude > _playerCarSettings.maxSpeed && _accelerationInput > 0f && _accelerationInput > 0) return;
        if (_rigidbody.linearVelocity.magnitude > _playerCarSettings.maxSpeed * 0.5f && _accelerationInput < 0f && _accelerationInput < 0) return;
        if (_rigidbody.velocity.magnitude > _playerCarSettings.maxSpeed*maxSpeedMultiplier && _accelerationInput > 0f && _accelerationInput > 0) return;
        if (_rigidbody.velocity.magnitude > _playerCarSettings.maxSpeed*maxSpeedMultiplier * 0.5f && _accelerationInput < 0f && _accelerationInput < 0) return;
        
        if (_accelerationInput == 0f)
        {
            _rigidbody.linearDamping = Mathf.Lerp(_rigidbody.linearDamping, 0.3f, Time.fixedDeltaTime * 3);
        }
        else
        {
            _rigidbody.linearDamping = 0f;
        }
        
        Vector3 engineForce = transform.forward * (_playerCarSettings.acceleration * _accelerationInput);
        _rigidbody.AddForce(engineForce, ForceMode.Force);
    }
    
    private void AlignToGround()
    {
        float raycastDistance = 5f;

        Vector3 front = transform.position + transform.forward * 1.5f + Vector3.up * 1.0f;
        Vector3 back = transform.position - transform.forward * 1.5f + Vector3.up * 1.0f;

        if (Physics.Raycast(front, Vector3.down, out RaycastHit hitFront, raycastDistance) &&
            Physics.Raycast(back, Vector3.down, out RaycastHit hitBack, raycastDistance))
        {
            Vector3 slopeDirection = (hitFront.point - hitBack.point).normalized;
            Vector3 surfaceNormal = Vector3.Cross(slopeDirection, transform.right);

            // Oblicz docelową rotację pitch względem terenu
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, surfaceNormal), surfaceNormal);
            float targetPitch = targetRotation.eulerAngles.x;

            // Poprawka dla kąta (360 → -180/+180)
            if (targetPitch > 180f) targetPitch -= 360f;

            // Ogranicz pitch do przedziału -15° do 15°
            targetPitch = Mathf.Clamp(targetPitch, -15f, 15f);

            // Płynne przejście
            _currentPitch = Mathf.LerpAngle(_currentPitch, targetPitch, Time.fixedDeltaTime * 5f);
        }
    }

    private void Turn()
    {
        if (_turnInput == 0 && _previousTurnInput != 0)
        {
            foreach (var wheel in _playerCarSettings.wheels)
            {
                wheel.transform.DOLocalRotate(new Vector3(90, 90, 0), 0.5f);
            }
            _playerCarSettings.carBase.transform.DOLocalRotate(new Vector3(0, 90, 0), 0.5f);
        }
        else if (_turnInput != 0 && _previousTurnInput == 0 || _turnInput > 0 && _previousTurnInput < 0 || _turnInput < 0 && _previousTurnInput > 0)
        {
            foreach (var wheel in _playerCarSettings.wheels)
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

        float minSpeedBeforeAllowTurningFactor = (_rigidbody.linearVelocity.magnitude / 8);
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);
        _rotationAngle -= _turnInput * _playerCarSettings.turnFactor * minSpeedBeforeAllowTurningFactor;

        // Nowa rotacja: pitch z AlignToGround + yaw z Turn
        Quaternion combinedRotation = Quaternion.Euler(_currentPitch, -_rotationAngle, 0f);
        _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, combinedRotation, Time.fixedDeltaTime * 100f));

        _previousTurnInput = _turnInput;
    }

    private void Drift()
    {
        Vector3 forwardVelocity = transform.forward * Vector3.Dot(_rigidbody.linearVelocity, transform.forward);
        Vector3 rightVelocity = transform.right * Vector3.Dot(_rigidbody.linearVelocity, transform.right);

        _rigidbody.linearVelocity = forwardVelocity + rightVelocity * _playerCarSettings.driftFactor;
    }

    private void SetTrailsRenderers(bool screeching)
    {
        CmdDrawTrails(screeching);
    }

    private bool IsTireScreeching(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = Vector3.Dot(_rigidbody.linearVelocity, transform.right);
        isBraking = false;

        if (_accelerationInput < 0 && Vector3.Dot(_rigidbody.linearVelocity, transform.forward) > 0f 
                                   && _rigidbody.linearVelocity.magnitude > _playerCarSettings.minSpeedToShowTrails)
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
        _playerCarSettings.audioSource.clip = audioClips[0];
        _playerCarSettings.audioSource.Play();
        yield return new WaitForSeconds(audioClips[0].length);
        _playerCarSettings.audioSource.clip = audioClips[1];
        _playerCarSettings.audioSource.Play();
        _playerCarSettings.audioSource.loop = true;
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
        _playerCarSettings.audioSource.Stop();
        if (_driftCoroutine != null)
        {
            StopCoroutine(_driftCoroutine);
            _driftCoroutine = null;
        }
        _playerCarSettings.audioSource.clip = audioClips[2];
        _playerCarSettings.audioSource.loop = false;
        _playerCarSettings.audioSource.Play();
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
            if (_playerCarSettings.trails[i] != null)
            {
                _playerCarSettings.trails[i].emitting = screeching;
            }
            if (_playerCarSettings.particles[i] != null)
            {
                if(!_playerCarSettings.particles[i].gameObject.activeSelf) _playerCarSettings.particles[i].gameObject.SetActive(true);
                var emission = _playerCarSettings.particles[i].emission;
                emission.enabled = screeching;
            }
        }
    }
    
    public void StopCar()
    {
        //StartCoroutine(StopCarCoroutine());
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