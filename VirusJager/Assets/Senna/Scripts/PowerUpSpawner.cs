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

    private List<GameObject> activePowerUps = new List<GameObject>();

    void Start()
    {
        InvokeRepeating(nameof(TrySpawnPowerUp), 2f, spawnInterval);
    }

    void TrySpawnPowerUp()
    {
        activePowerUps.RemoveAll(item => item == null);

        if (activePowerUps.Count >= maxPowerUps) return;
        if (powerUpPrefabs.Count < 2) return;

        // Pick two DIFFERENT prefabs
        List<GameObject> selectedPrefabs = GetTwoDifferentPrefabs();
        if (selectedPrefabs == null || selectedPrefabs.Count != 2) return;

        // Spawn TWO power-ups at COMPLETELY INDEPENDENT random positions
        for (int i = 0; i < 2; i++)
        {
            // Completely new random point inside the circle
            Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomPoint.x, 0, randomPoint.y);

            // Raycast down to find ground
            if (Physics.Raycast(spawnPosition + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f))
            {
                spawnPosition = hit.point + Vector3.up * 1.3f;
            }
            else
            {
                spawnPosition.y += 1.3f;
            }

            GameObject powerUp = Instantiate(selectedPrefabs[i], spawnPosition, Quaternion.identity);
            activePowerUps.Add(powerUp);
            Destroy(powerUp, 30f);
        }
    }

    List<GameObject> GetTwoDifferentPrefabs()
    {
        List<GameObject> selected = new List<GameObject>();

        int firstIndex = Random.Range(0, powerUpPrefabs.Count);
        selected.Add(powerUpPrefabs[firstIndex]);

        int secondIndex;
        do
        {
            secondIndex = Random.Range(0, powerUpPrefabs.Count);
        } while (secondIndex == firstIndex);

        selected.Add(powerUpPrefabs[secondIndex]);
        return selected;
    }

    void Update()
    {
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