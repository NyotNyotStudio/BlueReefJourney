using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHewan : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject[] seaCreatures;     // list of creature prefabs
    public Transform[] spawnPoints;       // assign your spawn points in Inspector
    public float spawnInterval = 3f;
    public int maxCreatures = 20;

    private int currentCount = 0;

    void Start()
    {
        InvokeRepeating(nameof(SpawnCreature), 1f, spawnInterval);
    }

    void SpawnCreature()
    {
        if (currentCount >= maxCreatures || seaCreatures.Length == 0 || spawnPoints.Length == 0)
            return;

        // prefab random
        GameObject prefab = seaCreatures[Random.Range(0, seaCreatures.Length)];

        // spawn point random
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Spawn hewan laut di titik spawn
        GameObject creature = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        currentCount++;

        // destroy setelah 30 detik
        Destroy(creature, 30f);

        // kurangi jumlah saat object di-destroy
        StartCoroutine(DecreaseCountAfterLifetime(30f));
    }

    IEnumerator DecreaseCountAfterLifetime(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentCount--;
    }
}
