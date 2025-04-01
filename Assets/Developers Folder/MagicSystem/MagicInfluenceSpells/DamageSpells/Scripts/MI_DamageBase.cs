using UnityEngine;

public class DamageMagic : MagicInfluenceBase
{
    private Struct_DamageData _damageData;

    public DamageMagic(float castTime, int manaCost, Struct_DamageData damageData)
        : base(castTime, manaCost)
    {
        _damageData = damageData;
        OnActivateMagic += DamageSpellActivation;
    }

    private void DamageSpellActivation(GameObject caster, GameObject target)
    {
        if (_target != null)
        {
            IDamageCalculationSystem damageDealler = caster.GetComponent<IDamageCalculationSystem>();
            damageDealler?.Damage(_target, _damageData);
            Debug.Log("Damage dealt to target: " + _target.name);
        }
    }
}