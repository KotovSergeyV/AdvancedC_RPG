using System.Collections.Generic;
using UnityEngine;

public class ManagerUI : BaseManager<ManagerUI>
{
    private GameObject playerCanvas;
    private Dictionary<IHealthSystem, HealthBar> healthBars = new Dictionary<IHealthSystem, HealthBar>();
    private Dictionary<IManaSystem, ManaBar> manaBars = new Dictionary<IManaSystem, ManaBar>();

    private new void Awake()
    {
        playerCanvas = GameObject.FindGameObjectWithTag("PlayerCanvas");
    }

    public override void Initialize()
    {
        playerCanvas = GameObject.FindGameObjectWithTag("PlayerCanvas");

        if (playerCanvas == null)
        {
            Debug.LogError("PlayerCanvas не найден! Проверь, загружен ли он в сцене.");
            return;
        }

        Debug.Log("UIManager Initialized: PlayerCanvas найден.");
    }

    public void RegisterHealthBar(IHealthSystem healthSystem, HealthBar healthBar)
    {
        if (healthSystem == null || healthBar == null) return;

        if (!healthBars.ContainsKey(healthSystem))
        {
            healthBars[healthSystem] = healthBar;
        }
    }

    public void UnregisterHealthBar(IHealthSystem healthSystem)
    {
        if (healthBars.ContainsKey(healthSystem))
        {
            healthBars.Remove(healthSystem);
        }
    }

    public void UpdateCanvasHp(IHealthSystem healthSystem, int currentHealth, int maxHealth)
    {
        if (healthBars.TryGetValue(healthSystem, out HealthBar targetHealthBar))
        {
            targetHealthBar.SetHealth(currentHealth, maxHealth);
        }
        else
        {
            Debug.LogError($"ManagerUI: HealthBar не найден для {healthSystem}");
        }
    }

    public void RegisterManaBar(IManaSystem manaSystem, ManaBar manaBar)
    {
        if (manaSystem == null || manaBar == null) return;

        if (!manaBars.ContainsKey(manaSystem))
        {
            manaBars[manaSystem] = manaBar;
        }
    }

    public void UnregisterManaBar(IManaSystem manaSystem)
    {
        if (manaBars.ContainsKey(manaSystem))
        {
            manaBars.Remove(manaSystem);
        }
    }

    public void UpdateCanvasMana(IManaSystem manaSystem, int currentHealth, int maxHealth)
    {
        if (manaBars.TryGetValue(manaSystem, out ManaBar targetHealthBar))
        {
            targetHealthBar.SetMana(currentHealth, maxHealth);
        }
        else
        {
            Debug.LogError($"ManagerUI: ManaBar не найден для {manaSystem}");
        }
    }
}
