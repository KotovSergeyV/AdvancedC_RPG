using System;
using UnityEngine;

public class HealthSystem : IHealthSystem
{
    //debug 
    private int _health;
    private int _maxHealth;
    private bool _isDead;

    public Action OnDamaged;
    public Action OnDead;
    public Action<int, int> OnChangedUI;

    private ManagerUI _managerUI;

    public HealthSystem(ManagerUI managerUI, int maxHp)
    {
        _managerUI = managerUI;
        _maxHealth = maxHp;
        _health = maxHp;
        _isDead = false;

        UpdateCanvas();

    }

    private void UpdateCanvas()
    {
        OnChangedUI?.Invoke(_health, _maxHealth);
        _managerUI?.UpdateCanvasHp(_health, _maxHealth);
    }
    // Functions from interface
    public int GetHp()    { Debug.Log("Çהמנמגüו " + _health); return _health; }
    public bool GetIsDead()    { return _isDead; }
    public int Damage(int amount)   
    {
        GetHp();

        _health = Mathf.Max(_health-amount, 0);

        OnDamaged?.Invoke();

        UpdateCanvas();

        if (_health == 0) 
        { 
            _isDead = true;
            OnDead?.Invoke(); 
        }   
        return _health;
    }
    public void Heal(int amount)
    {
        UpdateCanvas();
        Debug.Log("Çהמנמגüו " + _health);

        _health = Mathf.Min(_health+amount, _maxHealth);
    }
}
