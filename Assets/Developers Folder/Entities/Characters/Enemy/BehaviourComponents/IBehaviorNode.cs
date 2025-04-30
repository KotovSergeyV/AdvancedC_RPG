using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public interface IBehaviorNode
{
    public void Execute();

    public Priority GetPriority();
}
