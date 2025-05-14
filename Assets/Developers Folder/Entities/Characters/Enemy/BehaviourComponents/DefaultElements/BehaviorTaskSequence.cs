using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BehaviorTaskSequence : IBehaviorNode
{

    List<IBehaviorNode> _taskExecutionQueue;
    List<IBehaviorNode> _taskExecutionQueueCopy = new List<IBehaviorNode>();
    IBehaviorNode _currentTask;
    public Action ExecutionFinished { get; set; }

    /// <summary>
    /// Initialize tasks sequence
    /// </summary>
    public void Initialize(List<IBehaviorNode> taskExecutionQueue)
    {
        _taskExecutionQueue = taskExecutionQueue;
    }


    public void Execute()
    {
        _taskExecutionQueueCopy.AddRange(_taskExecutionQueue);

        Next();
    }

    public Priority GetPriority()
    {
        if (_currentTask == null) { return _taskExecutionQueue[0].GetPriority(); }
        return _currentTask.GetPriority();
    }

    /// <summary>
    /// Start next method in sequence
    /// </summary>
    async void Next()
    {
        if (_taskExecutionQueueCopy.Count == 0) { ExecutionFinished?.Invoke(); return; }

        _currentTask = _taskExecutionQueueCopy[0];
        _taskExecutionQueueCopy.RemoveAt(0);

        if (_currentTask is BehaviorTask_Delayed) { await Task.Delay((int)((BehaviorTask_Delayed)_currentTask).TaskDelay * 1000); }

        _currentTask.ExecutionFinished += Next;
        _currentTask.Execute();
    }

    /// <summary>
    /// Interupt current sequence
    /// </summary>
    public void InteruptPayload()
    {
        //if (_currentTask is BehaviorTask_Async) {
       //     ((BehaviorTask_Async)_currentTask).InteruptPayload();
       // } 
    }
}
