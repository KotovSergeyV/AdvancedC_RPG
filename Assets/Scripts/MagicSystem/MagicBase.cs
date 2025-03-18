using System;
using UnityEngine;

public class MagicBase
{
    public float CastTime { get; private set; }
    public int ManaCost { get; private set; }
    public Action<GameObject, GameObject> OnActivateMagic { get; set; }

    public MagicBase(float castTime, int manaCost)
    {
        CastTime = castTime;
        ManaCost = manaCost;
    }

    public virtual void ActivateMagic(GameObject caster, GameObject target)
    {
        OnActivateMagic?.Invoke(caster, target);
    }
}