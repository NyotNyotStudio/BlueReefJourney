using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnvironmentSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject[] environmentPrefabs;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private Collider terrainCollider;
    [SerializeField] private int minObjects = 20;
    [SerializeField] private int maxObjects = 50;
    [SerializeField] private float minDistance = 5f;
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private float heightOffset = -1.0f;
    [SerializeField] private bool alignToSlope = true;

    private List<Vector3> spawnedPositions = new List<Vector3>();

    private void Start()
    {
        StartCoroutine(SpawnEnvironmentRoutine());
    }

    public IEnumerator SpawnEnvironmentRoutine()
    {
        if (environmentPrefabs == null || environmentPrefabs.Length == 0) yield break;

        if (terrainCollider == null)
        {
            Debug.LogError("Terrain Collider belum di-assign!");
            yield break;
        }

        if (spawnParent == null) spawnParent = transform;

        yield return null;

        ClearEnvironment();
        spawnedPositions.Clear();

        int objectCount = Random.Range(minObjects, maxObjects + 1);
        int currentSpawned = 0;
        int totalAttempts = 0;
        int maxAttempts = objectCount * 50;

        while (currentSpawned < objectCount && totalAttempts < maxAttempts)
        {
            totalAttempts++;

            Vector3 pos = GetRandomPosition(out bool hitTerrain, out Vector3 surfaceNormal);

            if (hitTerrain && IsPositionValid(pos))
            {
                SpawnObjectAt(pos, surfaceNormal);
                spawnedPositions.Add(pos);
                currentSpawned++;
            }

            if (totalAttempts % 5 == 0)
            {
                yield return null;
            }
        }
    }

    [ContextMenu("Clear Environment")]
    public void ClearEnvironment()
    {
        if (spawnParent == null) return;

        for (int i = spawnParent.childCount - 1; i >= 0; i--)
        {
            GameObject obj = spawnParent.GetChild(i).gameObject;
            if (Application.isPlaying)
                Destroy(obj);
            else
                DestroyImmediate(obj);
        }
        spawnedPositions.Clear();
    }

    private Vector3 GetRandomPosition(out bool hitTerrain, out Vector3 surfaceNormal)
    {
        Bounds bounds = terrainCollider.bounds;

        if (bounds.size == Vector3.zero)
        {
            hitTerrain = false;
            surfaceNormal = Vector3.up;
            return Vector3.zero;
        }

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        Vector3 origin = new Vector3(x, bounds.max.y + 200f, z);

        hitTerrain = false;
        surfaceNormal = Vector3.up;
        Vector3 finalPosition = Vector3.zero;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 1000f, terrainLayer))
        {
            finalPosition = hit.point;
            finalPosition.y += heightOffset;
            surfaceNormal = hit.normal;
            hitTerrain = true;
        }

        return finalPosition;
    }

    private bool IsPositionValid(Vector3 position)
    {
        float minDistSqr = minDistance * minDistance;

        for (int i = 0; i < spawnedPositions.Count; i++)
        {
            if ((position - spawnedPositions[i]).sqrMagnitude < minDistSqr)
                return false;
        }
        return true;
    }

    private void SpawnObjectAt(Vector3 position, Vector3 surfaceNormal)
    {
        GameObject prefab = environmentPrefabs[Random.Range(0, environmentPrefabs.Length)];
        Quaternion finalRotation;

        if (alignToSlope)
        {
            Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);
            Quaternion yRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            finalRotation = slopeRotation * yRotation;
        }
        else
        {
            finalRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        }

        Instantiate(prefab, position, finalRotation, spawnParent);
    }

    private void OnDrawGizmosSelected()
    {
        if (terrainCollider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(terrainCollider.bounds.center, terrainCollider.bounds.size);
        }
    }
}