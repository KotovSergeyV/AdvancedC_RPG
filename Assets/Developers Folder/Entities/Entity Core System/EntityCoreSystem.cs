using UnityEngine;

public class EntityCoreSystem : MonoBehaviour
{
    IHealthSystem _healthSystem;
    IDamageCalculationSystem _damageCalculationSystem;
    IManaSystem _manaSystem;
    IStatSystem _statSystem;
    IEntityStatesSystem _statesSystem;
    IMovable _moveSystem;

    public void Initialize(IHealthSystem healthSystem, IDamageCalculationSystem damageCalculationSystem,
        IManaSystem manaSystem, IStatSystem statSystem, IEntityStatesSystem statesSystem, IMovable moveSystem)
    {
        _healthSystem = healthSystem;
        _damageCalculationSystem = damageCalculationSystem;
        _manaSystem = manaSystem;
        _statSystem = statSystem;
        _statesSystem = statesSystem;
        _moveSystem = moveSystem;
    }

    public IHealthSystem GetHealthSystem() { return _healthSystem; }
    public IManaSystem GetManaSystem() { return _manaSystem; }
    public IDamageCalculationSystem GetDamageCalculationSystem() { return _damageCalculationSystem; }
    public IStatSystem GetStatSystem() { return _statSystem; }
    public IEntityStatesSystem GetStatesSystem() { return _statesSystem; }
    public IMovable GetMoveSystem() { return _moveSystem; }
}
