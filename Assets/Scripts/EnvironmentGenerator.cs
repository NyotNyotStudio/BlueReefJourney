using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnvironmentSpawner : MonoBehaviour
{
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
        SpawnEnvironment();
    }

    [ContextMenu("Spawn Environment")]
    public void SpawnEnvironment()
    {
        if (environmentPrefabs == null || environmentPrefabs.Length == 0) return;

        if (terrainCollider == null)
        {
            UnityEngine.Debug.LogError("Terrain Collider belum di-assign!");
            return;
        }

        if (spawnParent == null) spawnParent = transform;

        ClearEnvironment();
        spawnedPositions.Clear();

        int objectCount = Random.Range(minObjects, maxObjects + 1);
        int attempts = 0;

        for (int i = 0; i < objectCount && attempts < objectCount * 10; attempts++)
        {
            Vector3 pos = GetRandomPosition(out bool hitTerrain, out Vector3 surfaceNormal);

            if (hitTerrain && IsPositionValid(pos))
            {
                SpawnObjectAt(pos, surfaceNormal);
                spawnedPositions.Add(pos);
                i++;
            }
        }
    }

    [ContextMenu("Clear Environment")]
    public void ClearEnvironment()
    {
        if (spawnParent == null) return;

        for (int i = spawnParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(spawnParent.GetChild(i).gameObject);
        }
        spawnedPositions.Clear();
    }

    private Vector3 GetRandomPosition(out bool hitTerrain, out Vector3 surfaceNormal)
    {
        Bounds bounds = terrainCollider.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        Vector3 origin = new Vector3(x, bounds.max.y + 100f, z);
        Vector3 finalPosition = origin;
        surfaceNormal = Vector3.up;
        hitTerrain = false;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, bounds.size.y + 200f, terrainLayer))
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
        foreach (Vector3 spawnedPos in spawnedPositions)
        {
            if (Vector3.Distance(position, spawnedPos) < minDistance)
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