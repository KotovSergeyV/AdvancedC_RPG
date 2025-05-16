using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityMind : MonoBehaviour
{
    protected BehaviorController _controller = new BehaviorController();
    protected EntityMemory _memory = new EntityMemory();
}
