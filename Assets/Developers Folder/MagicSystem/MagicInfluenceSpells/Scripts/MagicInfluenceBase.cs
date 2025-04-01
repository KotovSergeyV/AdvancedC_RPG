using UnityEngine;

public class MagicInfluenceBase : MagicBase
{
    protected GameObject _target;

    public MagicInfluenceBase(float castTime, int manaCost)
        : base(castTime, manaCost)
    {
    }

    public void SetTarget(GameObject target)
    {
        _target = target;
    }

    public override void ActivateMagic(GameObject caster, GameObject target)
    {
        base.ActivateMagic(caster, target);
        if (_target != null)
        {
            Debug.Log("Magic influenced target: " + _target.name);
        }
    }
}