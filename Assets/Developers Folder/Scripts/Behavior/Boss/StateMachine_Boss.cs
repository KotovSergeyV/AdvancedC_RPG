using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMachine_Boss
{
    Abs_State_Boss _currentAbsState;
    BossController _boss;
    
    
    Dictionary<Abs_State_Boss, List<(Func<Abs_State_Boss, bool> condition,
        Func<Abs_State_Boss, Abs_State_Boss> transition)>> _stateTransitionTable;

    public StateMachine_Boss(BossController bossController,  Dictionary<Abs_State_Boss, List<(Func<Abs_State_Boss, bool> condition,
        Func<Abs_State_Boss, Abs_State_Boss> transition)>> stateTransitionTable) 
    {
        _boss = bossController; 
        _stateTransitionTable = stateTransitionTable;
        _currentAbsState = _stateTransitionTable.Keys.First();
        _currentAbsState.Enter(this);
    }

    public void TryChangeState()
    {
        foreach (var (condition, transition) in _stateTransitionTable[_currentAbsState])
        {
            if (condition(_currentAbsState))
            {
                _currentAbsState.Exit();
                _currentAbsState = transition(_currentAbsState);
                _currentAbsState.Enter(this);
                break;
            }
        }
    }

    public BossController GetMachineOwner()
    {
        return _boss;
    }
    public Abs_State_Boss GetCurrentState()
    {
        return _currentAbsState;
    }

    public void Delete()
    {
        _currentAbsState.Exit();
    }
}
