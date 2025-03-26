using TMPro;
using UnityEngine;

public class CarController2 : MonoBehaviour
{
    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float turnFactor;
    [SerializeField] private TMP_Text speedText;

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
        Turn();

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
}