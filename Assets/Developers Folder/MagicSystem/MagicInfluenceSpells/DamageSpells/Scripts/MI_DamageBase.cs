using UnityEngine;

public class DamageMagic : MagicInfluence
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
            I_DamageDealler damageDealler = caster.GetComponent<I_DamageDealler>();
            damageDealler?.Damage(_target, _damageData);
            Debug.Log("Damage dealt to target: " + _target.name);
        }
    }
}