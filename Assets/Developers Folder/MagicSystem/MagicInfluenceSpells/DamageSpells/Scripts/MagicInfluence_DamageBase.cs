using UnityEngine;

public class DamageMagic : MagicInfluenceBase
{
    private Struct_DamageData _damageData;

    public DamageMagic(float castTime, int manaCost, Struct_DamageData damageData, string audioEffectAddress)
        : base(castTime, manaCost, audioEffectAddress)
    {
        _damageData = damageData;
        OnActivateMagic += DamageSpellActivation;
    }

    private void DamageSpellActivation(GameObject caster, GameObject target)
    {
        if (_target != null)
        {
            IDamageCalculationSystem damageDealler = caster.GetComponent<EntityCoreSystem>().GetDamageCalculationSystem();
            damageDealler?.Damage(_target, _damageData);
            Debug.Log("Damage dealt to target: " + _target.name);
        }
    }
}