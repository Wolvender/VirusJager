using UnityEngine;

[RequireComponent(typeof(CharacterController))] // Optional: remove if you don't use CharacterController
public class PlayerCelebration : MonoBehaviour
{
    [Header("Celebration Settings")]
    public Transform celebrationPoint;      // Assign in Inspector
    public Animator animator;                // Assign in Inspector

    [Tooltip("Disable movement scripts during celebration to prevent interference")]
    public MonoBehaviour[] scriptsToDisable; // Drag PlayerMovement, CharacterController script, etc.

    private bool celebrationIsRunning = false;

    public void StartCelebration()
    {
        if (celebrationIsRunning) return;
        celebrationIsRunning = true;

        if (celebrationPoint == null)
        {
            Debug.LogError("Celebration Point not assigned in " + gameObject.name);
            celebrationIsRunning = false;
            return;
        }

        // === 1. Disable movement scripts to prevent them from moving the player ===
        if (scriptsToDisable != null)
        {
            foreach (var script in scriptsToDisable)
            {
                if (script != null)
                    script.enabled = false;
            }
        }

        // === 2. If using CharacterController, disable it properly ===
        var cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // === 3. Force exact position and rotation ===
        transform.position = celebrationPoint.position;
        transform.rotation = celebrationPoint.rotation;

        // === 4. Optional: Lock position completely using a coroutine (recommended!) ===
        StartCoroutine(LockTransformDuringCelebration());

        // === 5. Trigger win animation ===
        if (animator != null)
        {
            animator.SetBool("Win", true);
            // Optional: make sure root motion is OFF for the win state or clip
            // (or disable Apply Root Motion in Animator if needed)
        }

        Debug.Log("Player celebrated at exact celebration point!");
    }

    private System.Collections.IEnumerator LockTransformDuringCelebration()
    {
        // This keeps the player locked in place even if something tries to move it
        Vector3 targetPos = celebrationPoint.position;
        Quaternion targetRot = celebrationPoint.rotation;

        while (celebrationIsRunning)
        {
            transform.position = targetPos;
            transform.rotation = targetRot;
            yield return null; // Wait one frame
        }
    }

    // Optional: Call this when celebration ends (e.g. from animation event or timer)
    public void EndCelebration()
    {
        celebrationIsRunning = false;

        // Re-enable movement scripts
        if (scriptsToDisable != null)
        {
            foreach (var script in scriptsToDisable)
            {
                if (script != null)
                    script.enabled = true;
            }
        }

        var cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = true;

        if (animator != null)
            animator.SetBool("Win", false);
    }
}   