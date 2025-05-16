using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.EventSystems.EventTrigger;

public class SceneBootstrapper 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize(ManagerSFX managerSFX, ManagerUI managerUI, List<EntitySaveData> data =null)
    {

        InitializeEnemies(managerSFX, managerUI, data);
        InitializeBoss(managerSFX, managerUI);
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
                    entityData = data
                        .FirstOrDefault(x => x.EntityType == Enum_EntityType.Range);
                    data.Remove(entityData);
                }
                else {
                    entityData = data
                        .FirstOrDefault(x => x.EntityType == Enum_EntityType.Melee);
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
            ((HealthSystem)healthSystem).OnDead += entity.GetComponent<AnimatorController>().PlayDeathAnimation;

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
            ((HealthSystem)healthSystem).OnDead += entity.GetComponent<AnimatorController>().PlayDeathAnimation;

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
            ((HealthSystem)healthSystem).OnDead += entity.GetComponent<AnimatorController>().PlayDeathAnimation;

        }
        catch { Debug.Log("Damage/Death anim assignation error!"); }


        return entityCoreSystem;
    }
}
