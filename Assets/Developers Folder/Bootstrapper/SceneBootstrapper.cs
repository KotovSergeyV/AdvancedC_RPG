using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SceneBootstrapper 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize(ManagerSFX managerSFX, ManagerUI managerUI)
    {
        InitializeEnemies(managerSFX, managerUI);
    }

    private void InitializeEnemies(ManagerSFX managerSFX, ManagerUI managerUI)
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            EntityCoreCreation(enemy, managerUI);
            if (enemy.GetComponent<GunnerAI>())
            {
                enemy.GetComponent<GunnerAI>().Initialize(managerSFX);
                EntityAgregator.AddEntity(enemy, Enum_EntityType.Range);
            }
            else
            {
                EntityAgregator.AddEntity(enemy, Enum_EntityType.Melee);
            }
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
}
