using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public enum SpawnerType
    {
        Battery,
        SeaCreature,
        Trash
    }

    [SerializeField] private SpawnerType spawnerType;
    [SerializeField] private BoxCollider spawnArea;
    [SerializeField] private float baseSpawnInterval = 3f;
    [SerializeField] private int maxItems = 20;
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private float notFoundThreshold = 5f;
    [SerializeField] private float panicSpawnMultiplier = 0.3f;
    [SerializeField] private float seaCreatureLifetime = 10f;

    private float _lowPopulationTimer = 0f;
    private List<GameObject> _spawnedObjects = new List<GameObject>();
    private UIManager _uiManager;

    private void Start()
    {
        if (spawnArea == null || prefabs == null || prefabs.Length == 0) return;

        _uiManager = UIManager.Instance;
        if (_uiManager == null) _uiManager = FindObjectOfType<UIManager>();

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (spawnerType == SpawnerType.Trash && _uiManager != null)
            {
                if (_uiManager.IsJunkComplete())
                {
                    yield break;
                }
            }

            int currentCount = CleanAndCountObjects();
            float waitTime = baseSpawnInterval;

            if (spawnerType == SpawnerType.SeaCreature)
            {
                if (currentCount <= 1)
                {
                    _lowPopulationTimer += baseSpawnInterval;

                    if (_lowPopulationTimer >= notFoundThreshold)
                    {
                        waitTime *= panicSpawnMultiplier;
                    }
                }
                else
                {
                    _lowPopulationTimer = 0f;
                }
            }

            yield return new WaitForSeconds(waitTime);

            currentCount = CleanAndCountObjects();

            if (currentCount < maxItems)
            {
                SpawnObject();
            }
        }
    }

    private void SpawnObject()
    {
        Bounds bounds = spawnArea.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        Vector3 spawnPos = new Vector3(x, y, z);

        GameObject prefabToSpawn = null;

        switch (spawnerType)
        {
            case SpawnerType.Battery:
                if (prefabs.Length > 0)
                    prefabToSpawn = prefabs[0];
                break;

            case SpawnerType.Trash:
                if (prefabs.Length > 0)
                    prefabToSpawn = prefabs[Random.Range(0, prefabs.Length)];
                break;

            case SpawnerType.SeaCreature:
                prefabToSpawn = GetUnscannedCreaturePrefab();
                break;
        }

        if (prefabToSpawn != null)
        {
            GameObject newObj = Instantiate(prefabToSpawn, spawnPos, Random.rotation);
            _spawnedObjects.Add(newObj);

            if (spawnerType == SpawnerType.SeaCreature)
            {
                Destroy(newObj, seaCreatureLifetime);
            }
        }
    }

    private GameObject GetUnscannedCreaturePrefab()
    {
        if (prefabs.Length == 0) return null;
        if (_uiManager == null) return prefabs[Random.Range(0, prefabs.Length)];

        List<GameObject> unscannedList = new List<GameObject>();

        foreach (GameObject prefab in prefabs)
        {
            FishInfo info = prefab.GetComponent<FishInfo>();

            if (info != null && !_uiManager.IsFishScanned(info.creatureName))
            {
                unscannedList.Add(prefab);
            }
        }

        if (unscannedList.Count > 0)
        {
            return unscannedList[Random.Range(0, unscannedList.Count)];
        }

        return prefabs[Random.Range(0, prefabs.Length)];
    }

    private int CleanAndCountObjects()
    {
        _spawnedObjects.RemoveAll(item => item == null);
        return _spawnedObjects.Count;
    }
}