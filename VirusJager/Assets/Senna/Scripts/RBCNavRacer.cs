using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RBCNavRacer : MonoBehaviour
{
    [Header("=== FINISH ===")]
    public Transform endPoint;                   // Drag your finish here (one per racer or shared)

    [Header("=== SPEED ===")]
    public float minSpeed = 10f;
    public float maxSpeed = 18f;

    [Header("=== HANDLING ===")]
    public float turnMultiplier = 6f;
    public float raycastDistance = 2f;
    public LayerMask groundLayer = 1;
    public float tiltSmoothing = 12f;

    [Header("=== CHAOS & MISTAKES ===")]
    [Range(0f, 1f)] public float mistakeChance = 0.3f;
    public float mistakeDuration = 1.2f;
    public float wobbleAmount = 3f;

    // ──────────────────────
    private NavMeshAgent agent;
    private float baseSpeed;
    private Vector3 smoothedUp = Vector3.up;
    private static List<float> usedSpeeds = new List<float>();

    private float mistakeTimer = 0f;
    private bool isMistakeActive = false;
    private Vector3 mistakeOffset = Vector3.zero;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.angularSpeed = 0f;

        // THIS IS THE MAGIC: They follow the path but still smash into each other
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        agent.avoidancePriority = Random.Range(1, 99);
        agent.autoBraking = false;
        agent.stoppingDistance = 0f;

        baseSpeed = GetUniqueSpeed(minSpeed, maxSpeed);
        agent.speed = baseSpeed;
        agent.acceleration = baseSpeed * 4f;

        // MAIN GOAL: Go to endPoint, but with random offset so they don't all take the exact same path
        if (endPoint != null)
        {
            Vector3 finalDest = endPoint.position + new Vector3(
                Random.Range(-4f, 4f),
                0,
                Random.Range(-4f, 4f)
            );
            agent.SetDestination(finalDest);
        }
    }

    void Update()
    {
        HandleMistakes();

        // Every few seconds, slightly nudge the destination to create overtaking and chaos
        if (Random.value < 0.02f && endPoint != null)
        {
            Vector3 chaoticDest = endPoint.position + new Vector3(
                Random.Range(-5f, 5f),
                0,
                Random.Range(-5f, 5f)
            ) + mistakeOffset;

            agent.SetDestination(chaoticDest);
        }
    }

    void LateUpdate()
    {
        // Manual rotation + ground tilt
        if (agent.velocity.sqrMagnitude > 0.5f)
        {
            Vector3 forward = agent.velocity.normalized;
            if (isMistakeActive) forward += mistakeOffset.normalized * 0.3f;

            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, raycastDistance, groundLayer))
                smoothedUp = Vector3.Lerp(smoothedUp, hit.normal, tiltSmoothing * Time.deltaTime);

            Quaternion targetRot = Quaternion.LookRotation(forward, smoothedUp);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, agent.speed * turnMultiplier * Time.deltaTime);
        }

        // FINISHED
        if (endPoint != null && Vector3.Distance(transform.position, endPoint.position) < 5f)
        {
            Destroy(gameObject);
        }
    }

    void HandleMistakes()
    {
        if (isMistakeActive)
        {
            mistakeTimer -= Time.deltaTime;
            if (mistakeTimer <= 0f)
            {
                isMistakeActive = false;
                mistakeOffset = Vector3.zero;
                agent.speed = baseSpeed;
            }
            return;
        }

        if (Random.value < mistakeChance * Time.deltaTime)
        {
            isMistakeActive = true;
            mistakeTimer = mistakeDuration + Random.Range(-0.5f, 0.8f);

            int type = Random.Range(0, 4);
            switch (type)
            {
                case 0: mistakeOffset = Random.insideUnitSphere * wobbleAmount; break;
                case 1: agent.speed = baseSpeed * 0.4f; break;   // sudden brake
                case 2: agent.speed = baseSpeed * 2f; break;     // panic boost
                case 3: mistakeOffset = Random.insideUnitSphere * wobbleAmount * 2f; break;
            }
        }
    }

    float GetUniqueSpeed(float min, float max)
    {
        float s;
        int safety = 100;
        do { s = Random.Range(min, max); safety--; }
        while (usedSpeeds.Contains(s) && safety > 0);
        if (safety > 0) usedSpeeds.Add(s);
        return s;
    }

    void OnDestroy() => usedSpeeds.Clear();
}