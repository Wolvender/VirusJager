using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class ResetRigOnStart : MonoBehaviour
{
    public Transform xrOrigin; // The XR Origin or Camera Rig

    private XRDisplaySubsystem display;

    void Start()
    {
        // Find XR Display subsystem
        List<XRDisplaySubsystem> displays = new List<XRDisplaySubsystem>();
        SubsystemManager.GetSubsystems(displays);

        if (displays.Count > 0)
            display = displays[0];

        // Delay one frame so tracking has initialized
        StartCoroutine(ResetNextFrame());
    }

    private System.Collections.IEnumerator ResetNextFrame()
    {
        yield return null; // wait 1 frame

        if (display != null && display.running)
            ResetDirection();
    }

    private void ResetDirection()
    {
        // Get current head rotation
        Quaternion headRot = InputTracking.GetLocalRotation(XRNode.CenterEye);

        // Only align yaw (horizontal rotation)
        float yaw = headRot.eulerAngles.y;

        // Rotate the root opposite
        transform.rotation = Quaternion.Euler(0, -yaw, 0);
    }
}
