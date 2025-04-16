using System;
using UnityEngine;

public interface IEntityStatesSystem
{
    public Enum_EntityStates GetEntityState() { return Enum_EntityStates.Idle; }
    public void SetEntityState(Enum_EntityStates newState) { }
    public event Action<Enum_EntityStates> OnStateChanged;

}
