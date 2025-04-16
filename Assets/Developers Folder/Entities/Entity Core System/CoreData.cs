using System;


[Serializable]
public class CoreData
{
    public HealthData HealthData = new HealthData();
    public ManaData ManaData = new ManaData();
    public StatData StatData = new StatData();
    public StateData StateData = new StateData();

    public void Initialise(EntityCoreSystem core)
    {

        HealthData.MaxHealth = core.GetHealthSystem().GetMaxHp();
        HealthData.Health = core.GetHealthSystem().GetHp();

        ManaData.MaxMana = core.GetManaSystem().GetMaxMana();
        ManaData.Mana = core.GetManaSystem().GetMana();

        StatData.Attack = core.GetStatSystem().GetAttack();
        StatData.Agility = core.GetStatSystem().GetAgility();
        StatData.Defence = core.GetStatSystem().GetDefence();
        StatData.Luck = core.GetStatSystem().GetLuck();
        StatData.Intelligence = core.GetStatSystem().GetInteligence();

        StateData.State = core.GetStatesSystem().GetEntityState();
    }
}


[Serializable]
public class HealthData
{
    public int MaxHealth;
    public int Health;
}


[Serializable]
public class ManaData
{
    public int MaxMana;
    public int Mana;
}

[Serializable]
public class StatData
{
    public int Attack;
    public int Defence;
    public int Agility;
    public int Luck;
    public int Intelligence;
}

public class StateData 
{
    public Enum_EntityStates State;
}