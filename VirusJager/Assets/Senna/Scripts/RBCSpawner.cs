using UnityEngine;

public class RBCSpawner : MonoBehaviour
{
    public GameObject rbcPrefab;       // RBC prefab with RBCMovement attached
    public Transform waypointFolder;    // Same folder for the prefab to follow
    public float spawnInterval = 1.5f;  // Time between spawning RBCs

    void Start()
    {
        InvokeRepeating(nameof(SpawnRBC), 0f, spawnInterval);
    }

    void SpawnRBC()
    {
        GameObject newRBC = Instantiate(rbcPrefab, waypointFolder.GetChild(0).position, Quaternion.identity);
        RBCMovement movement = newRBC.GetComponent<RBCMovement>();
        movement.waypointFolder = waypointFolder;
    }
}
