using System.Collections.Generic;
using UnityEngine;

public class ManagerUI : BaseManager<ManagerUI>
{
    private GameObject playerCanvas;
    private List<GameObject> enemyCanvases = new List<GameObject>();

    private new void Awake()
    {
        playerCanvas = GameObject.Find("PlayerCanvas");
    }

    public override void Initialize()
    {
        playerCanvas = GameObject.Find("PlayerCanvas");

        if (playerCanvas == null)
        {
            Debug.LogError("PlayerCanvas не найден! Проверь, загружен ли он в сцене.");
            return;
        }

        Debug.Log("UIManager Initialized: PlayerCanvas найден.");
    }

    public void RegisterEnemyCanvas(GameObject enemyCanvas)
    {
        if (!enemyCanvases.Contains(enemyCanvas))
        {
            enemyCanvases.Add(enemyCanvas);
        }
    }

    public void UnregisterEnemyCanvas(GameObject enemyCanvas)
    {
        if (enemyCanvases.Contains(enemyCanvas))
        {
            enemyCanvases.Remove(enemyCanvas);
        }
    }

    public void ToggleEnemyUI(bool state)
    {
        foreach (var canvas in enemyCanvases)
        {
            if (canvas != null)
            {
                canvas.SetActive(state);
            }
        }
    }

    public void UpdateCanvasHp(int currentHealth, int maxHealth)
    {
        HealthBar targetHealthBar = playerCanvas?.GetComponentInChildren<HealthBar>();

        if (targetHealthBar == null)
        {
            Debug.LogError("ManagerUI: HealthBar не найден!");
            return;
        }

        targetHealthBar.SetHealth(currentHealth, maxHealth);
    }

    public void UpdateCanvasMana(int currentMana, int maxMana)
    {
        ManaBar targetHealthBar = playerCanvas?.GetComponentInChildren<ManaBar>();
        if (targetHealthBar != null)
        {
            targetHealthBar.SetMana(currentMana, maxMana);
        }
    }
}
