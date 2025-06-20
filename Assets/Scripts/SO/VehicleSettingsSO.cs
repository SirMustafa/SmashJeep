using UnityEngine;

[CreateAssetMenu(fileName = "CarSettings")]
public class VehicleSettingsSO : ScriptableObject
{
    [Header("Paddings")]
    [SerializeField] float _wheelPaddingX;
    [SerializeField] float _wheelPaddingZ;

    public float WheelPaddingX => _wheelPaddingX;
    public float WheelPaddingZ => _wheelPaddingZ;
}