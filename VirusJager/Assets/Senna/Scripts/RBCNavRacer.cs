using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RBCNavRacer : MonoBehaviour
{
    [Header("=== FINISH ===")]
    public Transform endPoint;

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

    [Header("=== WALL & CORNER BEHAVIOR ===")]
    public float stuckDetectionDistance = 0.3f;
    public float stuckTimeThreshold = 0.8f;
    public float pushAwayForce = 14f;
    public float extraAvoidanceRadius = 0.35f;

    // ──────────────────────
    private NavMeshAgent agent;
    private float baseSpeed;
    private Vector3 smoothedUp = Vector3.up;
    private static List<float> usedSpeeds = new List<float>();

    // Mistake system
    private float mistakeTimer = 0f;
    private bool isMistakeActive = false;
    private Vector3 mistakeOffset = Vector3.zero;

    // Anti-stuck system
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private int stuckFrames = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Core settings for aggressive chaotic racing
        agent.updateRotation = false;
        agent.updateUpAxis = false;           // Crucial for tilted/banked tracks
        agent.angularSpeed = 0f;
        agent.autoBraking = false;
        agent.stoppingDistance = 0f;

        agent.radius = 0.5f;
        agent.height = 1.8f;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.avoidancePriority = Random.Range(1, 99);

        baseSpeed = GetUniqueSpeed(minSpeed, maxSpeed);
        agent.speed = baseSpeed;
        agent.acceleration = baseSpeed * 5f;

        if (endPoint != null)
        {
            Vector3 finalDest = endPoint.position + new Vector3(
                Random.Range(-4f, 4f), 0f, Random.Range(-4f, 4f));
            agent.SetDestination(finalDest);
        }

        lastPosition = transform.position;
    }

    void Update()
    {
        HandleMistakes();
        HandleStuckAgainstWall();

        // Occasional chaotic re-targeting for overtaking & madness
        if (Random.value < 0.02f && endPoint != null)
        {
            Vector3 chaoticDest = endPoint.position + new Vector3(
                Random.Range(-6f, 6f), 0f, Random.Range(-6f, 6f)) + mistakeOffset;
            agent.SetDestination(chaoticDest);
        }
    }

    void FixedUpdate()
    {
        // Optional wall-sliding / bounce (feels amazing)
        WallSlideAndBounce();
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

        // Finish line
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
                case 1: agent.speed = baseSpeed * 0.4f; break; // sudden brake
                case 2: agent.speed = baseSpeed * 2f; break;   // panic boost
                case 3: mistakeOffset = Random.insideUnitSphere * wobbleAmount * 2f; break;
            }
        }
    }

    void HandleStuckAgainstWall()
    {
        float movedDistance = Vector3.Distance(transform.position, lastPosition);

        if (movedDistance < stuckDetectionDistance)
        {
            stuckTimer += Time.deltaTime;
            stuckFrames++;
        }
        else
        {
            stuckTimer = 0f;
            stuckFrames = 0;
        }

        if (stuckTimer > stuckTimeThreshold || stuckFrames > 25)
        {
            // Strong sideways push to escape wall grinding
            Vector3 pushDir = Vector3.up;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, -transform.forward, out RaycastHit hit, 3f))
            {
                pushDir = Vector3.Cross(Vector3.up, agent.velocity.normalized + Vector3.up * 0.1f);
                if (Vector3.Dot(pushDir, (transform.position - hit.point)) < 0) pushDir = -pushDir;
            }

            agent.velocity += pushDir * pushAwayForce;
            agent.speed = baseSpeed * 1.5f; // temporary escape boost

            stuckTimer = 0f;
            stuckFrames = 0;
        }

        lastPosition = transform.position;
    }

    void WallSlideAndBounce()
    {
        if (agent.velocity.sqrMagnitude < 4f) return;

        if (Physics.Raycast(transform.position + Vector3.up * 0.6f, agent.velocity.normalized, out RaycastHit hit, 1.8f))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle > 70f) // definitely a wall
            {
                Vector3 reflectDir = Vector3.Reflect(agent.velocity.normalized, hit.normal);
                agent.velocity = Vector3.Lerp(agent.velocity, reflectDir * agent.velocity.magnitude, 0.4f);
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

    void OnDestroy()
    {
        usedSpeeds.Clear();
    }
}