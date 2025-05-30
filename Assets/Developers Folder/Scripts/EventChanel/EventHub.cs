using System;
using UnityEngine;

public static class EventHub
{
    #region EnemyDeathChannel

    private static int EnemyDeathCalls;
    public static event Action<int> Broadcast_EnemyCounterUpdated = delegate { };
    public static void AddDeathCall()
    {
        EnemyDeathCalls++;
        Broadcast_EnemyCounterUpdated.Invoke(EnemyDeathCalls);
    }

    public static void ResetDeathCounter()
    {
        EnemyDeathCalls = 0;
        Broadcast_EnemyCounterUpdated = delegate { };
    }
    #endregion

}
