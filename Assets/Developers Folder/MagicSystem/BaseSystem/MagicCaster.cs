using System;
using System.Collections;
using UnityEngine;

public class MagicCaster : MonoBehaviour
{
    [SerializeField] private MagicBase _magicToCast; // Assign this in code
    [SerializeField] private bool _needMana;
    [SerializeField] private GameObject _target;

    public Action OnCastStarted;
    public Action OnCastSuccessful;
    public Action OnCastSuspended;

    [SerializeField] AudioClip Clip;
    [SerializeField] GameObject _partical; 

    ManagerSFX _managerSFX;
    ManagerVFX _managerVFX;

    IManaSystem _manaSystem;

    public void Initialize(IManaSystem manaSystem, ManagerSFX managerSFX, ManagerVFX managerVFX)
    {
        _manaSystem = manaSystem;
        _managerSFX = managerSFX;
        _managerVFX = managerVFX;
    }

    public void SetTarget(GameObject target)
    {
        _target = target;
    }

    public void InitiateCast()
    {
        Debug.Log(_manaSystem + " " + _manaSystem.GetMana() + " " + _magicToCast + " " + _magicToCast.ManaCost);
        
        if (_manaSystem == null || _manaSystem.GetMana() < _magicToCast.ManaCost)
            {
                SuspendCast();

                return; // Exit if mana is insufficient
            }
        _manaSystem.RemoveMana(_magicToCast.ManaCost);
        

        StartCoroutine(CastRoutine());
    }

    public void SuspendCast()
    {
        StopAllCoroutines(); // Stop any running coroutines
        OnCastSuspended?.Invoke();
    }

    private IEnumerator CastRoutine()
    {
        OnCastStarted?.Invoke();

        yield return new WaitForSeconds(_magicToCast.CastTime);

        _managerSFX.PlaySFX(_magicToCast.Clip, transform.position, null, false, 1, 0);
       
        _managerVFX.PlayVFX(_partical, transform.position, -1.5f);

        // Activate the magic
        if (_magicToCast is MagicInfluenceBase magicInfluence)
        {
            magicInfluence.SetTarget(_target); // Set the target for influence-based magic
        }
        _magicToCast.ActivateMagic(gameObject, _target);

        OnCastSuccessful?.Invoke();
        Debug.Log("Magic casted successfully!");
    }

    public void SetMagic(MagicBase magic)
    {
        _magicToCast = magic;
    }
}