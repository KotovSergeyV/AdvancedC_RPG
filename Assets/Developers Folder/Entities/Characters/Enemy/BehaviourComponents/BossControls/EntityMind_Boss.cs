using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class EntityMind_Boss : EntityMind
{

    // Outer Components
    IAnimatorController _animatorController;
    NavMeshAgent _navMeshAgent;

    // MEMTimers (in seconds)
    float MEMTimer_SeekTargetForget = 60f;

    // Flags
    bool _seekFlag;
    bool _followFlag;


    // Keys
    GameObject Key_target;
    Vector3 Key_targetLastPosition;

    float Key_distanceOfAttack = 3f;
    float Key_seekAcceptanceRange = 2.5f;


    // Mind Control Triggers
    public event Action MindControlTrigger_Sight;
    public event Action MindControlTrigger_SightExit;

    public event Action MindControlTrigger_NewSeekDestination;

    public event Action MindControlTrigger_Damaged;

    public event Action MindControlTrigger_AttackTriggered;



    private void Start()
    {
        _animatorController = gameObject.GetComponent<IAnimatorController>();
        _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

        Initialize(gameObject.GetComponent<BehaviorSense_Sight>(), gameObject.GetComponent<BehaviorSense_Damaged>());
    }

    public void Initialize(BehaviorSense_Sight sight, BehaviorSense_Damaged damage)
    {
        ACS_Movement acs_movement = gameObject.AddComponent<ACS_Movement>();
        acs_movement.Initialize(2.5f, 2.5f, 10, .5f, _navMeshAgent);


        // Sense assigning
        sight.ViewFieldEntered += SightHandle;
        sight.ViewFieldExited += SightExitHandle;
        damage.DamagedTriggered += DamagedHandle;


        // Tasking
        #region Task creation

        BehaviorTask Task_followTarget = new BehaviorTask();
        Task_followTarget.Initialize(Priority.Advanced,
            // Logic
            delegate {
                Debug.Log("Task_followTarget");
                SetFlag(0);
                acs_movement.Follow(Key_target);
                StartCoroutine(FollowRoutine());
            },
            // Animation
            delegate { _animatorController.PlayWalkAnimation(false);
                       _animatorController.PlayRunAnimation(true); }
        );


        BehaviorTask Task_seekAroundLastPosition = new BehaviorTask();
        Task_seekAroundLastPosition.Initialize(Priority.Basic,
            // Logic
            delegate {
                Debug.Log("Task_seekAroundLastPosition");
                       acs_movement.StopFollow();

                       acs_movement.Seek(Key_targetLastPosition);
                       SetFlag(1);
                       StartCoroutine(SeekRoutine());
            },
            // Animation
            delegate {
                _animatorController.PlayRunAnimation(false);
                _animatorController.PlayWalkAnimation(true);
            }
        );

        BehaviorTask Task_startForgetTarget = new BehaviorTask();
        Task_startForgetTarget.Initialize(Priority.Basic,
            // Memo
            delegate { _memory.WriteMemory(
                                delegate {
                                    _seekFlag = false;
                                    _navMeshAgent.speed = 0;
                                    _animatorController.PlayWalkAnimation(false); 
                                },  
                                MEMTimer_SeekTargetForget); }
        );

        BehaviorTaskSequence TaskSeq_SeekAndForget = new BehaviorTaskSequence();
        TaskSeq_SeekAndForget.Initialize(new List<IBehaviorNode>() { Task_seekAroundLastPosition, Task_startForgetTarget });

        BehaviorTask Task_attackTarget = new BehaviorTask();
        Task_attackTarget.Initialize(Priority.High,
            // Logic
            Attack
        );

        #endregion

        // Controller initialize
        _controller.Initialize(new List<Instruction>()
        {
            new Instruction((callback) => MindControlTrigger_Sight += callback, Task_followTarget),
            new Instruction((callback) => MindControlTrigger_Damaged += callback, Task_followTarget),

            new Instruction((callback) => MindControlTrigger_SightExit += callback, TaskSeq_SeekAndForget),
            new Instruction((callback) => MindControlTrigger_NewSeekDestination += callback, Task_seekAroundLastPosition),

            new Instruction((callback) => MindControlTrigger_AttackTriggered += callback, Task_attackTarget),
        });
    }



    #region SenseHandling
    private void SightHandle(GameObject target)
    {
        Key_target = target;
        MindControlTrigger_Sight.Invoke();
    }
    private void SightExitHandle()
    {
        Key_targetLastPosition = Key_target.transform.position;
        Key_target = null;
        Debug.Log("MindControlTrigger_SightExit.Invoke");
        MindControlTrigger_SightExit.Invoke();
    }

    private void DamagedHandle(GameObject target)
    {
        Key_target = target;
        MindControlTrigger_Damaged.Invoke();
    }

    #endregion

    #region Continuous actions routine

    IEnumerator OnAttackRangeReached() 
    {
        yield return null; 
    }

    IEnumerator FollowRoutine()
    {
        yield return null;
        //while (_followFlag)
        //{
        //    yield return null;
        //}
    }

    IEnumerator SeekRoutine() 
    {
        while (_seekFlag)
        {
            if (Vector3.Distance(transform.position, _navMeshAgent.destination) < Key_seekAcceptanceRange)
            { 
                MindControlTrigger_NewSeekDestination.Invoke();
                Debug.DrawRay(_navMeshAgent.destination, Vector3.up*100, Color.yellow, 5);
                Debug.Log("MindControlTrigger_NewSeekDestination.Invoke");
            }
            yield return null;
        }
    }
    #endregion

    //private void TargetDistanceAnalysys()
    //{

    //}

    #region Attacking

    public void Attack()
    {
        //StartCoroutine(BlockMovement());
        StopMovement();
        _animatorController.PlayAttackAnimationByTrigger();
    }

    #endregion



    #region Utils

    private void StopMovement()
    {
        _navMeshAgent.speed = 0f;

        _animatorController.PlayRunAnimation(false);
        _animatorController.PlayWalkAnimation(false);
    }

    private void SetFlag(int flagIdx)
    {
        List<string> flags = new List<string>() { "_followFlag", "_seekFlag" };
        _followFlag = flagIdx == flags.IndexOf("_followFlag");
        _seekFlag = flagIdx == flags.IndexOf("_seekFlag");
    }
    #endregion

}
