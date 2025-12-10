using UnityEngine;

public class PlayerCelebration : MonoBehaviour
{
    public Transform celebrationPoint;        // Drag your celebration spot here
    public Animator animator;            // Drag player's Animator here

    private bool celebrationIsRunning = false;

    public void StartCelebration()
    {
        if (celebrationIsRunning) return;
        celebrationIsRunning = true;

        if (celebrationPoint == null)
        {
            Debug.LogError("Celebration Point not assigned!");
            return;
        }

        // Instant teleport
        transform.position = celebrationPoint.position;
        transform.rotation = celebrationPoint.rotation; // optional: also match rotation

        Debug.Log("Player teleported to celebration point!");

        if (animator != null)
            animator.SetBool("Win", true);
    }
}