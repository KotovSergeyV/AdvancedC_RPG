using UnityEngine;

public class HealthComponent : MonoBehaviour, I_Health
{
    //debug 
    [SerializeField] private int _health;
    [SerializeField] private int _maxHealth;
    [SerializeField] private bool _isDead;


    // Functions from interface
    public int GetHp()    { return _health; }
    public bool GetIsDead()    { return _isDead; }
    public int Damage(int amount)   
    { 
        _health = Mathf.Max(_health-amount, 0);
        return _health;
    }
    public void Heal(int amount)
    {
        _health = Mathf.Min(_health+amount, _maxHealth);
    }
}
