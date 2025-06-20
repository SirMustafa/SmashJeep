using UnityEngine;

public class FpsLimiter : MonoBehaviour
{
    [SerializeField] bool isLimitting;
    [SerializeField] int desiredFps;
    void Start()
    {
        if (isLimitting)
        {
            Application.targetFrameRate = desiredFps;
        }
    }
}