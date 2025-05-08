using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EntityMind_Boss : EntityMind
{

    GameObject Key_target;
    Vector3 Key_lastTargetPosition;

    public event Action MindControlTrigger_SightTriggered;
    public event Action MindControlTrigger_SightExitTriggered;

    public event Action MindControlTrigger_DamagedTriggered;



    private void Start()
    {
        Initialize(gameObject.GetComponent<BehaviorSense_Sight>(), gameObject.GetComponent<BehaviorSense_Damaged>());
    }
    public void Initialize(BehaviorSense_Sight sight, BehaviorSense_Damaged damage)
    {

        // Sense assigning
        sight.ViewFieldEntered += SightHandle;
        sight.ViewFieldExited += SightExitHandle;
        damage.DamagedTriggered += DamagedHandle;


        // Tasking
        var Task_followTarget = new BehaviorTask();
        Task_followTarget.Initialize(Priority.High, 
            delegate { StartCoroutine(Follow()); }
            );

        var Task_startForgetTarget = new BehaviorTask();
        Task_startForgetTarget.Initialize(Priority.Basic,
            delegate { StartCoroutine(Seek()); },
            delegate { _memory.WriteMemory(delegate { Key_lastTargetPosition = Vector3.zero; }, 20f); }      
        );

        // Controller initialize
        _controller.Initialize(new List<Instruction>()
        {
            new Instruction((callback) => MindControlTrigger_SightTriggered += callback, Task_followTarget),
            new Instruction((callback) => MindControlTrigger_DamagedTriggered += callback, Task_followTarget),

            new Instruction((callback) => MindControlTrigger_SightExitTriggered += callback, Task_startForgetTarget),
        });
    }


    #region SenseHandling
    private void SightHandle(GameObject target)
    {
        Key_target = target;
        MindControlTrigger_SightTriggered.Invoke();
    }
    private void SightExitHandle()
    {
        if (Key_target == null) return;
        Key_lastTargetPosition = Key_target.transform.position;
        Key_target = null;
        MindControlTrigger_SightExitTriggered.Invoke();
    }

    private void DamagedHandle(GameObject target)
    {
        Key_target = target;
        MindControlTrigger_DamagedTriggered.Invoke();
    }
    #endregion


    #region Move
    public IEnumerator Follow(float speed=2.5f)
    {
        while (Key_target)
        {
            NavMeshAgent _agent = gameObject.GetComponent<NavMeshAgent>();
            if (Key_target.transform != null)
            {
                transform.LookAt(Key_target.transform.position);
            }

            if (_agent == null || Key_target.transform == null) Key_target = null;
            else
            {
                _agent.speed = speed;
                _agent.SetDestination(Key_target.transform.position);
                yield return new WaitForEndOfFrame();
            }
        }
    }
    public IEnumerator Seek(float speed=2.5f)
    {
        while (Key_lastTargetPosition != Vector3.zero)
        {
            NavMeshAgent _agent = gameObject.GetComponent<NavMeshAgent>();

            if (_agent == null || Key_lastTargetPosition == null) { 
                Key_lastTargetPosition = Vector3.zero;
                yield return new WaitForEndOfFrame();
            }
            else
            {
                _agent.speed = speed;

                Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 2f;
                randomDirection += Key_lastTargetPosition;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, 2f, NavMesh.AllAreas))
                {
                    _agent.SetDestination(hit.position);
                    yield return new WaitForSeconds(1f);
                }
            }
        }
    }
    #endregion


}
