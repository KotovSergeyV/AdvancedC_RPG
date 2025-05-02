using System;
using System.Threading.Tasks;
using UnityEngine;

public class BehaviorTask : IBehaviorNode
{

    ///  <summary>Task priority</summary>
    protected Priority _priority;
    ///  <summary>Methods embeded in task</summary>
    protected Action Payload;         

    public Action ExecutionFinished { get; set; }


    ///  <summary>Initialize task</summary>
    public void Initialize(Priority priority, params Action[] payloadMethods)
    {
        _priority = priority;

        foreach (var method in payloadMethods)
        {
            Payload += method.Invoke; 
        }

    }

    public virtual void Execute()
    {
        StartPayload();
        ExecutionFinished?.Invoke();
    }

    /// <summary>Invoke payload methods</summary>
    protected virtual void StartPayload() { Payload?.Invoke(); }

    public Priority GetPriority() { return _priority; }

}



public enum Priority
{
    Minus,  //system default for overwriting, do not assign to ingame actions.
    Low,
    Basic,
    Advanced,
    High,
    Absolute 
}
