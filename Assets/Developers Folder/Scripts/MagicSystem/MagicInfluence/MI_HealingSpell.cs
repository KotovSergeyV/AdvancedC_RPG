using UnityEngine;

public class HealingSpell : MagicInfluence
{
    private int _healAmount;

    public HealingSpell(float castTime, int manaCost, int healAmount)
        : base(castTime, manaCost)
    {
        _healAmount = healAmount;
        OnActivateMagic += HealSpellActivation;
    }

    private void HealSpellActivation(GameObject caster, GameObject target)
    {
        if (target != null)
        {
            // Heal the target if it has an I_Health component
            target.GetComponent<I_Health>()?.Heal(_healAmount);
            Debug.Log($"Healed target {target.name} for {_healAmount} health.");
        }
        else
        {
            // Heal the caster if no target is specified
            caster.GetComponent<I_Health>()?.Heal(_healAmount);
            Debug.Log($"Healed caster {caster.name} for {_healAmount} health.");
        }
    }
}