using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;


[Serializable]
public class EntityCoreSystem : MonoBehaviour
{
    IHealthSystem _healthSystem;
    IDamageCalculationSystem _damageCalculationSystem;
    IManaSystem _manaSystem;
    IStatSystem _statSystem;
    IEntityStatesSystem _statesSystem;

    public void Initialize(IHealthSystem healthSystem, IDamageCalculationSystem damageCalculationSystem,
        IManaSystem manaSystem, IStatSystem statSystem, IEntityStatesSystem statesSystem)
    {
        _healthSystem = healthSystem;
        _damageCalculationSystem = damageCalculationSystem;
        _manaSystem = manaSystem;
        _statSystem = statSystem;
        _statesSystem = statesSystem;
    }


    public IHealthSystem GetHealthSystem() { return _healthSystem; }
    public IManaSystem GetManaSystem() { return _manaSystem; }
    public IDamageCalculationSystem GetDamageCalculationSystem() { return _damageCalculationSystem; }
    public IStatSystem GetStatSystem() { return _statSystem; }
    public IEntityStatesSystem GetStatesSystem() { return _statesSystem; }

}


public static class EntityCoreCreator
{
    
    public static EntityCoreSystem EntityCoreCreation(GameObject entity, ManagerUI managerUI, int maxHp=100,
        int maxMana=100, float regenTime=0.5f, int agi=1, int atc=1, int luck=1, int def=1, int intl=1)
    {
        EntityCoreSystem entityCoreSystem = entity.AddComponent<EntityCoreSystem>();

        HealthBar healthBar = entity?.GetComponentInChildren<HealthBar>();
        ManaBar manaBar = entity?.GetComponentInChildren<ManaBar>();

        entityCoreSystem.Initialize(new HealthSystem(managerUI, maxHp, healthBar), new DamageCalculationSystem(), 
            new ManaSystem(managerUI, maxMana, regenTime, manaBar),
            new StatSystem(agi, atc, luck, def, intl), new EntityStatesSystem());
        try
        {
            IHealthSystem healthSystem = (entityCoreSystem.GetHealthSystem());
            healthSystem.OnDamaged += entity.GetComponent<AnimatorController>().PlayHitAnimation;
            healthSystem.OnDeath += entity.GetComponent<AnimatorController>().PlayDeathAnimation;

        }
        catch { Debug.Log("Damage/Death anim assignation error!"); }


        return entityCoreSystem;
    }
    
    public static EntityCoreSystem EntityCoreCreation(GameObject entity, ManagerUI managerUI, CoreData coreData)
    {
        EntityCoreSystem entityCoreSystem = entity.AddComponent<EntityCoreSystem>();

        HealthBar healthBar = entity?.GetComponentInChildren<HealthBar>();
        ManaBar manaBar = entity?.GetComponentInChildren<ManaBar>();

        entityCoreSystem.Initialize(new HealthSystem(managerUI, coreData.HealthData.MaxHealth, 
                healthBar, coreData.HealthData.Health),
            new DamageCalculationSystem(),
            new ManaSystem(managerUI, coreData.ManaData.MaxMana, 0.5f, manaBar, coreData.ManaData.Mana),
            new StatSystem(coreData.StatData.Agility, coreData.StatData.Attack, coreData.StatData.Luck, 
                coreData.StatData.Defence, coreData.StatData.Intelligence),
            new EntityStatesSystem() // <---- current state here after it released in game
            );
        try
        {
            IHealthSystem healthSystem = (entityCoreSystem.GetHealthSystem());
            healthSystem.OnDamaged += entity.GetComponent<AnimatorController>().PlayHitAnimation;
            healthSystem.OnDeath += entity.GetComponent<AnimatorController>().PlayDeathAnimation;

        }
        catch { Debug.Log("Damage/Death anim assignation error!"); }


        return entityCoreSystem;
    }
    
    
}