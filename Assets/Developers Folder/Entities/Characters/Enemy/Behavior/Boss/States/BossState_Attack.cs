using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Unity.VisualScripting;


public class BossState_Attack : Abs_State_Boss
{
    private float _attackCooldown = 10f;
    private float _lastAttackTime;
    
    private float _animationLength = 3f;
    private IAnimatorController _animatorController;
    
    private Transform _target;
    
    protected override IEnumerator StateFixedUpdate()
    { 
        Debug.Log("BossState_Attack");
        while (InWork)
        {
            transform.LookAt(_target);
            if (Time.time - _lastAttackTime >= _attackCooldown)
            {
                _lastAttackTime =  Time.time;
                _animatorController.PlayAttackAnimationByTrigger();
                yield return new WaitForEndOfFrame();
            }
            else
            {
                if (Time.time - _lastAttackTime >= _animationLength)
                {
                    StateMachine.TryChangeState();
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public override void Enter(StateMachine_Boss stateMachine)
    {
        _animatorController ??= GetComponent<IAnimatorController>();
        
        _target = stateMachine.GetMachineOwner().GetTarget();
        
        base.Enter(stateMachine);
    }
}
