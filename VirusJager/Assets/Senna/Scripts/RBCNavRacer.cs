using UnityEngine;
using UnityEngine.AI;

public class RBCNavRacer : MonoBehaviour
{
    public Transform endPoint;

    [Header("Speed")]
    public float minSpeed = 2f;
    public float maxSpeed = 5f;

    [Header("Rotation")]
    public float turnMultiplier = 1f;

    [Header("Slope Following")]
    public float raycastDistance = 1f;
    public LayerMask groundLayer = 1;
    public float tiltSmoothing = 8f;

    // === NEW: Smart Speed Regulation ===
    [Header("Smart Speed (Straights vs Turns)")]
    [Tooltip("How far ahead the racer looks to see if there's a turn coming")]
    public float lookAheadDistance = 8f;

    [Tooltip("At what angle (degrees) the racer starts slowing down (30-60 typical)")]
    public float turnThresholdAngle = 45f;

    [Tooltip("How much slower in sharp turns (0.6 = 60% of normal speed)")]
    [Range(0.4f, 1f)] public float turnSpeedMultiplier = 0.7f;

    [Tooltip("How much faster on long straights (1.3 = +30% speed)")]
    [Range(1f, 1.8f)] public float straightSpeedMultiplier = 1.3f;

    private NavMeshAgent agent;
    private float baseSpeed;
    private Vector3 smoothedUp = Vector3.up;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.angularSpeed = 0f;

        baseSpeed = Random.Range(minSpeed, maxSpeed);
        agent.speed = baseSpeed;
        agent.acceleration = baseSpeed * 2f;
        agent.stoppingDistance = 0.1f;

        if (endPoint != null)
        {
            // SMALL RANDOM OFFSET SO THEY DONT FOLLOW THE SAME LINE
            Vector3 randomOffset = new Vector3(
                Random.Range(-1.5f, 1.5f),
                0f,
                Random.Range(-1.5f, 1.5f)
            );

            Vector3 randomizedEndPos = endPoint.position + randomOffset;

            agent.SetDestination(randomizedEndPos);
        }
    }


    void Update()
    {
        // This runs every frame and adjusts speed based on upcoming path
        AdjustSpeedBasedOnPath();
    }

    void LateUpdate()
    {
        if (agent.velocity.magnitude > 0.1f)
        {
            Vector3 forward = agent.velocity.normalized;

            RaycastHit hit;
            Vector3 upDir = Vector3.up;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, raycastDistance, groundLayer))
            {
                upDir = hit.normal;
            }

            smoothedUp = Vector3.Lerp(smoothedUp, upDir, tiltSmoothing * Time.deltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(forward, smoothedUp);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, agent.speed * turnMultiplier * Time.deltaTime);
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            Destroy(gameObject);
    }

    // NEW: Smart speed control
    void AdjustSpeedBasedOnPath()
    {
        if (!agent.hasPath || agent.path.corners.Length < 2)
        {
            agent.speed = Mathf.Lerp(agent.speed, baseSpeed * straightSpeedMultiplier, Time.deltaTime * 5f);
            return;
        }

        Vector3 currentPos = transform.position;
        Vector3 currentDir = agent.velocity.sqrMagnitude > 0.01f ? agent.velocity.normalized : transform.forward;

        // Look ahead along the path
        Vector3 futurePoint = GetPointAlongPath(lookAheadDistance);
        Vector3 futureDir = (futurePoint - currentPos).normalized;

        float angleToFuture = Vector3.Angle(currentDir, futureDir);

        float targetSpeed;

        if (angleToFuture > turnThresholdAngle)
        {
            // Sharp turn coming → slow down
            targetSpeed = baseSpeed * turnSpeedMultiplier;
        }
        else
        {
            // Straight or gentle curve → speed up!
            targetSpeed = baseSpeed * straightSpeedMultiplier;
        }

        // Smoothly apply the new speed
        agent.speed = Mathf.Lerp(agent.speed, targetSpeed, Time.deltaTime * 6f);
    }

    // Helper: find point X units ahead along NavMesh path
    Vector3 GetPointAlongPath(float distance)
    {
        var corners = agent.path.corners;
        float distSoFar = 0f;

        for (int i = 0; i < corners.Length - 1; i++)
        {
            Vector3 a = corners[i];
            Vector3 b = corners[i + 1];
            float segmentLength = Vector3.Distance(a, b);

            if (distSoFar + segmentLength >= distance)
            {
                float t = (distance - distSoFar) / segmentLength;
                return Vector3.Lerp(a, b, t);
            }
            distSoFar += segmentLength;
        }

        // If we're near the end, just use last corner or endpoint
        return corners.Length > 0 ? corners[corners.Length - 1] : endPoint.position;
    }
}