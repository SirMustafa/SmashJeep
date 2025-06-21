using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public class SpringData
    {
        public float _currentLength;
        public float _currentVelocity;
    }

    private static readonly WheelType[] _wheels = new WheelType[]
    {
        WheelType.FrontLeft, WheelType.FrontRight, WheelType.BackLeft, WheelType.BackRight,
    };

    private static readonly WheelType[] _backWheels = new WheelType[]
    {
        WheelType.BackLeft, WheelType.BackRight,
    };

    [Header("References")]
    [SerializeField] Rigidbody _rb;
    [SerializeField] BoxCollider _vehicleCollider;
    [SerializeField] VehicleSettingsSO _vehicleSettings;

    private Dictionary<WheelType, SpringData> _springDatas;

    private float _steeringInput;
    private float _accelerationInput;

    public Vector3 Velocity => _rb.linearVelocity;
    public Vector3 Forward => transform.forward;
    public VehicleSettingsSO Settings => _vehicleSettings;

    private void Awake()
    {
        _springDatas = new Dictionary<WheelType, SpringData>();

        foreach (WheelType wheelType in _wheels)
        {
            _springDatas.Add(wheelType, new SpringData());
        }
    }

    private void Update()
    {
        SetAccelerationInput(Input.GetAxis("Vertical"));
        SetSteeringInput(Input.GetAxis("Horizontal"));
    }
    private void FixedUpdate()
    {
        UpdateSuspansions();
        UpdateSteering();
        UpdateAcceleration();
        UpdateBreakes();
        UpdateAirResistance();
    }

    void SetSteeringInput(float steering)
    {
        _steeringInput = Mathf.Clamp(steering, -1, 1);
    }
    void SetAccelerationInput(float acceleration)
    {
        _accelerationInput = Mathf.Clamp(acceleration, -1, 1);
    }

    private void UpdateSuspansions()
    {
        foreach (WheelType id in _springDatas.Keys)
        {
            CastSpring(id);

            float currentVelocity = _springDatas[id]._currentVelocity;
            float currentLength = _springDatas[id]._currentLength;

            float force = SpringMathExtensions.CalculateForceDamped(currentLength, currentVelocity, _vehicleSettings.SpringRestLength, _vehicleSettings.SpringStrength, _vehicleSettings.SpringDamper);

            _rb.AddForceAtPosition(force * transform.up, GetSpringPosition(id));
        }
    }

    private void UpdateSteering()
    {
        foreach (WheelType wheelType in _wheels)
        {
            if (!IsGrounded(wheelType))
            {
                continue;
            }

            Vector3 springPosition = GetSpringPosition(wheelType);
            Vector3 slideDirection = GetWheelSlideDirection(wheelType);

            float slideVelocity = Vector3.Dot(slideDirection, _rb.GetPointVelocity(springPosition));
            float desiredVelocityChange = GetWheelGripFactor(wheelType) * -slideVelocity;
            float desiredAcceleration = desiredVelocityChange / Time.fixedDeltaTime;

            Vector3 force = desiredAcceleration * slideDirection * _vehicleSettings.TireMass;
            _rb.AddForceAtPosition(force, GetWheelTorquePosition(wheelType));
        }
    }

    private void UpdateAcceleration()
    {
        if (Mathf.Approximately(_accelerationInput, 0f))
        {
            return;
        }

        float forwardSpeed = Vector3.Dot(transform.forward, _rb.linearVelocity);
        bool isMovingForward = forwardSpeed > 0;
        float speed = Mathf.Abs(forwardSpeed);

        if (isMovingForward && speed > _vehicleSettings.MaxSpeed)
        {
            return;
        }
        else if (!isMovingForward && speed > _vehicleSettings.MaxReverseSpeed)
        {
            return;
        }

        foreach (WheelType wheelType in _wheels)
        {
            if (!IsGrounded(wheelType))
            {
                continue;
            }

            Vector3 position = GetWheelTorquePosition(wheelType);
            Vector3 wheelForward = GetWheelRollDirection(wheelType);

            _rb.AddForceAtPosition(_accelerationInput * wheelForward * _vehicleSettings.AcceleratePower, position);
        }
    }

    private void UpdateBreakes()
    {
        float forwardSpeed = Vector3.Dot(transform.forward, _rb.linearVelocity);
        float speed = Mathf.Abs(forwardSpeed);
        float breaksRatio;

        const float ALMOST_STOPPING_SPEED = 2f;
        bool almostStopping = speed > ALMOST_STOPPING_SPEED;

        if (almostStopping)
        {
            breaksRatio = 1f;
        }
        else
        {
            bool accelerateContrary = Mathf.Approximately(_accelerationInput, 0f) && Vector3.Dot(_accelerationInput * transform.forward, _rb.linearVelocity) < 0f;

            if (accelerateContrary)
            {
                breaksRatio = 1f;
            }
            else if (Mathf.Approximately(_accelerationInput, 0f))
            {
                breaksRatio = 0.1f;
            }
            else
            {
                return;
            }
        }

        foreach (WheelType wheelType in _backWheels)
        {
            if (!IsGrounded(wheelType))
            {
                continue;
            }

            Vector3 springPosition = GetSpringPosition(wheelType);
            Vector3 rollDirection = GetWheelRollDirection(wheelType);
            float rollVelocity = Vector3.Dot(rollDirection, _rb.GetPointVelocity(springPosition));

            float desiredVelocityChange = -rollVelocity * breaksRatio * _vehicleSettings.BreakesPower;
            float desiredAcceleration = desiredVelocityChange / Time.fixedDeltaTime;
            Vector3 force = desiredAcceleration * _vehicleSettings.TireMass * rollDirection;
            _rb.AddForceAtPosition(force, GetWheelTorquePosition(wheelType));
        }
    }

    private void UpdateAirResistance()
    {
        _rb.AddForce(_vehicleCollider.size.magnitude * -(_rb.linearVelocity) * _vehicleSettings.AirResistance);
    }

    private void CastSpring(WheelType wheelType)
    {
        Vector3 position = GetSpringPosition(wheelType);
        float previousLength = _springDatas[wheelType]._currentLength;
        float currentLength;

        if (Physics.Raycast(position, -transform.up, out var hit, _vehicleSettings.SpringRestLength))
        {
            currentLength = hit.distance;
        }
        else
        {
            currentLength = _vehicleSettings.SpringRestLength;
        }

        _springDatas[wheelType]._currentVelocity = (currentLength - previousLength) / Time.fixedDeltaTime;
        _springDatas[wheelType]._currentLength = currentLength;
    }

    private Vector3 GetSpringPosition(WheelType wheelType)
    {
        return transform.localToWorldMatrix.MultiplyPoint3x4(GetSpringRelativePosition(wheelType));
    }

    private Vector3 GetSpringRelativePosition(WheelType wheelType)
    {
        Vector3 boxSize = _vehicleCollider.size;
        float boxBottom = boxSize.y * -0.5f;

        float paddingX = _vehicleSettings.WheelPaddingX;
        float paddingZ = _vehicleSettings.WheelPaddingZ;

        return wheelType switch
        {
            WheelType.FrontLeft => new Vector3(boxSize.x * (paddingX - 0.5f), boxBottom, boxSize.z * (0.5f - paddingZ)),
            WheelType.FrontRight => new Vector3(boxSize.x * (0.5f - paddingX), boxBottom, boxSize.z * (0.5f - paddingZ)),
            WheelType.BackLeft => new Vector3(boxSize.x * (paddingX - 0.5f), boxBottom, boxSize.z * (paddingZ - 0.5f)),
            WheelType.BackRight => new Vector3(boxSize.x * (0.5f - paddingX), boxBottom, boxSize.z * (paddingZ - 0.5f)),

            _ => default
        };
    }

    private Vector3 GetWheelSlideDirection(WheelType wheelType)
    {
        Vector3 forward = GetWheelRollDirection(wheelType);
        return Vector3.Cross(transform.up, forward);
    }

    private Vector3 GetWheelRollDirection(WheelType wheelType)
    {
        bool frontWheels = wheelType == WheelType.FrontLeft || wheelType == WheelType.FrontRight;

        if (frontWheels)
        {
            var steerQuaternation = Quaternion.AngleAxis(_steeringInput * _vehicleSettings.SteerAngle, Vector3.up);
            return steerQuaternation * transform.forward;
        }
        else
        {
            return transform.forward;
        }
    }

    private float GetWheelGripFactor(WheelType wheelType)
    {
        bool frontWheels = wheelType == WheelType.FrontLeft || wheelType == WheelType.FrontRight;
        return frontWheels ? _vehicleSettings.FrontWheelGripFactor : _vehicleSettings.BackWheelGripFactor;
    }

    private Vector3 GetWheelTorquePosition(WheelType wheelType)
    {
        return transform.localToWorldMatrix.MultiplyPoint3x4(GetWheelRelativeTorquePosition(wheelType));
    }

    private Vector3 GetWheelRelativeTorquePosition(WheelType wheelType)
    {
        Vector3 boxSize = _vehicleCollider.size;

        float paddingX = _vehicleSettings.WheelPaddingX;
        float paddingZ = _vehicleSettings.WheelPaddingZ;

        return wheelType switch
        {
            WheelType.FrontLeft => new Vector3(boxSize.x * (paddingX - 0.5f), 0f, boxSize.z * (0.5f - paddingZ)),
            WheelType.FrontRight => new Vector3(boxSize.x * (0.5f - paddingX), 0f, boxSize.z * (0.5f - paddingZ)),
            WheelType.BackLeft => new Vector3(boxSize.x * (paddingX - 0.5f), 0f, boxSize.z * (paddingZ - 0.5f)),
            WheelType.BackRight => new Vector3(boxSize.x * (0.5f - paddingX), 0f, boxSize.z * (paddingZ - 0.5f)),

            _ => default
        };
    }

    private bool IsGrounded(WheelType wheelType)
    {
        return _springDatas[wheelType]._currentLength < _vehicleSettings.SpringRestLength;
    }

    public float GetSpringCurrentLength(WheelType wheelType)
    {
        return _springDatas[wheelType]._currentLength;
    }
}

public static class SpringMathExtensions
{
    public static float CalculateForceDamped(float currentLength, float lengthVelocity, float restLength, float strength, float damper)
    {
        float lengthOffset = restLength - currentLength;
        return (lengthOffset * strength) - (damper * lengthVelocity);
    }
}