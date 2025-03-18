using UnityEngine;

public class ManaComponent : MonoBehaviour, I_Mana
{
    //debug 
    [SerializeField] private int _mana;
    [SerializeField] private int _maxMana;


    // Functions from interface
    public int GetMana()    { return _mana; }
    public int RemoveMana(int amount)   
    {
        _mana = Mathf.Max(_mana - amount, 0);
        return _mana;
    }
    public void AddMana(int amount)
    {
        _mana = Mathf.Min(_mana + amount, _maxMana);
    }
}
