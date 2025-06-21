using System.Collections.Generic;
using UnityEngine;

public class VehicleVisualController : MonoBehaviour
{
    [SerializeField] private VehicleController _playerVehicleController;
    [SerializeField] private Transform _wheelFrontLeft, _wheelFrontRight, _wheelBackLeft, _wheelBackRight;
    [SerializeField] private float _wheelSpinSpeed, _wheelYSpringMin, _wheelYSpringMax;

    private Quaternion _wheelFrontLeftRoll;
    private Quaternion _wheelFrontRightRoll;
    private float _springRestLength;
    private float _forwardSpeed;
    private float _steerInput;
    private float _steerAngle;

    private Dictionary<WheelType, float> _springCurrentLength = new()
    {
        { WheelType.FrontLeft, 0 },
        { WheelType.FrontRight, 0 },
        { WheelType.BackLeft, 0 },
        { WheelType.BackRight, 0 }
    };

    private void Start()
    {
        _wheelFrontLeftRoll = _wheelFrontLeft.localRotation;
        _wheelFrontRightRoll = _wheelFrontRight.localRotation;

        _springRestLength = _playerVehicleController.Settings.SpringRestLength;
        _steerAngle = _playerVehicleController.Settings.SteerAngle;
    }

    private void Update()
    {
        UpdateVisualState();
        RotateWheels();
        SetSuspansion();
    }

    private void UpdateVisualState()
    {
        _steerInput = Input.GetAxis("Horizontal");
        _forwardSpeed = Vector3.Dot(_playerVehicleController.Forward, _playerVehicleController.Velocity);

        _springCurrentLength[WheelType.FrontLeft] = _playerVehicleController.GetSpringCurrentLength(WheelType.FrontLeft);
        _springCurrentLength[WheelType.FrontRight] = _playerVehicleController.GetSpringCurrentLength(WheelType.FrontRight);
        _springCurrentLength[WheelType.BackLeft] = _playerVehicleController.GetSpringCurrentLength(WheelType.BackLeft);
        _springCurrentLength[WheelType.BackRight] = _playerVehicleController.GetSpringCurrentLength(WheelType.BackRight);
    }

    private void RotateWheels()
    {
        if (_springCurrentLength[WheelType.FrontLeft] < _springRestLength)
        {
            _wheelFrontLeftRoll *= Quaternion.AngleAxis(_forwardSpeed * _wheelSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if (_springCurrentLength[WheelType.FrontRight] < _springRestLength)
        {
            _wheelFrontRightRoll *= Quaternion.AngleAxis(_forwardSpeed * _wheelSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if (_springCurrentLength[WheelType.BackLeft] < _springRestLength)
        {
            _wheelBackLeft.localRotation *= Quaternion.AngleAxis(_forwardSpeed * _wheelSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if (_springCurrentLength[WheelType.BackRight] < _springRestLength)
        {
            _wheelBackRight.localRotation *= Quaternion.AngleAxis(_forwardSpeed * _wheelSpinSpeed * Time.deltaTime, Vector3.right);
        }

        _wheelFrontLeft.localRotation = Quaternion.AngleAxis(_steerInput * _steerAngle, Vector3.up) * _wheelFrontLeftRoll;
        _wheelFrontRight.localRotation = Quaternion.AngleAxis(_steerInput * _steerAngle, Vector3.up) * _wheelFrontRightRoll;
    }

    private void SetSuspansion()
    {
        float springFrontLeftRatio = _springCurrentLength[WheelType.FrontLeft] / _springRestLength;
        float springFrontRightRatio = _springCurrentLength[WheelType.FrontRight] / _springRestLength;
        float springBackLeftRatio = _springCurrentLength[WheelType.BackLeft] / _springRestLength;
        float springBackRightRatio = _springCurrentLength[WheelType.BackRight] / _springRestLength;

        _wheelBackLeft.localPosition = new Vector3(_wheelBackLeft.localPosition.x, _wheelYSpringMin + (_wheelYSpringMax - _wheelYSpringMin) * springBackLeftRatio, _wheelBackLeft.localPosition.z);
        _wheelBackRight.localPosition = new Vector3(_wheelBackRight.localPosition.x, _wheelYSpringMin + (_wheelYSpringMax - _wheelYSpringMin) * springBackRightRatio, _wheelBackRight.localPosition.z);
        _wheelFrontLeft.localPosition = new Vector3(_wheelFrontLeft.localPosition.x, _wheelYSpringMin + (_wheelYSpringMax - _wheelYSpringMin) * springFrontLeftRatio, _wheelFrontLeft.localPosition.z);
        _wheelFrontRight.localPosition = new Vector3(_wheelFrontRight.localPosition.x, _wheelYSpringMin + (_wheelYSpringMax - _wheelYSpringMin) * springFrontRightRatio, _wheelFrontRight.localPosition.z);
    }
}