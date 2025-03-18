using UnityEngine;


public class MagicInfluence: MagicBase
{
    [field: SerializeField] protected GameObject _target;


    public void SetTarget(GameObject target) { _target = target; }
}
