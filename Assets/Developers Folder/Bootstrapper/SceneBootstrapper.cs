using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SceneBootstrapper 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize(ManagerSFX managerSFX, ManagerUI managerUI, List<EntitySaveData> data =null)
    {

        InitializeEnemies(managerSFX, managerUI, data);
    }

    private void InitializeEnemies(ManagerSFX managerSFX, ManagerUI managerUI, List<EntitySaveData> data)
    {

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            GunnerAI gunner = enemy.GetComponent<GunnerAI>();
            if (gunner)
            {
                gunner.Initialize(managerSFX);
                EntityAgregator.AddEntity(enemy, Enum_EntityType.Range);
            }
            else
            {
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
    

    private EntityCoreSystem EntityCoreCreation(GameObject entity, ManagerUI managerUI)
    {
        EntityCoreSystem entityCoreSystem = entity.AddComponent<EntityCoreSystem>();

        HealthBar healthBar = entity?.GetComponentInChildren<HealthBar>();
        ManaBar manaBar = entity?.GetComponentInChildren<ManaBar>();

        entityCoreSystem.Initialize(new HealthSystem(managerUI, 100, healthBar), new DamageCalculationSystem(), new ManaSystem(managerUI, 100, 0.5f, manaBar),
            new StatSystem(1, 1, 1, 1, 1), new EntityStatesSystem(), new Movable());
        try
        {
            IHealthSystem healthSystem = (entityCoreSystem.GetHealthSystem());
            ((HealthSystem)healthSystem).OnDamaged += entity.GetComponent<AnimatorController>().PlayHitAnimation;
            ((HealthSystem)healthSystem).OnDead += entity.GetComponent<AnimatorController>().PlayDeathAnimation;

            Debug.Log("Initial HP:" + healthSystem.GetHp());
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
            new EntityStatesSystem(), // <---- current state here after it released in game
            new Movable());
        try
        {
            IHealthSystem healthSystem = (entityCoreSystem.GetHealthSystem());
            ((HealthSystem)healthSystem).OnDamaged += entity.GetComponent<AnimatorController>().PlayHitAnimation;
            ((HealthSystem)healthSystem).OnDead += entity.GetComponent<AnimatorController>().PlayDeathAnimation;

            Debug.Log("Initial HP:" + healthSystem.GetHp());
        }
        catch { Debug.Log("Damage/Death anim assignation error!"); }


        return entityCoreSystem;
    }
}
