using TMPro;
using UnityEngine;

public class CarController2 : MonoBehaviour
{
    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float turnFactor;
    [SerializeField] private float driftFactor;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TrailRenderer[] trailsRenderer;
    [SerializeField] private ParticleSystem[] particleSystem;

    private float _accelerationInput;
    private float _turnInput;
    private Rigidbody _rigidbody;
    private float _rotationAngle = 0f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
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
        speedText.SetText(Mathf.RoundToInt(speed).ToString());
    }

    private void AddSpeed()
    {
        if (_rigidbody.velocity.magnitude > maxSpeed && _accelerationInput > 0f && _accelerationInput > 0) return;
        if (_rigidbody.velocity.magnitude > maxSpeed * 0.5f && _accelerationInput < 0f && _accelerationInput < 0) return;
        
        if (_accelerationInput == 0f)
        {
            _rigidbody.drag = Mathf.Lerp(_rigidbody.drag, 3.0f, Time.fixedDeltaTime * 3);
        }
        else
        {
            _rigidbody.drag = 0f;
        }
        
        Vector3 engineForce = transform.forward * (acceleration * _accelerationInput);
        _rigidbody.AddForce(engineForce, ForceMode.Force);
    }

    private void Turn()
    {
        float minSpeedBeforeAllowTurningFactor = (_rigidbody.velocity.magnitude / 8);
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);
        _rotationAngle -= _turnInput * turnFactor * minSpeedBeforeAllowTurningFactor;
        Quaternion deltaRotation = Quaternion.Euler(0f, -_rotationAngle, 0f);
        _rigidbody.MoveRotation(deltaRotation);
    }
    private void Drift()
    {
        Vector3 forwardVelocity = transform.forward * Vector3.Dot(_rigidbody.velocity, transform.forward);
        Vector3 rightVelocity = transform.right * Vector3.Dot(_rigidbody.velocity, transform.right);

        _rigidbody.velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    private void SetTrailsRenderers(bool screeching)
    {
        foreach (var trail in trailsRenderer)
        {
            if (trail != null)
            {
                trail.emitting = screeching;
            }
        }
        
        foreach (var particle in particleSystem)
        {
            if (particle != null)
            {
                if (screeching)
                {
                    particle.Play();
                }
                else
                {
                    particle.Stop();
                }
            }
        }
    }

    private bool IsTireScreeching(out float lateralVelocity, out bool isBraking)
    {
        lateralVelocity = Vector3.Dot(_rigidbody.velocity, transform.right);
        isBraking = false;

        if (_accelerationInput < 0 && Vector3.Dot(_rigidbody.velocity, transform.forward) > 0f)
        {
            isBraking = true;
            return true;
        }

        if (Mathf.Abs(lateralVelocity) > 3f)
        {
            return true;
        }

        return false;
    }
}