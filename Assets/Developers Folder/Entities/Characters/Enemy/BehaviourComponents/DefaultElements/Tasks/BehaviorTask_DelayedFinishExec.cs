using System;
using UnityEngine;

public class BehaviorTask_DelayedFinishExec : BehaviorTask
{

    public void Initialize(string name,Priority priority, Action<Action> FinishingAction, params Action[] payloadMethods)
    {
        base.Initialize(name, priority, payloadMethods);


        FinishingAction(delegate {
            ExecutionFinished?.Invoke();
            Debug.Log("DelayedFinishExec - ExecutionFinished");
        });

           
    }

    public override void Execute()
    {
        StartPayload();
    }
}
