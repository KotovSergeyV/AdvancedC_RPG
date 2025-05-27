using System.Collections.Generic;
using UnityEngine;

public class Spawner : ISpawner
{
    protected List<Transform> spawnPoints = new List<Transform>();
    protected List<GameObject> spawnedObjects = new List<GameObject>();
    protected float spawnTimer;
    protected float spawnInterval = 5f;
    protected int maxObjects = 10;
    protected bool isActive;

    public event System.Action<GameObject> OnObjectSpawned;

    public Spawner(Transform[] spawnPoints)
    {
        if (spawnPoints != null)
        {
            this.spawnPoints.AddRange(spawnPoints);
        }
    }

    public virtual void Update(float deltaTime)
    {
        if (!isActive || spawnedObjects.Count >= maxObjects) return;

        spawnTimer += deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnObject();
        }
    }

    public void StartSpawning()
    {
        isActive = true;
    }
    public void StopSpawning()
    {
        isActive = false;
    }

    public void SetSpawnParameters(float interval, int maxObjects)
    {
        spawnInterval = interval;
        this.maxObjects = maxObjects;
    }

    protected virtual void SpawnObject()
    {
        if (spawnPoints.Count == 0) return;

        var spawnPoint = GetRandomSpawnPoint();
        var obj = Object.Instantiate(GetObjectToSpawn(), spawnPoint.position, spawnPoint.rotation);
        spawnedObjects.Add(obj);
        OnObjectSpawned?.Invoke(obj);
    }

    protected virtual GameObject GetObjectToSpawn()
    {
        return new GameObject("DefaultSpawnedObject");
    }

    protected Transform GetRandomSpawnPoint()
    {
        return spawnPoints.Count > 0 ? spawnPoints[Random.Range(0, spawnPoints.Count)] : new GameObject("FallbackSpawnPoint").transform;
    }

    public void Cleanup()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
            {
                Object.Destroy(obj);
            }
            spawnedObjects.Clear();
        }
    }
}