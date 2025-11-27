using UnityEngine;

public class RBCMovement : MonoBehaviour
{
    public Transform waypointFolder;
    public float minSpeed = 2f;
    public float maxSpeed = 5f;

    private float speed;
    private Transform[] waypoints;
    private int currentIndex = 0;

    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);

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
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            currentIndex++;

            if (currentIndex >= waypoints.Length)
                Destroy(gameObject);
        }
    }
}
