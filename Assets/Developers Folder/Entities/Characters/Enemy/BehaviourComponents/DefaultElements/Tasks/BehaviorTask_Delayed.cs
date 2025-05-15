using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class BehaviorTask_Delayed : BehaviorTask
{
    public float TaskDelay { get; private set; }
    public void Initialize(string name, Priority priority, float taskDelay, params Action[] payloadMethods)
    {
        base.Initialize(name, priority, payloadMethods);
        TaskDelay = taskDelay;
    }

}
