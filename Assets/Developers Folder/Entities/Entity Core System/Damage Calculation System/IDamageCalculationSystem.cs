using UnityEngine;

public interface IDamageCalculationSystem
{
    int Damage(GameObject target, Struct_DamageData damageData, IStatSystem ownerStats = null) { return 0; }
}
