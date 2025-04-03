using UnityEngine;

public class HealingSpell : MagicInfluenceBase
{
    private int _healAmount;

    public HealingSpell(float castTime, int manaCost, int healAmount, string audioEffectAddress)
        : base(castTime, manaCost, audioEffectAddress)
    {
        _healAmount = healAmount;
        OnActivateMagic += HealSpellActivation;
    }


    private void HealSpellActivation(GameObject caster, GameObject target)
    {
        if (target != null)
        {
            // Heal the target if it has an I_Health component
            target.GetComponent<EntityCoreSystem>()?.GetHealthSystem().Heal(_healAmount);
            Debug.Log($"Healed target {target.name} for {_healAmount} health.");
        }
        else
        {
            // Heal the caster if no target is specified
            caster.GetComponent<EntityCoreSystem>()?.GetHealthSystem().Heal(_healAmount);
            Debug.Log($"Healed caster {caster.name} for {_healAmount} health.");
        }
    }
}