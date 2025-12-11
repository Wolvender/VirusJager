using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerCelebration : MonoBehaviour
{
    [Header("Celebration Settings")]
    public Transform celebrationPoint;           // Drag the empty/point in scene here
    public Animator animator;                    // Drag player's Animator here
    public MonoBehaviour[] scriptsToDisable;     // Drag PlayerMovement, AI scripts, etc.

    private bool celebrationIsRunning = false;

    public void StartCelebration()
    {
        if (celebrationIsRunning) return;
        celebrationIsRunning = true;

        if (celebrationPoint == null)
        {
            Debug.LogError("Celebration Point not assigned on " + name);
            celebrationIsRunning = false;
            return;
        }

        // Disable movement
        if (scriptsToDisable != null)
        {
            foreach (var script in scriptsToDisable)
                if (script != null) script.enabled = false;
        }

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Snap to exact position/rotation
        transform.position = celebrationPoint.position;
        transform.rotation = celebrationPoint.rotation;

        // Lock position forever until celebration ends
        StartCoroutine(LockTransformDuringCelebration());

        // Play win animation
        if (animator != null)
            animator.SetBool("Win", true);
    }

    private IEnumerator LockTransformDuringCelebration()
    {
        Vector3 pos = celebrationPoint.position;
        Quaternion rot = celebrationPoint.rotation;

        while (celebrationIsRunning)
        {
            transform.position = pos;
            transform.rotation = rot;
            yield return null;
        }
    }

    // Call this from an Animation Event at the end of the win clip, or via timer
    public void EndCelebration()
    {
        celebrationIsRunning = false;

        if (scriptsToDisable != null)
        {
            foreach (var script in scriptsToDisable)
                if (script != null) script.enabled = true;
        }

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = true;

        if (animator != null)
            animator.SetBool("Win", false);
    }
}