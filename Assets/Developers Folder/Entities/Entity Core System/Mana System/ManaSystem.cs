using UnityEngine;

public class ManaSystem : IManaSystem
{
    //debug 
    [SerializeField] private int _mana;
    [SerializeField] private int _maxMana;


    // Functions from interface
    public int GetMana()    { return _mana; }
    public int RemoveMana(int amount)   
    {
        _mana = Mathf.Max(_mana - amount, 0);
        ManagerUI.Instance?.UpdateCanvasMana(_mana, _maxMana);
        return _mana;
    }
    public void AddMana(int amount)
    {
        _mana = Mathf.Min(_mana + amount, _maxMana);
    }
}
