using UnityEngine;

public class StatSystem : IStatSystem
{
    int _agility;
    int _attack;
    int _luck;
    int _defence;
    int _intelligence;

    public StatSystem(int agi, int atc, int luck, int def, int intl)
    {
        _agility = agi;
        _attack = atc;  
        _luck = luck;
        _defence = def;
        _intelligence = luck;
    }

    public int GetAgility() { return _agility; }
    public int GetAttack() { return _attack; }
    public int GetLuck() { return _luck; }
    public int GetDefence() { return _defence; }
    public int GetInteligence() { return _intelligence; }
}
