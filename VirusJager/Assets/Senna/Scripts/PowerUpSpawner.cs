using UnityEngine;
using System.Collections.Generic;

public class PowerUpSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public float spawnRadius = 50f;
    public float spawnInterval = 8f;
    public int maxPowerUps = 5;

    [Header("Power-Up Prefabs")]
    public List<GameObject> powerUpPrefabs = new List<GameObject>();

    [Header("Debug")]
    public bool showGizmos = true;

    // THIS IS THE LIST THAT TRACKS EVERYTHING
    private List<GameObject> activePowerUps = new List<GameObject>();

    void Start()
    {
        InvokeRepeating(nameof(TrySpawnPowerUp), 2f, spawnInterval);
    }

    void TrySpawnPowerUp()
    {
        // Clean up any destroyed power-ups first (important!)
        activePowerUps.RemoveAll(item => item == null);

        if (activePowerUps.Count >= maxPowerUps) return;
        if (powerUpPrefabs.Count == 0) return;

        Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);

        if (Physics.Raycast(spawnPosition + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f))
        {
            // BEST ONE → nice floating height + room for spin/bob
            spawnPosition = hit.point + Vector3.up * 1.3f;
        }
        else
        {
            // Fallback if no ground found
            spawnPosition.y += 1.3f;
        }

        GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Count)];
        GameObject powerUp = Instantiate(prefab, spawnPosition, Quaternion.identity);

        // ADD TO TRACKING LIST
        activePowerUps.Add(powerUp);

        // Optional: auto-remove after 30 seconds if never picked up
        Destroy(powerUp, 30f);
    }

    // Optional: visual count in hierarchy or console
    void Update()
    {
        // Clean up null references every frame (very cheap)
        activePowerUps.RemoveAll(item => item == null);
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        Gizmos.color = new Color(0, 1, 1, 0.2f);
        Gizmos.DrawSphere(transform.position, spawnRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}