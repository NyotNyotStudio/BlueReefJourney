using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BatterySpawner : MonoBehaviour
{
    [SerializeField] private GameObject batteryPrefab;
    [SerializeField] private BoxCollider spawnArea;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxBatteries = 20;

    private void Start()
    {
        if (spawnArea == null) return;
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            int jumlahBaterai = FindObjectsOfType<SinkingItem>().Length;

            if (jumlahBaterai < maxBatteries)
            {
                SpawnBattery();
            }
        }
    }

    private void SpawnBattery()
    {
        if (spawnArea == null || batteryPrefab == null) return;

        Bounds bounds = spawnArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        Instantiate(batteryPrefab, new Vector3(x, y, z), Random.rotation);
    }
}