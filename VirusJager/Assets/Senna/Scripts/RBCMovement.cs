using UnityEngine;

public class RBCMovement : MonoBehaviour
{
    public Transform waypointFolder;
    public float minSpeed = 2f;
    public float maxSpeed = 5f;

    [Header("Movement Feel")]
    public float acceleration = 8f;           // How fast they reach their target speed
    public float turnMultiplier = 2f;          // 2–3 feels very snappy and sperm-like
    public float waypointReachedDistance = 0.3f; // Bigger = smoother transitions

    [Header("Wiggle (optional but looks amazing)")]
    public bool addWiggle = true;
    public float wiggleFrequency = 8f;
    public float wiggleMagnitude = 0.3f;

    private float speed;
    private float currentVelocity; // for smooth acceleration
    private float turnSpeed;

    private Transform[] waypoints;
    private int currentIndex = 0;

    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        turnSpeed = speed * turnMultiplier;

        int count = waypointFolder.childCount;
        waypoints = new Transform[count];
        for (int i = 0; i < count; i++)
            waypoints[i] = waypointFolder.GetChild(i);

        if (waypoints.Length > 0)
            transform.position = waypoints[0].position;
    }

    void Update()
    {
        if (currentIndex >= waypoints.Length)
        {
            Destroy(gameObject);
            return;
        }

        Transform target = waypoints[currentIndex];
        Vector3 toTarget = target.position - transform.position;
        float distanceToTarget = toTarget.magnitude;

        // === Early switch to next waypoint for super smooth flow ===
        if (distanceToTarget < waypointReachedDistance)
        {
            currentIndex++;
            if (currentIndex >= waypoints.Length)
            {
                Destroy(gameObject);
                return;
            }
            target = waypoints[currentIndex];
            toTarget = target.position - transform.position;
        }

        Vector3 direction = toTarget.normalized;

        // Smoothly accelerate/decelerate (feels much more natural)
        float targetSpeed = speed;
        currentVelocity = Mathf.MoveTowards(currentVelocity, targetSpeed, acceleration * Time.deltaTime);

        // Move
        transform.position += direction * currentVelocity * Time.deltaTime;

        // === Rotation (super snappy but smooth) ===
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            // Use a fixed high rotation speed so even fast speed changes feel instant
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // === Optional tail-like wiggle (makes it look 10× more like real sperm) ===
        if (addWiggle && direction != Vector3.zero)
        {
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;
            float wiggle = Mathf.Sin(Time.time * wiggleFrequency) * wiggleMagnitude;
            transform.rotation *= Quaternion.Euler(0, 0, wiggle);
        }
    }
}