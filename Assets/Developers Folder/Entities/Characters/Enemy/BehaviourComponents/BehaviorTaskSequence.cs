using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTaskSequence : IBehaviorNode
{

    List<IBehaviorNode> _taskExecutionQueue;
    IBehaviorNode _currentTask;
    Action ExecutionFinished;


    public void Initialize(List<IBehaviorNode> taskExecutionQueue)
    {
        _taskExecutionQueue = taskExecutionQueue;
    }

    public void Execute()
    {
        Next();
    }

    public Priority GetPriority()
    {
        return _currentTask.GetPriority();
    }


    void Next()
    {
        if (_taskExecutionQueue.Count == 0) { ExecutionFinished?.Invoke(); return; }

        _currentTask = _taskExecutionQueue[0];
        _taskExecutionQueue.RemoveAt(0);

        _currentTask.Execute();
    }

    public void Cancel()
    {
        if (_currentTask is BehaviorTask_Async) {
            ((BehaviorTask_Async)_currentTask).InteruptPayload();
        } 
    }
}
