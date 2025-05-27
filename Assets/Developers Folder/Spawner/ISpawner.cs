using UnityEngine;

public interface ISpawner
{
    void Update(float deltaTime);
    void StartSpawning();
    void StopSpawning();
    void SetSpawnParameters(float interval, int maxObjects);
    event System.Action<GameObject> OnObjectSpawned;
}