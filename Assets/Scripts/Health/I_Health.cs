using UnityEngine;

public interface I_Health
{
    int Damage(int amount);
    void Heal(int amount);
    bool GetIsDead();
    int GetHp();
}
