using UnityEngine;

public class DEBUG_MagicCasterUsage : MonoBehaviour
{
    private MagicCaster _magicCaster;
    [SerializeField] private GameObject _target;

    private void Start()
    {
        // Get the MagicCaster component
        _magicCaster = GetComponent<MagicCaster>();

        // Example: Create a damage magic instance using the factory
        Struct_DamageData damageData = new Struct_DamageData()
        {
            DamageAmount = 10,
            DamageType = Enum_DamageTypes.Magic,
            isBlockable = false,
            isInneviåtable = true,
            Responce = Enum_DamageResponses.SmallStun
        };

        // DamageMagic damageMagic = MagicFactory.CreateDamageMagic(2.0f, 20, damageData);
        HealingSpell damageMagic = MagicFactory.CreateHealingSpell(2.0f, 20, 15);
        // Assign the magic to the caster
        _magicCaster.SetMagic(damageMagic);

        // Set the target (e.g., an enemy in the scene)
        _magicCaster.SetTarget(_target);

        // Cast the magic
        _magicCaster.InitiateCast();

    }
}
