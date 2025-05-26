using UnityEngine;
using System;

public interface IHealthSystem
{

    public event Action OnDeath;
    public event Action OnDamaged;
    int Damage(int amount);
    void Heal(int amount);
    bool GetIsDead();
    int GetHp();
    int GetMaxHp();


}
