using UnityEngine;

public interface IHealthSystem
{
    int Damage(int amount);
    void Heal(int amount);
    bool GetIsDead();
    int GetHp();
}
