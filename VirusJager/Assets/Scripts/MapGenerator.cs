using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Path Settings")]
    public int numberOfPoints = 24;
    public float spacing = 5f; // distance between points
    public float noise = 0.0f; // optional vertical noise

    [Header("Prefab Settings")]
    public List<GameObject> roadPieces;
    public float pieceSpacing = 0.0f;
    public Vector3 pivotOffset;

    private List<Vector3> pathPoints = new List<Vector3>();
    private List<GameObject> spawnedPieces = new List<GameObject>();

    [Header("Turn Prefabs")]
    public List<GameObject> turnPrefabs;
    public float turnChance = 0.2f; // 20% chance for a turn at a segment


    // ----------------------------
    // BUTTON: Generate Map
    // ----------------------------
    [ContextMenu("Generate Map")]
    public void GenerateMap()
    {
        ResetMap();
        GeneratePath();
        SpawnRoadPieces();
    }

    // ----------------------------
    // BUTTON: Reset Map
    // ----------------------------
    [ContextMenu("Reset Map")]
    public void ResetMap()
    {
        for (int i = spawnedPieces.Count - 1; i >= 0; i--)
        {
            if (spawnedPieces[i] != null)
            {
                // If in editor and not playing, use DestroyImmediate
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    UnityEngine.Object.DestroyImmediate(spawnedPieces[i]);
                else
                    UnityEngine.Object.Destroy(spawnedPieces[i]);
#else
            UnityEngine.Object.Destroy(spawnedPieces[i]);
#endif
            }
        }

        spawnedPieces.Clear();
        pathPoints.Clear();
    }


    // ----------------------------
    // Generate Straight Path
    // ----------------------------
    void GeneratePath()
    {
        pathPoints.Clear();
        Vector3 startPos = transform.position;
        Vector3 direction = transform.forward.normalized;

        for (int i = 0; i < numberOfPoints; i++)
        {
            Vector3 point = startPos + direction * (i * spacing);
            point.y += Random.Range(-noise, noise); // optional vertical noise
            pathPoints.Add(point);
        }
    }

    // ----------------------------
    // Spawn Prefabs Along Path
    // ----------------------------

    float GetPrefabLength(GameObject prefab)
    {
        MeshFilter mf = prefab.GetComponentInChildren<MeshFilter>();
        if (mf == null) return 1f; // default length if no mesh

        Bounds bounds = mf.sharedMesh.bounds;
        // scale it properly
        Vector3 scaledSize = Vector3.Scale(bounds.size, prefab.transform.lossyScale);
        return scaledSize.z;
    }

    void SpawnRoadPieces()
    {
        if (pathPoints.Count == 0) return;

        Vector3 currentPos = pathPoints[0];

        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Vector3 nextPos = pathPoints[i + 1];
            Vector3 direction = (nextPos - currentPos).normalized;

            GameObject prefab;
            Quaternion rot;
            Vector3 spawnPos;

            // Decide if this segment should be a turn
            if (turnPrefabs.Count > 0 && Random.value < turnChance && spawnedPieces.Count > 0)
            {
                prefab = turnPrefabs[Random.Range(0, turnPrefabs.Count)];

                // Get the last straight prefab
                GameObject lastStraight = spawnedPieces[spawnedPieces.Count - 1];

                // Calculate the forward extent of the last prefab
                float forwardOffset = GetForwardExtent(lastStraight);

                // Position the turn at the end of the last straight prefab
                spawnPos = lastStraight.transform.position + lastStraight.transform.forward * forwardOffset;

                // Apply turn rotation (left turn)
                rot = Quaternion.Euler(-90f, -90f, 0f);

                // Apply turn prefab's pivot offset
                spawnPos += rot * pivotOffset;

                // Spawn the turn prefab
                GameObject turnObj = Instantiate(prefab, spawnPos, rot, transform);
                spawnedPieces.Add(turnObj);

                // Move currentPos to the end of the turn for future pieces
                float turnLength = GetPrefabLength(prefab);
                currentPos = spawnPos + rot * Vector3.forward * turnLength;

                // Stop straight line here
                break;
            }
            else
            {
                // Straight prefab
                prefab = roadPieces[Random.Range(0, roadPieces.Count)];

                // Straight rotation along path, Y = -90
                rot = Quaternion.LookRotation(direction, Vector3.up);
                Vector3 euler = rot.eulerAngles;
                euler.y = -90f;
                rot = Quaternion.Euler(euler);

                // Spawn straight prefab at current position with pivot offset
                spawnPos = currentPos + rot * pivotOffset;
                GameObject straightObj = Instantiate(prefab, spawnPos, rot, transform);
                spawnedPieces.Add(straightObj);

                // Move current position forward by prefab length
                float length = GetPrefabLength(prefab);
                currentPos += direction * (length + pieceSpacing);
            }
        }
    }

    // Helper: Get the forward half-length of a prefab (distance from pivot to front face)
    float GetForwardExtent(GameObject obj)
    {
        MeshFilter mf = obj.GetComponentInChildren<MeshFilter>();
        if (mf == null) return 1f; // default if no mesh

        Bounds bounds = mf.sharedMesh.bounds;
        Vector3 scaledSize = Vector3.Scale(bounds.size, obj.transform.lossyScale);
        return scaledSize.z / 2f; // half Z = forward extent
    }




}
