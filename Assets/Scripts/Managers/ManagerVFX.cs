using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerVFX : BaseManager<ManagerVFX>
{
    public override void Initialize()
    {
        Debug.Log("VFXManager initialized.");
    }

    /// <summary>
    /// Какой эффект и где создаётся
    /// </summary>
    public void PlayVFX(GameObject effectPrefab, Vector3 position)
    {
        if (effectPrefab == null) return;

        GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
            Destroy(effect, ps.main.duration + 0.1f);
        }
        else
        {
            Destroy(effect, 2f);
        }
    }
}
