using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SpermSpawner : MonoBehaviour
{
    [Header("Prefab & Start")]
    public GameObject racerPrefab;
    public Transform startPoint;

    [Header("Spawn Wave")]
    public int spawnCount = 25;
    public float spawnWidth = 4f;
    public float spawnDelay = 0.05f;
    public bool autoSpawnOnStart = true;

    [Header("Speed Override (Optional)")]
    public bool overrideSpeeds = false;
    public float minSpeed = 2f;
    public float maxSpeed = 5f;

    [Header("Debug")]
    public bool showSpawnDebugLogs = true; // Toggle for console spam

    void Start()
    {
        if (autoSpawnOnStart && racerPrefab != null && startPoint != null)
        {
            if (showSpawnDebugLogs) Debug.Log("🧬 SpermSpawner: Auto-spawning wave on Start!");
            SpawnWave();
        }
    }

    [ContextMenu("Spawn Wave! 🏁")]
    public void SpawnWave()
    {
        if (racerPrefab == null)
        {
            Debug.LogError("❌ Assign racerPrefab!");
            return;
        }
        if (startPoint == null)
        {
            Debug.LogError("❌ Assign startPoint!");
            return;
        }

        if (showSpawnDebugLogs) Debug.Log("🧬 SpermSpawner: Starting spawn wave...");

        StartCoroutine(SpawnWaveCoroutine());
    }

    IEnumerator SpawnWaveCoroutine()
    {
        // 🛡️ STEP 1: Find VALID base start position on NavMesh (auto-fixes off-mesh issues!)
        Vector3 baseSpawnPos;
        if (!NavMesh.SamplePosition(startPoint.position, out NavMeshHit baseHit, 5f, NavMesh.AllAreas))
        {
            Debug.LogError("❌ NO NavMesh near startPoint! Bake NavMesh or move startPoint closer.");
            yield break;
        }
        baseSpawnPos = baseHit.position;
        if (showSpawnDebugLogs) Debug.Log($"✅ Base spawn snapped to NavMesh: {baseSpawnPos}");

        int spawnedCount = 0;
        for (int i = 0; i < spawnCount; i++)
        {
            // Random side-by-side offset
            Vector3 offset = new Vector3(
                Random.Range(-spawnWidth / 2f, spawnWidth / 2f),
                0f,
                Random.Range(-spawnWidth / 4f, spawnWidth / 4f)
            );

            // Candidate from BASE NavMesh pos (more reliable)
            Vector3 candidatePos = baseSpawnPos + startPoint.right * offset.x + startPoint.forward * offset.z;

            // 🛡️ STEP 2: Snap EACH spawn to VALID NavMesh (large radius = always succeeds)
            if (NavMesh.SamplePosition(candidatePos, out NavMeshHit hit, 8f, NavMesh.AllAreas))
            {
                GameObject newRacer = Instantiate(racerPrefab, hit.position, startPoint.rotation);

                // Setup racer
                RBCNavRacer navRacer = newRacer.GetComponent<RBCNavRacer>();
                if (navRacer != null)
                {
                    if (overrideSpeeds)
                    {
                        navRacer.minSpeed = minSpeed;
                        navRacer.maxSpeed = maxSpeed;
                    }
                    // Force path recalc (in case endPoint issue)
                    if (navRacer.endPoint != null)
                        newRacer.GetComponent<NavMeshAgent>().SetDestination(navRacer.endPoint.position);
                }

                spawnedCount++;
                if (showSpawnDebugLogs) Debug.Log($"✅ Spawned sperm #{i + 1} at {hit.position}");
            }
            else
            {
                if (showSpawnDebugLogs) Debug.LogWarning($"⚠️ Failed spawn #{i + 1} — no NavMesh spot!");
            }

            yield return new WaitForSeconds(spawnDelay);
        }

        Debug.Log($"🏁 Spawn COMPLETE: {spawnedCount}/{spawnCount} sperm racers unleashed!");
    }

    void OnDrawGizmosSelected()
    {
        if (startPoint == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(startPoint.position, new Vector3(spawnWidth, 0.2f, spawnWidth / 2f));

        // Show sampled base pos
        if (NavMesh.SamplePosition(startPoint.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(hit.position, 0.3f);
        }
    }
}