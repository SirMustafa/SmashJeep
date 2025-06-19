using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleController : MonoBehaviour
{
    public class SpringData
    {
        public float _currentLength;
        public float _currentVelocity;
    }

    private static readonly WheelTypes[] _wheels = new WheelTypes[]
    {
        WheelTypes.FrontLeft, WheelTypes.FrontRight, WheelTypes.BackLeft, WheelTypes.BackRight,
    };

    [Header("References")]
    [SerializeField] Rigidbody _rb;
    [SerializeField] BoxCollider _vehicleCollider;

    private Dictionary<WheelTypes, SpringData> _springDatas;

    private float _steeringInput;
    private float _accelerationInput;

    private void Awake()
    {
        _springDatas = new Dictionary<WheelTypes, SpringData>();

        foreach (WheelTypes wheelType in _wheels)
        {
            _springDatas.Add(wheelType, new SpringData());
        }
    }

    private void Update()
    {
        SetAccelerationInput(Input.GetAxis("Vertical"));
        SetSteeringInput(Input.GetAxis("Horizontal"));
    }

    void SetSteeringInput(float steering)
    {
        _steeringInput = Mathf.Clamp(steering, -1, 1);
    }
    void SetAccelerationInput(float acceleration)
    {
        _accelerationInput = Mathf.Clamp(acceleration, -1, 1);
    }
}