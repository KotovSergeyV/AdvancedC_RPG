public static class MagicFactory
{
    // Create a basic magic instance
    public static MagicBase CreateMagic(float castTime, int manaCost)
    {
        return new MagicBase(castTime, manaCost);
    }

    // Create a magic influence instance
    public static MagicInfluence CreateMagicInfluence(float castTime, int manaCost)
    {
        return new MagicInfluence(castTime, manaCost);
    }

    // Create a damage magic instance
    public static DamageMagic CreateDamageMagic(float castTime, int manaCost, Struct_DamageData damageData)
    {
        return new DamageMagic(castTime, manaCost, damageData);
    }
}