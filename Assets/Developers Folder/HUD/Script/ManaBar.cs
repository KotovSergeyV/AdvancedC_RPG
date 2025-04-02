using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    [SerializeField] private Slider _manaBar;

    private void Awake()
    {
        _manaBar = GetComponentInChildren<Slider>();
    }

    public void SetMana(int currentHealth, int maxHealth)
    {
        _manaBar.maxValue = maxHealth;
        _manaBar.value = currentHealth;
    }
}