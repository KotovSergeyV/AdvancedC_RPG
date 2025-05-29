using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Unity.VisualScripting;

public class BossState_Follow : Abs_State_Boss
{
    float _followSpeed = 3f;
    Transform _target;
    NavMeshAgent _navMeshAgent;
    private IAnimatorController _animatorController;
    
    protected override IEnumerator StateFixedUpdate()
    {
        Debug.Log("BossState_Follow");
        while (InWork)
        {
            _navMeshAgent.SetDestination(_target.position);
            StateMachine.TryChangeState();
            yield return new WaitForFixedUpdate();
        }
    }

    public override void Enter(StateMachine_Boss stateMachine)
    {
        _animatorController ??= GetComponent<IAnimatorController>();
        _animatorController.PlayRunAnimation(true);
        
        _target = stateMachine.GetMachineOwner().GetTarget();
        if (_navMeshAgent == null) _navMeshAgent = GetComponent<NavMeshAgent>(); 
        
        _navMeshAgent.speed = _followSpeed;
        base.Enter(stateMachine);
    }

    public override void Exit()
    {
        _animatorController.PlayRunAnimation(false);
        _navMeshAgent.speed = 0;
        base.Exit();
    }
}
