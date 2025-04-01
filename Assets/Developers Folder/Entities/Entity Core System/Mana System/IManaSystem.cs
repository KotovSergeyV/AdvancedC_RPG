using UnityEngine;

public interface I_Mana
{
    int RemoveMana(int amount);
    void AddMana(int amount);
    int GetMana();
}
