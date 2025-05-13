using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EntityMind_Boss : EntityMind
{
    // Movement Params
    float _runSpeed = 2.5f;
    float _walkSpeed = 2.5f;
    bool _destinationReached = false;
    float _seekDistMutiplier = 10f;
    float _minSeekDistPercent = .5f;

    // Inner Utils
    //bool _isMovementBlocked = false;

    // Outer Components
    IAnimatorController _animatorController;
    NavMeshAgent _navMeshAgent;

    // MEMTimers (in seconds)
    float MEMTimer_SeekTargetForget = 60f;

    // Keys
    GameObject Key_target;
    Vector3 Key_lastTargetPosition;

    float Key_distanceOfAttack = 3f;


    // Mind Control Triggers
    public event Action MindControlTrigger_SightTriggered;
    public event Action MindControlTrigger_SightExitTriggered;
    public event Action MindControlTrigger_DamagedTriggered;
    public event Action MindControlTrigger_AttackTriggered;



    private void Start()
    {
        _animatorController = gameObject.GetComponent<IAnimatorController>();
        _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

        Initialize(gameObject.GetComponent<BehaviorSense_Sight>(), gameObject.GetComponent<BehaviorSense_Damaged>());
    }

    public void Initialize(BehaviorSense_Sight sight, BehaviorSense_Damaged damage)
    {

        // Sense assigning
        sight.ViewFieldEntered += SightHandle;
        sight.ViewFieldExited += SightExitHandle;
        damage.DamagedTriggered += DamagedHandle;


        // Tasking
        #region Task creation

        BehaviorTask Task_followTarget = new BehaviorTask();
        Task_followTarget.Initialize(Priority.Advanced,
            // Logic
            delegate { StartCoroutine(Follow(_runSpeed)); },
            // Animation
            delegate { _animatorController.PlayWalkAnimation(false); },
            delegate { _animatorController.PlayRunAnimation(true); }
        );

        BehaviorTask Task_startForgetTarget = new BehaviorTask();
        Task_startForgetTarget.Initialize(Priority.Basic,
            // Logic
            delegate { StartCoroutine(Seek(_walkSpeed)); },
            // Memo
            delegate { _memory.WriteMemory(
                                delegate { 
                                    Key_lastTargetPosition = Vector3.zero;
                                    _animatorController.PlayWalkAnimation(false); 
                                },  MEMTimer_SeekTargetForget); }
        );

        BehaviorTask Task_attackTarget = new BehaviorTask();
        Task_attackTarget.Initialize(Priority.High,
            // Logic
            Attack
        );

        #endregion

        // Controller initialize
        _controller.Initialize(new List<Instruction>()
        {
            new Instruction((callback) => MindControlTrigger_SightTriggered += callback, Task_followTarget),
            new Instruction((callback) => MindControlTrigger_DamagedTriggered += callback, Task_followTarget),
            new Instruction((callback) => MindControlTrigger_SightExitTriggered += callback, Task_startForgetTarget),
            new Instruction((callback) => MindControlTrigger_AttackTriggered += callback, Task_attackTarget),
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


    //---- Note for future me: probably better to move "attacking" and "movement" to another scripts 
    #region Attacking

    public void Attack()
    {
        //StartCoroutine(BlockMovement());
        StopMovement();
        _animatorController.PlayAttackAnimationByTrigger();
    }

    #endregion


    #region Movement
    public IEnumerator Follow(float speed=2.5f)
    {
        _animatorController.PlayRunAnimation(true);
        _animatorController.PlayWalkAnimation(false);
        _navMeshAgent.speed = speed;

        while (Key_target)
        {
                
                if (Key_target.transform != null)
                {
                    transform.LookAt(Key_target.transform.position);
                }

                if (_navMeshAgent == null || Key_target.transform == null) Key_target = null;
                else
                {
                    _navMeshAgent.SetDestination(Key_target.transform.position);

                    if (Vector3.Distance(Key_target.transform.position, transform.position) < Key_distanceOfAttack)
                    { 
                    MindControlTrigger_AttackTriggered.Invoke();
                    Debug.Log("Attack"); 
                     
                    }
                }
            
            yield return new WaitForEndOfFrame();
        }
    }


    public IEnumerator Seek(float speed=2.5f)
    {
        _animatorController.PlayRunAnimation(false);
        _animatorController.PlayWalkAnimation(true);

        _navMeshAgent.speed = speed;

        while (Key_lastTargetPosition != Vector3.zero)
        {
            if (_navMeshAgent == null || Key_lastTargetPosition == null) { 
                Key_lastTargetPosition = Vector3.zero;
            }
            else
            {
                if (_destinationReached)
                {
                    Vector3 randomDirection;
                    randomDirection = UnityEngine.Random.insideUnitSphere;
                    while (randomDirection.sqrMagnitude  < _minSeekDistPercent * _minSeekDistPercent) randomDirection = UnityEngine.Random.insideUnitCircle;
                    randomDirection *= _seekDistMutiplier;
                    randomDirection += Key_lastTargetPosition;

                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(randomDirection, out hit, _seekDistMutiplier, NavMesh.AllAreas))
                    {
                        if (_navMeshAgent.SetDestination(hit.position)) _destinationReached = false;
                        Debug.DrawLine(hit.position, hit.position+Vector3.up*100f, new Color(255, 255, 0), 15f);

                    }
                }
                else
                {
                    if (Vector3.Distance(transform.position, _navMeshAgent.destination) < 1f) _destinationReached = true;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
    #endregion

    #region Utils

    //private IEnumerator BlockMovement()
    //{
    //    _isMovementBlocked = true;
    //    yield return new WaitForSeconds(_animatorController.GetLengthOfClip());
    //    _isMovementBlocked = false;

    //}

    private void StopMovement()
    {
        _navMeshAgent.speed = 0f;

        _animatorController.PlayRunAnimation(false);
        _animatorController.PlayWalkAnimation(false);
    }
    #endregion

}
