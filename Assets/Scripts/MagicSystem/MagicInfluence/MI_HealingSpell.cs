using UnityEngine;

public class HealingSpell : MagicInfluence
{
    [SerializeField] int _healAmmount;

    void HealSpellActivation()
    {
        if (_target != null)
        {
            _target.GetComponent<I_Health>()?.Heal(_healAmmount);
        }
        else 
        {
            GetComponent<I_Health>()?.Heal(_healAmmount);
        }
    }

    private void OnEnable()
    {
        OnActivateMagic += HealSpellActivation;
    }
}
