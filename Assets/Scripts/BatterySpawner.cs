using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterySpawner : MonoBehaviour
{
    [Header("Battery Settings")]
    [Tooltip("The battery prefab to spawn")]
    public GameObject batteryPrefab;

    [Tooltip("How often to spawn a battery (seconds)")]
    public float spawnInterval = 10f;

    [Header("Spawn Points")]
    [Tooltip("List of spawn points where the battery can appear")]
    public List<Transform> spawnPoints = new List<Transform>();

    private void Start()
    {
        // Start spawning batteries repeatedly
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true) // infinite loop
        {
            yield return new WaitForSeconds(spawnInterval);

            SpawnBattery();
        }
    }

    private void SpawnBattery()
    {
        if (batteryPrefab == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("Battery prefab or spawn points not set!");
            return;
        }

        // Pick a random spawn point
        int index = Random.Range(0, spawnPoints.Count);
        Transform spawnPoint = spawnPoints[index];

        // Spawn the battery
        Instantiate(batteryPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
