using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ManagerSFX : BaseManager<ManagerSFX>
{
    [Header("Mixer Groups")]
    public AudioMixerGroup masterGroup;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup voiceGroup;
    public AudioMixerGroup musicGroup;

    private List<AudioSource> activeSFXSources = new List<AudioSource>();

    public override void Initialize()
    {
        Debug.Log("SFXManager initialized.");
    }

    public void PlaySFX(AudioClip clip, Vector3 position, MixerGroupType mixerGroupType, AudioSource template = null, bool is3D = true, float volume = 1f, float pitchVariation = 0.1f)
    {
        if (clip == null) return;

        AudioMixerGroup mixerGroup = GetMixerGroup(mixerGroupType);

        AudioSource newSource = (template != null)
            ? Instantiate(template, position, Quaternion.identity)
            : new GameObject("SFX_AudioSource").AddComponent<AudioSource>();

        newSource.clip = clip;
        newSource.volume = volume;
        newSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        newSource.spatialBlend = is3D ? 1f : 0f;

        newSource.outputAudioMixerGroup = mixerGroup;

        activeSFXSources.Add(newSource);
        newSource.Play();
        Destroy(newSource.gameObject, clip.length + 0.1f);
    }

    private AudioMixerGroup GetMixerGroup(MixerGroupType type)
    {
        return type switch
        {
            MixerGroupType.SFX => sfxGroup,
            MixerGroupType.Voice => voiceGroup,
            MixerGroupType.Music => musicGroup,
            _ => masterGroup
        };
    }

    public void StopAllSFX()
{
    foreach (var source in activeSFXSources)
    {
        if (source != null)
        {
            source.Stop();
            Destroy(source.gameObject);
        }
    }
    activeSFXSources.Clear();
}


    public enum MixerGroupType
    {
        Master,
        SFX,
        Voice,
        Music
    }
}