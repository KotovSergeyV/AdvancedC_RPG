using System.Collections.Generic;
using UnityEngine;

public class ManagerSFX : BaseManager<ManagerSFX>
{
    public override void Initialize()
    {
        Debug.Log("SFXManager initialized.");
    }

    ///<summary>
    ///Какой звук и где создаётся, а также есть ли audiosource, 2D или 3D, громкость и питч звука
    ///</summary>
    public void PlaySFX(AudioClip clip, Vector3 position, AudioSource template = null, bool is3D = true, float volume = 1f, float pitchVariation = 0.1f)
    {
        if (clip == null) return;

        AudioSource newSource = (template != null)
            ? Instantiate(template, position, Quaternion.identity)
            : new GameObject("SFX_AudioSource").AddComponent<AudioSource>();

        newSource.clip = clip;
        newSource.volume = volume;
        newSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        newSource.spatialBlend = is3D ? 1f : 0f;
        newSource.Play();

        Destroy(newSource.gameObject, clip.length + 0.1f);
    }
}