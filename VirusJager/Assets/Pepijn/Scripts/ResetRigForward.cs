using UnityEngine;
using UnityEngine.XR;

// This works with OpenXR, Oculus, SteamVR, Pico, etc. – no XR Interaction Toolkit required!
public class ResetRigForward : MonoBehaviour
{
    [Header("Leave empty to auto-detect")]
    public Transform rigRoot;     // Usually your "XR Origin" or "XR Rig"
    public Transform headCamera;  // Usually the Main Camera

    [Tooltip("Delay in seconds to wait for tracking to stabilize")]
    public float delay = 0.5f;

    void Start()
    {
        // Auto-find if not assigned
        if (rigRoot == null)
        {
            // Most common names Unity uses
            rigRoot = GameObject.Find("XR Origin")?.transform
                   ?? GameObject.Find("XR Rig")?.transform
                   ?? GameObject.Find("XROrigin")?.transform
                   ?? transform; // fallback to this object
        }

        if (headCamera == null)
        {
            headCamera = Camera.main?.transform;
        }

        Invoke(nameof(Recenter), delay);
    }

    void Recenter()
    {
        if (headCamera == null)
        {
            Debug.LogError("[ResetRigForward] Could not find head camera!");
            return;
        }

        // Get only the yaw (Y rotation) of the player's head
        float headYaw = headCamera.rotation.eulerAngles.y;

        // Cancel yaw by rotating the rig the opposite direction
        Quaternion cancelYaw = Quaternion.Euler(0f, -headYaw, 0f);
        rigRoot.rotation = cancelYaw * rigRoot.rotation;

        // Optional: Snap player back to X=0, Z=0 on the floor (keeps real-world height)
        Vector3 pos = rigRoot.position;
        pos.x = 0f;
        pos.z = 0f;
        rigRoot.position = pos;

        Debug.Log($"[ResetRigForward] Done! Player now faces +Z. Head yaw was {headYaw:F1} degrees.");
    }
}