using System;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

public class ManaSystem : IManaSystem
{
    //debug 
    [SerializeField] private int _mana;
    [SerializeField] private int _maxMana;
    [SerializeField] private float _manaRegenTime;
    [SerializeField] private bool _isRegening;

    ManagerUI _managerUI;

    public ManaSystem(ManagerUI managerUI, int maxMana, float regenTime, ManaBar manaBar) 
    {
        _managerUI = managerUI;
        _maxMana = maxMana;
        _mana = maxMana;
        _managerUI.RegisterManaBar(this, manaBar);
        _manaRegenTime = regenTime;
    }

    private async Task RegenAsync()
    {
        _isRegening = true;
        while (_mana < _maxMana)
        {
            await Task.Delay((int)(_manaRegenTime * 1000));
            _mana += 1;
            UpdateCanvas();
        }
        _isRegening = false;
    }

    private void UpdateCanvas()
    {
        _managerUI?.UpdateCanvasMana(this, _mana, _maxMana);
    }

    // Functions from interface
    public int GetMana()    { UpdateCanvas(); Debug.Log("Мана " + _mana); return _mana; }
    public int RemoveMana(int amount)   
    {
        UpdateCanvas();
        _mana = Mathf.Max(_mana - amount, 0);

        if (!_isRegening) { RegenAsync(); }
        return _mana;
    }


    public void AddMana(int amount)
    {
        UpdateCanvas();
        _mana = Mathf.Min(_mana + amount, _maxMana);
    }
}
