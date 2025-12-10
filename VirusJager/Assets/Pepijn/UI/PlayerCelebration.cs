using UnityEngine;
using System.Collections;

public class PlayerCelebration : MonoBehaviour
{
    [Header("References")]
    public Transform celebrationPoint;
    public Animator animator;

    [Header("Movement")]
    public float moveSpeed = 3f;

    private bool celebrationIsRunning = false;

    public void StartCelebration()
    {
        if (celebrationIsRunning) return;
        celebrationIsRunning = true;

        Debug.Log("Celebration started");

        // If you have a movement script, disable it here:
        // GetComponent<PlayerMovement>().enabled = false;

        StartCoroutine(TeleportRoutine());

        if (animator != null)
            animator.SetBool("Win", true);
    }

    private IEnumerator TeleportRoutine()
    {
        if (celebrationPoint == null)
        {
            Debug.LogError("NO Celebration Point Assigned!");
            yield break;
        }

        // Move until close enough
        while (Vector3.Distance(transform.position, celebrationPoint.position) > 0.05f)
        {
            transform.position = Vector3.MoveTowards
            (
                transform.position,
                celebrationPoint.position,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        // Snap to exact final point
        transform.position = celebrationPoint.position;
        Debug.Log("Arrived at celebration position");
    }

}
