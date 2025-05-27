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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize(ManagerSFX managerSFX, ManagerUI managerUI, Transform[] spawnPoints = null, List<EntitySaveData> data =null)
    {
        sfxManager = managerSFX;
        uiManager = managerUI;

        InitializeEnemies(managerSFX, managerUI, data);
        InitializeBoss(managerSFX, managerUI);
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
                Debug.LogWarning($"WeaponSocket не найден у врага {enemy.name}");
            }
        }

        EntityCoreCreation(enemy, uiManager);
    }


    private void InitializeBoss(ManagerSFX managerSFX, ManagerUI managerUI)
    {
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        EntityCoreCreation(boss, managerUI, 500, 500, 1, 3, 15, 3, 3, 3);
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
            if (data != null) {
                EntitySaveData entityData;
                if (gunner) {
                    entityData = data.FirstOrDefault(x => x.EntityType == Enum_EntityType.Range);
                    data.Remove(entityData);
                }
                else {
                    entityData = data.FirstOrDefault(x => x.EntityType == Enum_EntityType.Melee);
                    data.Remove(entityData);
                }
                EntityCoreCreation(enemy, managerUI, entityData.CoreData);
                enemy.transform.position = entityData.Position;
                enemy.transform.rotation = entityData.Rotation;
            }
            else EntityCoreCreation(enemy, managerUI);
        } 
    }
    
    private EntityCoreSystem EntityCoreCreation(GameObject entity, ManagerUI managerUI, int maxHp, int maxMana, float 
        regenTime, int agi, int atc, int luck, int def, int intl)
    {
        EntityCoreSystem entityCoreSystem = entity.AddComponent<EntityCoreSystem>();

        HealthBar healthBar = entity?.GetComponentInChildren<HealthBar>();
        ManaBar manaBar = entity?.GetComponentInChildren<ManaBar>();

        entityCoreSystem.Initialize(new HealthSystem(managerUI, maxHp, healthBar), new DamageCalculationSystem(), new ManaSystem(managerUI, maxMana, regenTime, manaBar),
            new StatSystem(agi, atc, luck, def, intl), new EntityStatesSystem());
        try
        {
            IHealthSystem healthSystem = (entityCoreSystem.GetHealthSystem());
            ((HealthSystem)healthSystem).OnDamaged += entity.GetComponent<AnimatorController>().PlayHitAnimation;
            ((HealthSystem)healthSystem).OnDeath += entity.GetComponent<AnimatorController>().PlayDeathAnimation;

        }
        catch { Debug.Log("Damage/Death anim assignation error!"); }


        return entityCoreSystem;
    }
    
    private EntityCoreSystem EntityCoreCreation(GameObject entity, ManagerUI managerUI)
    {
        EntityCoreSystem entityCoreSystem = entity.AddComponent<EntityCoreSystem>();

        HealthBar healthBar = entity?.GetComponentInChildren<HealthBar>();
        ManaBar manaBar = entity?.GetComponentInChildren<ManaBar>();

        entityCoreSystem.Initialize(new HealthSystem(managerUI, 100, healthBar), new DamageCalculationSystem(), new ManaSystem(managerUI, 100, 0.5f, manaBar),
            new StatSystem(1, 1, 1, 1, 1), new EntityStatesSystem());
        try
        {
            IHealthSystem healthSystem = (entityCoreSystem.GetHealthSystem());
            ((HealthSystem)healthSystem).OnDamaged += entity.GetComponent<AnimatorController>().PlayHitAnimation;
            ((HealthSystem)healthSystem).OnDeath += entity.GetComponent<AnimatorController>().PlayDeathAnimation;

        }
        catch { Debug.Log("Damage/Death anim assignation error!"); }


        return entityCoreSystem;
    }

    private EntityCoreSystem EntityCoreCreation(GameObject entity, ManagerUI managerUI, CoreData coreData)
    {
        EntityCoreSystem entityCoreSystem = entity.AddComponent<EntityCoreSystem>();

        HealthBar healthBar = entity?.GetComponentInChildren<HealthBar>();
        ManaBar manaBar = entity?.GetComponentInChildren<ManaBar>();

        entityCoreSystem.Initialize(new HealthSystem(managerUI, coreData.HealthData.MaxHealth, healthBar, coreData.HealthData.Health),
            new DamageCalculationSystem(),
            new ManaSystem(managerUI, coreData.ManaData.MaxMana, 0.5f, manaBar, coreData.ManaData.Mana),
            new StatSystem(coreData.StatData.Agility, coreData.StatData.Attack, coreData.StatData.Luck, coreData.StatData.Defence, coreData.StatData.Intelligence),
            new EntityStatesSystem() // <---- current state here after it released in game
            );
        try
        {
            IHealthSystem healthSystem = (entityCoreSystem.GetHealthSystem());
            ((HealthSystem)healthSystem).OnDamaged += entity.GetComponent<AnimatorController>().PlayHitAnimation;
            ((HealthSystem)healthSystem).OnDeath += entity.GetComponent<AnimatorController>().PlayDeathAnimation;

        }
        catch { Debug.Log("Damage/Death anim assignation error!"); }


        return entityCoreSystem;
    }
}
