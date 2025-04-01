using UnityEngine;

public interface HealthSystem
{
    int Damage(int amount);
    void Heal(int amount);
    bool GetIsDead();
    int GetHp();
}
