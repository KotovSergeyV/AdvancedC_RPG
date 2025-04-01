public static class MagicFactory
{
    // Create a basic magic instance
    public static MagicBase CreateMagic(float castTime, int manaCost)
    {
        return new MagicBase(castTime, manaCost);
    }

    // Create a magic influence instance
    public static MagicInfluenceBase CreateMagicInfluence(float castTime, int manaCost)
    {
        return new MagicInfluenceBase(castTime, manaCost);
    }

    // Create a damage magic instance
    public static DamageMagic CreateDamageMagic(float castTime, int manaCost, Struct_DamageData damageData)
    {
        return new DamageMagic(castTime, manaCost, damageData);
    }

    // Create a healling spell instance
    public static HealingSpell CreateHealingSpell(float castTime, int manaCost, int healAmount)
    {
        return new HealingSpell(castTime, manaCost, healAmount);
    }
}