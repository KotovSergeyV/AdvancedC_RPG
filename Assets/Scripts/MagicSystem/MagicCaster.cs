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

    public void SetTarget(GameObject target)
    {
        _target = target;
    }

    public void InitiateCast()
    {
        if (_needMana)
        {
            I_Mana mana = GetComponent<I_Mana>();
            if (mana == null || mana.GetMana() < _magicToCast.ManaCost)
            {
                SuspendCast();
                return; // Exit if mana is insufficient
            }
            mana.RemoveMana(_magicToCast.ManaCost);
        }

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
        Debug.Log("Started cast: " + _magicToCast.CastTime);

        yield return new WaitForSeconds(_magicToCast.CastTime);

        // Activate the magic
        if (_magicToCast is MagicInfluence magicInfluence)
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