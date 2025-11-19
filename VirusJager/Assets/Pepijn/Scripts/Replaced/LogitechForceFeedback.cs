using UnityEngine;
using System.Runtime.InteropServices;

public class LogitechForceFeedback : MonoBehaviour
{
    [DllImport("LogitechSteeringWheelEnginesWrapper")]
    private static extern bool LogiSteeringInitialize(bool ignoreXInputControllers);

    [DllImport("LogitechSteeringWheelEnginesWrapper")]
    private static extern void LogiSteeringShutdown();

    [DllImport("LogitechSteeringWheelEnginesWrapper")]
    private static extern bool LogiUpdate();

    [DllImport("LogitechSteeringWheelEnginesWrapper")]
    private static extern bool LogiIsConnected(int index);

    [DllImport("LogitechSteeringWheelEnginesWrapper")]
    private static extern void LogiPlaySpringForce(int index, int offsetPercentage, int saturationPercentage, int coefficientPercentage);

    [DllImport("LogitechSteeringWheelEnginesWrapper")]
    private static extern void LogiStopSpringForce(int index);

    private bool initialized = false;
    private bool springActive = false;

    void OnEnable()
    {
        // Initialize once per play session
        initialized = LogiSteeringInitialize(false);
        Debug.Log("Logitech init: " + initialized);
    }

    void Update()
    {
        if (!initialized)
            return;

        LogiUpdate();

        if (LogiIsConnected(0))
        {
            // Start spring *once*
            if (!springActive)
            {
                LogiPlaySpringForce(0, 0, 50, 50);
                springActive = true;
            }
        }
        else
        {
            // If disconnected, stop spring safely
            if (springActive)
            {
                LogiStopSpringForce(0);
                springActive = false;
            }
        }
    }

    void OnDisable()
    {
        Debug.Log("Logitech shutting down...");

        if (initialized)
        {
            if (springActive)
            {
                LogiStopSpringForce(0);
                springActive = false;
            }

            LogiSteeringShutdown();
            initialized = false;
        }
    }
}
