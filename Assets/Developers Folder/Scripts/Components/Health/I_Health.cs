using UnityEngine;

public interface I_Health // А к чему эти интерфейсы?
{
    int Damage(int amount);
    void Heal(int amount);
    bool GetIsDead();
    int GetHp();
}
