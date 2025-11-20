using UnityEngine;

public class ForceFeedback : MonoBehaviour
{
    void Update()
    {
        if (!LogitechDriver.IsConnected()) return;

        LogitechDriver.PlaySpring(0, 50, 50);
    }

    void OnDisable()
    {
        LogitechDriver.StopSpring();
    }
}
