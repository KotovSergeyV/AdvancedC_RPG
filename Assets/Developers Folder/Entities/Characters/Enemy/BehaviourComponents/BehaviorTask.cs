using System;
using System.Threading.Tasks;
using UnityEngine;

public class BehaviorTask : IBehaviorNode
{
    Priority _priority;
    Action Payload;

    Action ExecutionFinished;

    public Action Initialize(Priority priority, params Action[] payloadMethods)
    {
        _priority = priority;

        foreach (var method in payloadMethods)
        {
            Payload += method; 
        }

        return ExecutionFinished;
    }

    public virtual void Execute()
    {
        StartPayload();
        ExecutionFinished?.Invoke();
    }

    void StartPayload() { Payload?.Invoke(); }

    public Priority GetPriority() { return _priority; }
}



public enum Priority
{
    Low ,
    Basic,
    Advanced,
    High,
    Absolute 
}
