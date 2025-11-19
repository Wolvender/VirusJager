using UnityEngine;
using System.Runtime.InteropServices;

public class LogitechG29Controller : MonoBehaviour
{
    [Header("Spring Force Settings")]
    [Range(-100, 100)]
    public int springOffset = 0;
    [Range(0, 100)]
    public int springSaturation = 50;
    [Range(0, 100)]
    public int springCoefficient = 50;

    [Header("Collision Vibration Settings")]
    [Range(0, 100)]
    public int vibrationStrength = 80;
    public string triggerTag = "Obstacle";

    private bool initialized = false;
    private bool springActive = false;
    private bool isInsideTrigger = false;

    // Logitech SDK imports
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
    [DllImport("LogitechSteeringWheelEnginesWrapper")]
    private static extern void LogiPlayDirtRoadEffect(int index, int magnitude);
    [DllImport("LogitechSteeringWheelEnginesWrapper")]
    private static extern void LogiStopDirtRoadEffect(int index);

    private void OnEnable()
    {
        initialized = LogiSteeringInitialize(false);
        Debug.Log("Logitech initialized: " + initialized);
    }

    private void Update()
    {
        if (!initialized) return;

        LogiUpdate();

        // Spring force
        if (LogiIsConnected(0) && !springActive)
        {
            LogiPlaySpringForce(0, springOffset, springSaturation, springCoefficient);
            springActive = true;
        }

        // Stop vibration als niet meer in trigger
        if (!isInsideTrigger)
            LogiStopDirtRoadEffect(0);
    }

    private void OnDisable()
    {
        if (!initialized) return;

        if (springActive)
        {
            LogiStopSpringForce(0);
            springActive = false;
        }

        LogiStopDirtRoadEffect(0);
        LogiSteeringShutdown();
        initialized = false;
        Debug.Log("Logitech shutdown");
    }

    // Collision triggers
    private void OnTriggerEnter(Collider other)
    {
        if (!initialized || !LogiIsConnected(0)) return;
        if (!other.CompareTag(triggerTag)) return;

        isInsideTrigger = true;
        LogiPlayDirtRoadEffect(0, vibrationStrength);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(triggerTag)) return;
        isInsideTrigger = false;
    }
}
