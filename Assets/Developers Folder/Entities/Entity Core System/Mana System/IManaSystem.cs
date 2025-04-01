using UnityEngine;

public interface IManaSystem
{
    int RemoveMana(int amount);
    void AddMana(int amount);
    int GetMana();
}
