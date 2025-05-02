using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BehaviorController
{

    //List<Instruction> ControlData = new List<Instruction>();

    IBehaviorNode _currentTask;
    BehaviorTask _defaultTask;

    IBehaviorNode _waitingTask;
    float _waitingTaskRecordedTime;
    float _maxWaitingTaskTime;

    void CreateDefault()
    {
        _defaultTask = new BehaviorTask();
        _defaultTask.Initialize(Priority.Minus, delegate {  });
    }

    public void Initialize(List<Instruction> controlData)
    {
        CreateDefault();
        _currentTask = _defaultTask;

        //ControlData = controlData;
        foreach (Instruction instruction in controlData)
        {
           // Subscription to execTask, becxause Action<Action> to link to original event.
           instruction.Trigger(() => ExecTaskCommand(instruction.Node));
        }
    }

    void ExecTaskCommand(IBehaviorNode newTask) 
    {
        if (newTask == null) { newTask = _defaultTask; }
        // NEVER EVER ASSIGN _defaultTask VIA CaptureNewCurrentTask(newTask) !!!!! MEMO OVERFLOW

        if (CheckPriority(newTask, _currentTask)) CaptureNewCurrentTask(newTask);
        else if (CheckPriority(newTask, _waitingTask) || Time.time - _waitingTaskRecordedTime >= _maxWaitingTaskTime) SetTaskAsWaiting(newTask);
    }

    void SetTaskAsWaiting(IBehaviorNode newTask)
    {
        _waitingTask = newTask;
        _waitingTaskRecordedTime = Time.time;
    }

    void CaptureNewCurrentTask(IBehaviorNode newTask)
    {
        if (newTask == null) { newTask = _defaultTask; }

        if (_currentTask is BehaviorTaskSequence) { ((BehaviorTaskSequence)_currentTask).InteruptPayload(); }
        //else if (_currentTask is BehaviorTask_Async) { ((BehaviorTask_Async)_currentTask).InteruptPayload(); }
        
        _currentTask = newTask;
        _currentTask.ExecutionFinished += ClearTask;
        _currentTask.ExecutionFinished += delegate { ExecTaskCommand(_waitingTask); };
        _currentTask.Execute();
    }
    void ClearTask() { _currentTask = _defaultTask; }
    bool CheckPriority(IBehaviorNode newTask, IBehaviorNode prevTask) 
    {
        return newTask.GetPriority() > _currentTask.GetPriority();
    }
}


public class Instruction 
{
    public Action<Action> Trigger;
    public IBehaviorNode Node;

    public Instruction(Action<Action> triggerMethod, IBehaviorNode node)
    {
        Trigger = triggerMethod;
        Node = node;
    }
}