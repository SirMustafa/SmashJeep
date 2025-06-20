using UnityEngine;

[CreateAssetMenu(fileName = "CarSettings")]
public class VehicleSettingsSO : ScriptableObject
{
    [Header("Paddings")]
    [SerializeField] float _wheelPaddingX;
    [SerializeField] float _wheelPaddingZ;

    [Header("Suspansion")]
    [SerializeField] float _sprintRestLength;
    [SerializeField] float _sprintStrength;
    [SerializeField] float _sprintDamper;

    [Header("Handling")]
    [SerializeField] float _steerAngle;
    [SerializeField] float _frontWheelsGripFactor;
    [SerializeField] float _backWheelsGripFactor;

    [Header("Body")]
    [SerializeField] float _tireMass;
    

    [Header("Power")]
    [SerializeField] float _acceleratePower;
    [SerializeField] float _maxSpeed;
    [SerializeField] float _maxReverseSpeed;

    public float WheelPaddingX => _wheelPaddingX;
    public float WheelPaddingZ => _wheelPaddingZ;
    public float SpringRestLength => _sprintRestLength;
    public float SpringStrength => _sprintStrength;
    public float SpringDamper => _sprintDamper;
    public float SteerAngle => _steerAngle;
    public float FrontWheelGripFactor => _frontWheelsGripFactor;
    public float BackWheelGripFactor => _backWheelsGripFactor;
    public float TireMass => _tireMass;
    public float AcceleratePower => _acceleratePower;
    public float MaxSpeed => _maxSpeed;
    public float MaxReverseSpeed => _maxReverseSpeed;
}