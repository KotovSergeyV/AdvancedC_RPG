using System;
using UnityEngine;

public class EntityStatesSystem : IEntityStatesSystem
{
    Enum_EntityStates _currentState;
    public event Action<Enum_EntityStates> OnStateChanged;  //for subscription

    public Enum_EntityStates GetEntityState() { return _currentState; }

    public void SetEntityState(Enum_EntityStates newState) { 
        _currentState = newState; 
        OnStateChanged?.Invoke(_currentState);
    }


}
