using System;
using System.Collections;
using UnityEngine;


public class MagicCaster : MonoBehaviour
{
    [SerializeField] MagicBase _magicToCast;

    [SerializeField] bool _needMana;
    [SerializeField] GameObject _target;

    public Action OnCastStarted;
    public Action OnCastSuccessful;
    public Action OnCastSuspended;

    private void Start()
    {
        InitiateCast();
    }

    public void InitiateCast()
    {
        if (_needMana)
        {
            if (GetComponent<I_Mana>()?.GetMana() > _magicToCast.manaCost)
            {
                GetComponent<I_Mana>()?.RemoveMana(_magicToCast.manaCost);
            }
            else { SuspendCast(); }
        }
        StartCoroutine(CastRoutine(_magicToCast.CastTime));
    }

    public void SuspendCast()
    {
        StopCoroutine(CastRoutine(_magicToCast.CastTime));
        OnCastSuspended?.Invoke();
    }

    private IEnumerator CastRoutine(float castTime)
    {
        OnCastStarted?.Invoke();
        Debug.Log("Started cast: " + castTime);
        yield return new WaitForSeconds(castTime);
        OnCastSuccessful?.Invoke();

        Debug.Log("casted");
        if (_magicToCast is MagicInfluence magicInfluence && _target)
        {
            magicInfluence.SetTarget(_target);
            Debug.Log("Target locked");
        }


        _magicToCast.OnActivateMagic?.Invoke();
    }
}