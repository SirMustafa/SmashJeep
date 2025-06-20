using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] VehicleSettingsSO _vehicleSettings;

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
    private void FixedUpdate()
    {
        UpdateSuspansions();
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
        foreach (WheelTypes id in _springDatas.Keys)
        {

        }
    }

    private void CastSpring(WheelTypes wheelType)
    {

    }

    private Vector3 GetSpringPosition(WheelTypes wheelType)
    {
        return transform.localToWorldMatrix.MultiplyPoint3x4(GetSpringRelativePosition(wheelType));
    }

    private Vector3 GetSpringRelativePosition(WheelTypes wheelType)
    {
        Vector3 boxSize = _vehicleCollider.size;
        float boxBottom = boxSize.y * -0.5f;

        float paddingX = _vehicleSettings.WheelPaddingX;
        float paddingZ = _vehicleSettings.WheelPaddingZ;

        return wheelType switch
        {
            WheelTypes.FrontLeft => new Vector3(boxSize.x * (paddingX - 0.5f), boxBottom, boxSize.z * (0.5f - paddingZ)),
            WheelTypes.FrontRight => new Vector3(boxSize.x * (0.5f - paddingX), boxBottom, boxSize.z * (0.5f - paddingZ)),
            WheelTypes.BackLeft => new Vector3(boxSize.x * (paddingX - 0.5f), boxBottom, boxSize.z * (paddingZ - 0.5f)),
            WheelTypes.BackRight => new Vector3(boxSize.x * (0.5f - paddingX), boxBottom, boxSize.z * (paddingZ - 0.5f)),

            _ => default
        };
    }
}