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

    public HealthSystem(ManagerUI managerUI, int maxHp, HealthBar healthBar, int currentHealth=-1)
    {
        _managerUI = managerUI;
        _maxHealth = maxHp;
        _health = currentHealth == -1 ? maxHp : currentHealth;
        _isDead = false;

        _managerUI.RegisterHealthBar(this, healthBar);
        
        UpdateCanvas();

    }


    private void UpdateCanvas()
    {
        _managerUI?.UpdateCanvasHp(this, _health, _maxHealth);
    }

    // Functions from interface
    public int GetHp()    {  return _health; }
    public int GetMaxHp()    {  return _maxHealth; }
    public bool GetIsDead()    { return _isDead; }
    public int Damage(int amount)   
    {
        if (_isDead) return 0;

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

        Debug.Log("Current hp: "+ _health);

        _health = Mathf.Min(_health+amount, _maxHealth);
        UpdateCanvas();
        Debug.Log("Healed on " + amount + "hp");
        Debug.Log("New current hp: " + _health);
    }
}
