using System.Collections.Generic;
using UnityEngine;

public class SimpleMapGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public List<GameObject> straightPrefabs;
    public List<GameObject> turnPrefabs;

    [Header("Settings")]
    public int maxSegments = 20;
    public float groundY = 0f;
    [Range(0f, 1f)] public float turnChance = 0.2f;
    public Vector3 pivotOffset;

    private List<GameObject> spawnedPieces = new List<GameObject>();
    private Transform lastSnap;

    [ContextMenu("Generate Map")]
    public void GenerateMap()
    {
        ResetMap();

        // Start snap at generator position
        lastSnap = new GameObject("StartSnap").transform;
        lastSnap.position = new Vector3(transform.position.x, groundY, transform.position.z);
        lastSnap.rotation = transform.rotation;

        for (int i = 0; i < maxSegments; i++)
        {
            bool spawnTurn = (turnPrefabs.Count > 0) && (Random.value < turnChance);

            GameObject prefab;
            Transform entrySnap, exitSnap;

            if (spawnTurn)
            {
                prefab = turnPrefabs[Random.Range(0, turnPrefabs.Count)];
                entrySnap = prefab.transform.Find("SnapPointEntry");
                exitSnap = prefab.transform.Find("SnapPointExit");
                if (entrySnap == null || exitSnap == null)
                {
                    Debug.LogWarning("Turn prefab missing snap points!");
                    break;
                }
            }
            else
            {
                prefab = straightPrefabs[Random.Range(0, straightPrefabs.Count)];
                entrySnap = prefab.transform.Find("SnapPoint");
                exitSnap = entrySnap;
                if (entrySnap == null)
                {
                    Debug.LogWarning("Straight prefab missing SnapPoint!");
                    continue;
                }
            }

            // Compute position: align prefab entry to lastSnap
            Vector3 spawnPos = lastSnap.position - prefab.transform.rotation * entrySnap.localPosition + pivotOffset;
            spawnPos.y = groundY; // force flat

            Quaternion spawnRot = lastSnap.rotation;

            GameObject obj = Instantiate(prefab, spawnPos, spawnRot, transform);
            spawnedPieces.Add(obj);

            lastSnap = obj.transform.Find(exitSnap.name);

            // Stop generation after first turn
            if (spawnTurn) break;
        }

        if (lastSnap != null && lastSnap.name == "StartSnap")
            Destroy(lastSnap.gameObject);
    }

    [ContextMenu("Reset Map")]
    public void ResetMap()
    {
        foreach (var obj in spawnedPieces)
        {
            if (obj != null)
#if UNITY_EDITOR
                DestroyImmediate(obj);
#else
                Destroy(obj);
#endif
        }
        spawnedPieces.Clear();
    }
}
