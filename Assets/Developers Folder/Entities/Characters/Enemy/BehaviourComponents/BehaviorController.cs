using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorController
{
    Dictionary<Action, IBehaviorNode> ControlData;

    IBehaviorNode _currentTask;
    IBehaviorNode _waitingTask;

    public void Initialize(Dictionary<Action, IBehaviorNode> ControlData)
    {
        
    }

    void ExecTaskByCondition(IBehaviorNode newTask) 
    {
        if (CheckPriority(newTask)) 
        { 
        }
    }

    bool CheckPriority(IBehaviorNode newTask) 
    {
        return newTask.GetPriority() < _currentTask.GetPriority();
    }
}
