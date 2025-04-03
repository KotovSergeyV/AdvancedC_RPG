using UnityEngine;

public class StatSystem : IStatSystem
{
    int _agility;
    int _attack;
    int _luack;
    int _defence;
    int _intelligence;

    public StatSystem(int agi, int atc, int luck, int def, int intl)
    {
        _agility = agi;
        _attack = atc;  
        _luack = luck;
        _defence = def;
        _intelligence = luck;
    }

    public int GetAgility() { return _agility; }
    public int GetAttack() { return _attack; }
    public int GetLuck() { return _luack; }
    public int GetDefence() { return _defence; }
    public int GetInteligence() { return _intelligence; }
}
