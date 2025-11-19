using UnityEngine;

public class Vibration : MonoBehaviour
{
    public int vibrationStrength = 80;
    public string triggerTag = "Obstacle";

    private bool isInsideTrigger = false;

    void Update()
    {
        if (!isInsideTrigger)
            LogitechDriver.StopDirt();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(triggerTag)) return;
        if (!LogitechDriver.IsConnected()) return;

        isInsideTrigger = true;
        LogitechDriver.PlayDirt(vibrationStrength);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(triggerTag)) return;

        isInsideTrigger = false;
    }
}
