using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EntityMind_Boss : EntityMind
{

    GameObject Key_target;

    public event Action MindControlTrigger_SightTriggered;
    public event Action MindControlTrigger_DamagedTriggered;

    private void Start()
    {
        Initialize(gameObject.GetComponent<BehaviorSense_Sight>(), gameObject.GetComponent<BehaviorSense_Damaged>());
    }
    public void Initialize(BehaviorSense_Sight sight, BehaviorSense_Damaged damage)
    {

        sight.ViewFieldEntered += SightHandle;
        damage.DamagedTriggered += DamagedHandle;

        var x = new BehaviorTask();
        x.Initialize(Priority.High, delegate { 
            GoToTarget(Key_target.transform, 5f); 
        });

        _controller.Initialize(new List<Instruction>()
        {
            new Instruction((callback) => MindControlTrigger_SightTriggered += callback, x),
            new Instruction((callback) => MindControlTrigger_DamagedTriggered += callback, x),
        });
    }

    public void GoToTarget(Transform target, float speed)
    {
        NavMeshAgent _agent = gameObject.GetComponent<NavMeshAgent>();
        if (target != null)
        {
            transform.LookAt(target.position);
        }

        if (_agent == null || target == null) return;

        _agent.speed = speed;
        _agent.SetDestination(target.position);
    }

    private void SightHandle(GameObject target)
    {
        Debug.Log("See!");

        Key_target = target;
        MindControlTrigger_SightTriggered.Invoke();
    }

    private void DamagedHandle(GameObject target)
    {
        Key_target = target;
        MindControlTrigger_DamagedTriggered.Invoke();
    }
}
