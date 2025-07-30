using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarCon : MonoBehaviour
{
    [SerializeField] private CarType carType;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private TempPlayerInfo tempPlayerInfo;
    [SerializeField] private float gravityMultiplier = 3.0f;
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private float raycastLength;
    
    private float _accelerationInput;
    private float _turnInput;
    private Rigidbody _rigidbody;
    private float _rotationAngle = 0f;
    private float _previousTurnInput;
    private bool _previousIsBraking;
    private bool isGrounded;
    private HashSet<Collider> _groundContacts = new HashSet<Collider>();
    
    private Coroutine _driftCoroutine;
    
    private PlayerInput _playerInput;

    public CinemachineVirtualCamera virtualCamera;
    public float maxSpeedMultiplier = 1;
    public float dampingMultiplier = 3;
    
    private float _pitchAngle = 0f; // używane przez AlignToGround
    private float _currentPitch = 0f;
    
    
    private PlayersInputActions _playersInputActions;
    private int buttonPressedBy = 0;
    private float groundedTimer;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();
        //virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
    }

    private void OnEnable()
    {
        //_playerInput.actions["UseSpecialAbility"].performed += OnUseSpecialAbility;
        _playersInputActions = new PlayersInputActions();
        _playersInputActions.Player1.Move.performed += context => {buttonPressedBy = 0;};
        _playersInputActions.Player2.Move.performed += context => {buttonPressedBy = 1;};
        _playersInputActions.Player1.UseSpecialAbility.performed += context => {buttonPressedBy = 0; OnUseSpecialAbility(context);};
        _playersInputActions.Player2.UseSpecialAbility.performed += context => {buttonPressedBy = 1; OnUseSpecialAbility(context);};
        _playersInputActions.Enable();
    }

    private void OnDisable()
    {
        //_playerInput.actions["UseSpecialAbility"].performed -= OnUseSpecialAbility;
        _playersInputActions.Player1.Move.performed -= context => {buttonPressedBy = 0;};
        _playersInputActions.Player2.Move.performed -= context => {buttonPressedBy = 1;};
        _playersInputActions.Player1.UseSpecialAbility.performed -= OnUseSpecialAbility;
        _playersInputActions.Player2.UseSpecialAbility.performed -= OnUseSpecialAbility;
        _playersInputActions.Disable();
    }
    
    public void SetupPlayerCamera()
    {
        virtualCamera.Follow = transform;
        virtualCamera.LookAt = transform;
    }
    private void FixedUpdate()
    {
        if (tempPlayerInfo.PlayerNumber == (PlayerNumbers)buttonPressedBy)
        {
            _accelerationInput = Input.GetAxis("Vertical");
            _turnInput = Input.GetAxis("Horizontal");
        }
        
        if (!IsGrounded())
        {
            _rigidbody.AddForce(Vector3.down * 80.0f, ForceMode.Acceleration); 
        }
        else
        {
            AlignToGround();
            AddSpeed();
            Drift();
            Turn();
            float lateralVelocity;
            bool isBraking;
            bool isScreeching = IsTireScreeching(out lateralVelocity, out isBraking);
            DrawTrails(isScreeching);
        }
        
        float speed = _rigidbody.linearVelocity.magnitude * 3.6f;
        //speedText.SetText(Mathf.RoundToInt(speed).ToString());
    }
    
    private void AddSpeed()
    {
        //pozyskac z car type
        if (_rigidbody.linearVelocity.magnitude > carType.maxSpeed*maxSpeedMultiplier && _accelerationInput > 0f && _accelerationInput > 0) return;
        if (_rigidbody.linearVelocity.magnitude > carType.maxSpeed*maxSpeedMultiplier * 0.5f && _accelerationInput < 0f && _accelerationInput < 0) return;
        
        if (_accelerationInput == 0f)
        {
            _rigidbody.linearDamping = Mathf.Lerp(_rigidbody.linearDamping, 0.3f, Time.fixedDeltaTime * dampingMultiplier);
        }
        else
        {
            _rigidbody.linearDamping = 0f;
        }
        
        //pozyskac z car type
        Vector3 engineForce = transform.forward * (carType.acceleration * _accelerationInput);
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
            foreach (var wheel in carType.wheels)
            {
                wheel.transform.DOLocalRotate(new Vector3(90, 0, 0), 0.5f);
            }
            carType.carBase.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
        }
        else if (_turnInput != 0 && _previousTurnInput == 0 || _turnInput > 0 && _previousTurnInput < 0 || _turnInput < 0 && _previousTurnInput > 0)
        {
            foreach (var wheel in carType.wheels)
            {
                if (_turnInput > 0f)
                {
                    wheel.transform.DOLocalRotate(new Vector3(90, 0, -35), 0.5f);
                }
                else if (_turnInput < 0f)
                {
                    wheel.transform.DOLocalRotate(new Vector3(90, 0, 35), 0.5f);
                }
            }
        }

        float minSpeedBeforeAllowTurningFactor = (_rigidbody.linearVelocity.magnitude / 8);
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);
        
        //refka
        _rotationAngle -= _turnInput * carType.turnFactor * minSpeedBeforeAllowTurningFactor;

        
        // Nowa rotacja: pitch z AlignToGround + yaw z Turn
        Quaternion combinedRotation = Quaternion.Euler(_currentPitch, -_rotationAngle, 0f);
        _rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.rotation, combinedRotation, Time.fixedDeltaTime * 5f));

        _previousTurnInput = _turnInput;
    }
    
    private void Drift()
    {
        Vector3 forwardVelocity = transform.forward * Vector3.Dot(_rigidbody.linearVelocity, transform.forward);
        Vector3 rightVelocity = transform.right * Vector3.Dot(_rigidbody.linearVelocity, transform.right);

        //cartype refka
        _rigidbody.linearVelocity = forwardVelocity + rightVelocity * carType.driftFactor;
    }
    
    private bool IsTireScreeching(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = Vector3.Dot(_rigidbody.linearVelocity, transform.right);
        isBraking = false;

        //refka
        if (_accelerationInput < 0 && Vector3.Dot(_rigidbody.linearVelocity, transform.forward) > 0f 
                                   && _rigidbody.linearVelocity.magnitude > carType.minSpeedToShowTrails)
        {
            isBraking = true;
        }

        if (Mathf.Abs(lateralVelocity) > 3f)
        {
            isBraking = true;
        }

        if (!_previousIsBraking && isBraking && carType.wheels.Length > 0)
        {
            //zamienic command na normalna metode
            _driftCoroutine = StartCoroutine(PlayDriftAudio());
        }
        else if (_previousIsBraking && !isBraking && carType.wheels.Length > 0)
        {
            //na normalna metode
            StopDriftAudio();
        }
        
        _previousIsBraking = isBraking;
        return isBraking;
    }
    
    public void SetNewRotation(float rotationAngle)
    {
        _rotationAngle = rotationAngle;
    }

    private void OnUseSpecialAbility(InputAction.CallbackContext callbackContext)
    {
        if (tempPlayerInfo.PlayerNumber == (PlayerNumbers)buttonPressedBy)
        {
            carType.UseSpecialAbility();
        }
    }

    private IEnumerator PlayDriftAudio()
    {
        carType.audioSource.clip = audioClips[0];
        carType.audioSource.Play();
        yield return new WaitForSeconds(audioClips[0].length);
        carType.audioSource.clip = audioClips[1];
        carType.audioSource.Play();
        carType.audioSource.loop = true;
        _driftCoroutine = null;
    }
    
    private void StopDriftAudio()
    {
        carType.audioSource.Stop();
        if (_driftCoroutine != null)
        {
            StopCoroutine(_driftCoroutine);
            _driftCoroutine = null;
        }
        carType.audioSource.clip = audioClips[2];
        carType.audioSource.loop = false;
        carType.audioSource.Play();
    }
    
    private void DrawTrails(bool screeching)
    {
        for (int i = 0; i < carType.trailsRenderer.Length; i++)
        {
            if (carType.trailsRenderer[i] != null)
            {
                carType.trailsRenderer[i].emitting = screeching;
            }
            if (carType.particleSystems[i] != null)
            {
                if(!carType.particleSystems[i].gameObject.activeSelf) carType.particleSystems[i].gameObject.SetActive(true);
                var emission = carType.particleSystems[i].emission;
                emission.enabled = screeching;
            }
        }
    }
    
    private bool IsGrounded()
    {
        Vector3 front = transform.position + transform.forward * 0.8f + Vector3.up * 0.1f;
        Vector3 back  = transform.position - transform.forward * 0.8f + Vector3.up * 0.1f;
        LayerMask groundMask = LayerMask.GetMask("Track");

        bool hitFront = Physics.Raycast(front, Vector3.down, raycastLength, groundMask);
        bool hitBack  = Physics.Raycast(back, Vector3.down, raycastLength, groundMask);

        return hitFront || hitBack;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 front = transform.position + transform.forward * 0.8f + Vector3.up * 0.1f;
        Vector3 back  = transform.position - transform.forward * 0.8f + Vector3.up * 0.1f;

        Gizmos.DrawLine(front, front + Vector3.down * raycastLength);
        Gizmos.DrawLine(back, back + Vector3.down * raycastLength);
    }
}
