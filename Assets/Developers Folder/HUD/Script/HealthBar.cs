using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _healthBar;

    private void Awake()
    {
        _healthBar = GetComponentInChildren<Slider>();
    }

    public void SetHealth(int currentHealth, int maxHealth)
    {
        if (_healthBar == null) return;
        _healthBar.maxValue = maxHealth;
        _healthBar.value = currentHealth;
    }
}