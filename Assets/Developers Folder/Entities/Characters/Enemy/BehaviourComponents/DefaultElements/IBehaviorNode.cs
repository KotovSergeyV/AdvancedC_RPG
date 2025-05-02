using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public interface IBehaviorNode
{
    /// <summary>On Task Finished Event to controller/sequence</summary>
    public Action ExecutionFinished { get; set; }

    ///  <summary>Start sequence/task</summary>
    public void Execute();

    /// <summary>Return task prioity</summary>
    /// <returns>Prioity of task</returns>
    public Priority GetPriority();  
}
