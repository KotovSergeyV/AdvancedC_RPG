using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Spawner
{
    private List<SpawnConfig> meleeConfigs = new List<SpawnConfig>();
    private List<SpawnConfig> rangedConfigs = new List<SpawnConfig>();
    private float rangedChance = 0.3f;

    public EnemySpawner(Transform[] spawnPoints) : base(spawnPoints) { }

    public void AddMeleeEnemy(GameObject prefab, List<GameObject> weapons, float weight = 1f)
    {
        meleeConfigs.Add(new SpawnConfig
        {
            Prefab = prefab,
            VariantsWeapons = weapons,
            SpawnWeight = weight
        });
    }

    public void AddRangedEnemy(GameObject prefab, List<GameObject> weapons, float weight = 1f)
    {
        rangedConfigs.Add(new SpawnConfig
        {
            Prefab = prefab,
            VariantsWeapons = weapons,
            SpawnWeight = weight
        });
    }

    private SpawnConfig lastSpawnedConfig;

    public SpawnConfig GetLastSpawnedConfig()
    {
        return lastSpawnedConfig;
    }

    protected override GameObject GetObjectToSpawn()
    {
        bool isRanged = Random.value <= rangedChance;
        var configs = isRanged ? rangedConfigs : meleeConfigs;

        if (configs.Count == 0)
            return base.GetObjectToSpawn();

        lastSpawnedConfig = GetRandomConfig(configs);
        return lastSpawnedConfig.Prefab;
    }

    private SpawnConfig GetRandomConfig(List<SpawnConfig> configs)
    {
        float totalWeight = 0f;
        foreach (var c in configs) totalWeight += c.SpawnWeight;

        float randomPoint = Random.value * totalWeight;
        float currentWeight = 0f;

        foreach (var config in configs)
        {
            currentWeight += config.SpawnWeight;
            if (randomPoint <= currentWeight) return config;
        }

        return configs[0];
    }
}