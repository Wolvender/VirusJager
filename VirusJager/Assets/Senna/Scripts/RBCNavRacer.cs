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

    [Header("=== SMOOTHING & PERFORMANCE ===")]
    public float speedChangeRate = 12f;
    public float destBlendRate = 8f;
    public float velocitySmoothing = 10f;

    // ──────────────────────
    private NavMeshAgent agent;
    private float baseSpeed;
    private float currentSpeed;
    private Vector3 smoothedUp = Vector3.up;
    private static List<float> usedSpeeds = new List<float>();

    // Mistake system
    private float mistakeTimer = 0f;
    private bool isMistakeActive = false;
    private Vector3 targetMistakeOffset = Vector3.zero;
    private Vector3 currentMistakeOffset = Vector3.zero;

    // Anti-stuck
    private Vector3 lastPosition;
    private float stuckTimer = 0f;
    private int stuckFrames = 0;

    // Smoothing
    private Vector3 smoothedVelocity;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        NavMesh.pathfindingIterationsPerFrame = 25000;

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.angularSpeed = 0f;
        agent.autoBraking = false;
        agent.stoppingDistance = 0f;
        agent.radius = 0.5f;
        agent.height = 1.8f;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.avoidancePriority = Random.Range(1, 99);

        baseSpeed = GetUniqueSpeed(minSpeed, maxSpeed);
        currentSpeed = baseSpeed;
        agent.speed = baseSpeed;
        agent.acceleration = baseSpeed * 5f;

        lastPosition = transform.position;
        smoothedVelocity = transform.forward * baseSpeed * 0.7f;

        if (endPoint != null)
        {
            Vector3 offset = new Vector3(
                Random.Range(-4f, 4f),   // X
                0f,                      // Y  ← you probably meant 0f, not 4f
                Random.Range(-4f, 4f)    // Z
            );
            SetSmoothDestination(endPoint.position + offset);
        }
    }

    void Update()
    {
        HandleMistakes();
        HandleStuckAgainstWall();

        // Chaotic retargeting (keeps them lively)
        if (Random.value < 0.02f && endPoint != null)
        {
            Vector3 chaoticDest = endPoint.position + new Vector3(
                Random.Range(-6f, 6f), 0f, Random.Range(-6f, 6f)) + currentMistakeOffset;
            SetSmoothDestination(chaoticDest);
        }

        ApplySmoothing();
    }

    void FixedUpdate()
    {
        WallSlideAndBounce();
    }

    void LateUpdate()
    {
        if (agent.velocity.sqrMagnitude > 0.1f || currentMistakeOffset != Vector3.zero)
        {
            Vector3 moveDirection = smoothedVelocity.normalized;

            if (moveDirection.sqrMagnitude < 0.1f || Vector3.Dot(moveDirection, transform.forward) < -0.3f)
                moveDirection = transform.forward;

            Vector3 wobble = currentMistakeOffset;
            if (wobble.sqrMagnitude > 0.1f)
            {
                wobble = Vector3.ProjectOnPlane(wobble, Vector3.up);
                wobble = Vector3.ClampMagnitude(wobble, wobbleAmount * 0.8f);
            }

            Vector3 finalForward = (moveDirection + wobble * 0.3f).normalized;

            if (Vector3.Dot(finalForward, transform.forward) < -0.1f)
                finalForward = Vector3.Lerp(transform.forward, finalForward, 0.6f).normalized;

            Vector3 up = smoothedUp;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, raycastDistance, groundLayer))
                up = Vector3.Lerp(up, hit.normal, tiltSmoothing * Time.deltaTime);

            Quaternion targetRot = Quaternion.LookRotation(finalForward, up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, currentSpeed * turnMultiplier * Time.deltaTime);
        }

        // REMOVED THE DESTROY LINE ON PURPOSE
        // Now only the EggFertilization script decides when to kill the sperm
    }

    // ────── ALL OTHER METHODS UNCHANGED METHODS BELOW ──────
    void ApplySmoothing()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, isMistakeActive ? agent.speed : baseSpeed, speedChangeRate * Time.deltaTime);
        agent.speed = currentSpeed;

        currentMistakeOffset = Vector3.Lerp(currentMistakeOffset, targetMistakeOffset, 14f * Time.deltaTime);

        Vector3 desiredVel = agent.desiredVelocity + currentMistakeOffset * 8f;
        smoothedVelocity = Vector3.Lerp(smoothedVelocity, desiredVel, velocitySmoothing * Time.deltaTime);
        agent.velocity = smoothedVelocity;
    }

    void SetSmoothDestination(Vector3 dest)
    {
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path) &&
            path.status == NavMeshPathStatus.PathComplete)
        {
            agent.SetPath(path);
        }
        else
        {
            agent.SetDestination(dest);
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
                targetMistakeOffset = Vector3.zero;
            }
            return;
        }

        if (Random.value < mistakeChance * Time.deltaTime)
        {
            isMistakeActive = true;
            mistakeTimer = mistakeDuration + Random.Range(-0.4f, 0.6f);
            int type = Random.Range(0, 4);
            switch (type)
            {
                case 0: targetMistakeOffset = Random.insideUnitSphere * wobbleAmount * 0.8f; break;
                case 1: agent.speed = baseSpeed * 0.45f; break;
                case 2: agent.speed = baseSpeed * 1.9f; break;
                case 3: targetMistakeOffset = Random.insideUnitSphere * wobbleAmount * 1.4f; break;
            }
        }
    }

    void HandleStuckAgainstWall()
    {
        float moved = Vector3.Distance(transform.position, lastPosition);
        if (moved < stuckDetectionDistance)
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
            Vector3 pushDir = transform.right;
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, -transform.forward, out RaycastHit hit, 3f))
            {
                pushDir = Vector3.Cross(Vector3.up, agent.velocity.normalized).normalized;
                if (Vector3.Dot(pushDir, transform.position - hit.point) < 0) pushDir = -pushDir;
            }

            agent.velocity += pushDir * pushAwayForce;
            agent.speed = baseSpeed * 1.6f;
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
            if (Vector3.Angle(hit.normal, Vector3.up) > 70f)
            {
                Vector3 reflect = Vector3.Reflect(agent.velocity.normalized, hit.normal);
                agent.velocity = Vector3.Lerp(agent.velocity, reflect * agent.velocity.magnitude, 0.5f);
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