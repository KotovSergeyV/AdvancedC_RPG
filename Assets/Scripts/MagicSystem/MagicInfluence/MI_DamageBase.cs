using UnityEngine;

public class DamageBase : MagicInfluence
{
    [SerializeField] private Struct_DamageData _damageData;

    void DamageSpellActivation()
    {
        Debug.Log("Active");
        if (_target != null)
        {
            GetComponent<I_DamageDealler>()?.Damage(_target, _damageData);
        }
    }

    private void Start()
    {
        OnActivateMagic += DamageSpellActivation;
    }
}
