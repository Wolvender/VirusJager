using UnityEngine;
using System.Runtime.InteropServices;

public class G29CollisionVibration : MonoBehaviour
{
    [Header("Collision Vibration Settings")]
    [Range(0, 100)]
    public int vibrationStrength = 80;
    public string triggerTag = "Obstacle";

    private bool isInsideTrigger = false;

    // Logitech SDK imports
    [DllImport("LogitechSteeringWheelEnginesWrapper")]
    private static extern bool LogiIsConnected(int index);

    [DllImport("LogitechSteeringWheelEnginesWrapper")]
    private static extern bool LogiUpdate();

    // --- Dirt Road effect ---
    [DllImport("LogitechSteeringWheelEnginesWrapper")]
    private static extern void LogiPlayDirtRoadEffect(int index, int magnitude);

    [DllImport("LogitechSteeringWheelEnginesWrapper")]
    private static extern void LogiStopDirtRoadEffect(int index);


    void Update()
    {
        LogiUpdate();

        // stop effect als auto niet meer in trigger is
        if (!isInsideTrigger)
        {
            LogiStopDirtRoadEffect(0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!LogiIsConnected(0)) return;
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
