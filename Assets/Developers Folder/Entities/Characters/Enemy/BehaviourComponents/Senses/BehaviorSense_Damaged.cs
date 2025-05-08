using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorSense_Damaged : BehaviorSense
{

    [SerializeField] List<GameObject> _targetList;

    public Action<GameObject> DamagedTriggered;

    public void Trigger(GameObject damageInstigator)
    {
        DamagedTriggered.Invoke(damageInstigator);
    }
}
