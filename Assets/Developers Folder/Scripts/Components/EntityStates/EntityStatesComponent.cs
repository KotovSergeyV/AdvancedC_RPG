using System;
using UnityEngine;

public class EntityStatesComponent : MonoBehaviour, I_EntityStates
{
    Enum_EntityStates _currentState;
    public event Action<Enum_EntityStates> OnStateChanged;  //for subscription

    public Enum_EntityStates GetEntityState() { return _currentState; }

    public void SetEntityState(Enum_EntityStates newState) { 
        _currentState = newState; 
        OnStateChanged?.Invoke(_currentState);
    }


}
