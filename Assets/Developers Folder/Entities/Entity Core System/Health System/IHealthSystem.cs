using UnityEngine;
using System;

public interface IHealthSystem
{

    int Damage(int amount);
    void Heal(int amount);
    bool GetIsDead();
    int GetHp();
    int GetMaxHp();


}
