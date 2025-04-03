using System;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MagicBase
{
    public float CastTime { get; private set; }
    public int ManaCost { get; private set; }
    public Action<GameObject, GameObject> OnActivateMagic { get; set; }
    public GameObject Partical { get; set; }
    public AudioClip Clip { get; set; }

    private AsyncOperationHandle<AudioClip> _clipHandle;

    public MagicBase(float castTime, int manaCost, string audioEffectAddress)
    {
        CastTime = castTime;
        ManaCost = manaCost;
        LoadAudioClip(audioEffectAddress);
    }

    public virtual void ActivateMagic(GameObject caster, GameObject target)
    {
        OnActivateMagic?.Invoke(caster, target);
    }

    async void LoadAudioClip(string audioAddress)
    {
        _clipHandle = Addressables.LoadAssetAsync<AudioClip>(audioAddress);
        await _clipHandle.Task;

        if (_clipHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Clip = _clipHandle.Result;
        }
        else
        {
            Debug.Log("Error assigningSound!");
        }
    }

}