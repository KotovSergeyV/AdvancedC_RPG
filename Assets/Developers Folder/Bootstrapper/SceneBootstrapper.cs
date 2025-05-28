using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.EventSystems.EventTrigger;

public class SceneBootstrapper
{
    private EnemySpawner enemySpawner;
    public EnemySpawner Spawner => enemySpawner;
    private ManagerSFX sfxManager;
    private ManagerUI uiManager;


    public void Initialize(GameObject bossPrefab, ManagerSFX managerSFX, ManagerUI managerUI, Transform[] spawnPoints = null, List<EntitySaveData> data = null)

    {
        sfxManager = managerSFX;
        uiManager = managerUI;

        InitializeEnemies(managerSFX, managerUI, data);

        EventHub.Broadcast_EnemyCounterUpdated += (count) =>
            {
                if (count == 3)
                    InitializeBoss(bossPrefab, managerUI);
            };
        
        InitializeSpawner(spawnPoints);

    }
    

    private void InitializeSpawner(Transform[] spawnPoints)
    {
        enemySpawner = new EnemySpawner(spawnPoints);

        var meleePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Developers Folder/Entities/Characters/Enemy/Melee/Warrior/DEBUG_Warrior_Enemy.prefab");

        var rangedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Developers Folder/Entities/Characters/Enemy/Range/Gunner/DEBUG_Gunner_Enemy.prefab");

        var sword = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Developers Folder/Weapons/Melee/Katana/Prefab/CyberKatana.prefab");

        var muramasa = AssetDatabase.LoadAssetAtPath<GameObject>
            ("Assets/Developers Folder/Weapons/Melee/Katana_Muramasa/Prefab/Katana_Muramasa.prefab");

        var blaster = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Developers Folder/Weapons/Range/Blaster/Prefab/Blaster.prefab");

        var blaster_carabin = AssetDatabase.LoadAssetAtPath<GameObject>
            ("Assets/Developers Folder/Weapons/Range/Blaster_Carabin/Prefab/Blaster_Carabin.prefab");


        if (meleePrefab == null) Debug.LogError("Melee enemy prefab not found!");
        if (rangedPrefab == null) Debug.LogError("Ranged enemy prefab not found!");

        enemySpawner.AddMeleeEnemy(meleePrefab, new List<GameObject> { sword, muramasa });
        enemySpawner.AddRangedEnemy(rangedPrefab, new List<GameObject> { blaster, blaster_carabin });

        enemySpawner.SetSpawnParameters(5f, 10);
        enemySpawner.OnObjectSpawned += OnEnemySpawned;
        enemySpawner.StartSpawning();
    }

    private void OnEnemySpawned(GameObject enemy)
    {
        var gunner = enemy.GetComponent<GunnerAI>();
        var warrior = enemy.GetComponent<WarriorAI>();

        SpawnConfig config = null;
        if (gunner != null)
        {
            gunner.Initialize(sfxManager);
            EntityAgregator.AddEntity(enemy, Enum_EntityType.Range);
            config = enemySpawner.GetLastSpawnedConfig();
        }
        else if (warrior != null)
        {
            warrior.Initialize(sfxManager);
            EntityAgregator.AddEntity(enemy, Enum_EntityType.Melee);
            config = enemySpawner.GetLastSpawnedConfig();
        }

        if (config != null && config.VariantsWeapons.Count > 0)
        {
            var weaponPrefab = config.VariantsWeapons[Random.Range(0, config.VariantsWeapons.Count)];
            var weaponInstance = GameObject.Instantiate(weaponPrefab);

            var socket = enemy.GetComponentsInChildren<Transform>().FirstOrDefault(t => t.name == "WeaponSocket");

            if (socket != null)
            {
                weaponInstance.transform.SetParent(socket);
                weaponInstance.transform.localPosition = Vector3.zero;
                weaponInstance.transform.localRotation = Quaternion.identity;
                weaponInstance.transform.localScale = Vector3.one;
            }
            else
            {
                Debug.LogWarning($"WeaponSocket �� ������ � ����� {enemy.name}");
            }
        }

        EntityCoreCreator.EntityCoreCreation(enemy, uiManager);
        enemy.GetComponent<EntityCoreSystem>().GetHealthSystem().OnDeath += EventHub.AddDeathCall;
    }




    private void InitializeBoss(GameObject bossPrefab, ManagerUI managerUI)
    {
        BossSpawner bossSpawner = GameObject.FindFirstObjectByType<BossSpawner>();
        int element = Random.Range(1, 4);
        if (element == 1)
            bossSpawner.SpawnBoss(bossPrefab, new LightningKatanaFactory(), new LightningMagicProjectileFactory(), managerUI);
        else if (element == 2)
            bossSpawner.SpawnBoss(bossPrefab, new FireKatanaFactory(), new FireMagicProjectileFactory(), managerUI);
        else if (element == 3)
            bossSpawner.SpawnBoss(bossPrefab, new WindKatanaFactory(), new WindMagicProjectileFactory(), managerUI);
        else if (element == 4)
            bossSpawner.SpawnBoss(bossPrefab, new SpaceKatanaFactory(), new SpaceMagicProjectileFactory(), managerUI);
    }


    private void InitializeEnemies(ManagerSFX managerSFX, ManagerUI managerUI, List<EntitySaveData> data)
    {

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            GunnerAI gunner = enemy.GetComponent<GunnerAI>();
            WarriorAI warrior = enemy.GetComponent<WarriorAI>();
            if (gunner)
            {
                gunner.Initialize(managerSFX);
                EntityAgregator.AddEntity(enemy, Enum_EntityType.Range);
            }
            else if (warrior)
            {
                warrior.Initialize(managerSFX);
                EntityAgregator.AddEntity(enemy, Enum_EntityType.Melee);
            }
            if (data != null)
            {
                EntitySaveData entityData;
                if (gunner)
                {
                    entityData = data.FirstOrDefault(x => x.EntityType == Enum_EntityType.Range);
                    data.Remove(entityData);
                }
                else
                {
                    entityData = data.FirstOrDefault(x => x.EntityType == Enum_EntityType.Melee);
                    data.Remove(entityData);
                }
                EntityCoreCreator.EntityCoreCreation(enemy, managerUI, entityData.CoreData);
                enemy.GetComponent<EntityCoreSystem>().GetHealthSystem().OnDeath += EventHub.AddDeathCall;
                enemy.transform.position = entityData.Position;
                enemy.transform.rotation = entityData.Rotation;
            }
            else EntityCoreCreator.EntityCoreCreation(enemy, managerUI);
            enemy.GetComponent<EntityCoreSystem>().GetHealthSystem().OnDeath += EventHub.AddDeathCall;
        }
    }


}