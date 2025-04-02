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

    private ManagerUI _managerUI;

    public HealthSystem(ManagerUI managerUI, int maxHp, HealthBar healthBar)
    {
        _managerUI = managerUI;
        _maxHealth = maxHp;
        _health = maxHp;
        _isDead = false;

        _managerUI.RegisterHealthBar(this, healthBar);
        UpdateCanvas();

    }

    private void UpdateCanvas()
    {
        _managerUI?.UpdateCanvasHp(this, _health, _maxHealth);
    }

    // Functions from interface
    public int GetHp()    { UpdateCanvas();  return _health; }
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
        GetHp();

        UpdateCanvas();

        _health = Mathf.Min(_health+amount, _maxHealth);
    }
}
