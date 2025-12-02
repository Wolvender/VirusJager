using UnityEngine;

public class RBCMovement : MonoBehaviour
{
    public Transform waypointFolder;
    public float minSpeed = 2f;
    public float maxSpeed = 5f;
    [Header("Rotation")]
    public float turnMultiplier = 1f; // Multiplier to adjust turn speed relative to movement speed (e.g., 1f = same as speed, 2f = twice as snappy)

    private float speed;
    private float turnSpeed;
    private Transform[] waypoints;
    private int currentIndex = 0;

    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        turnSpeed = speed * turnMultiplier; // Automatically adjusts turn speed to match the random movement speed (random too!)

        int count = waypointFolder.childCount;
        waypoints = new Transform[count];
        for (int i = 0; i < count; i++)
            waypoints[i] = waypointFolder.GetChild(i);
        if (waypoints.Length > 0)
            transform.position = waypoints[0].position;
    }

    void Update()
    {
        if (currentIndex >= waypoints.Length) return;

        Transform target = waypoints[currentIndex];
        Vector3 direction = (target.position - transform.position).normalized;

        // Move towards target
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Smooth rotation to face forward (head always points direction of travel)
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // Check if reached waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            currentIndex++;
            if (currentIndex >= waypoints.Length)
                Destroy(gameObject);
        }
    }
}