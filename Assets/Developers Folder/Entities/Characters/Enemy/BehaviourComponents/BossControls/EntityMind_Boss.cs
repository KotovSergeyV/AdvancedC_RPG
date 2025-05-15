using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class EntityMind_Boss : EntityMind
{

    int LOGGER_PARAM___FOLLOW_ROUTINE_COUNT = 0;
    int LOGGER_PARAM___SEEK_ROUTINE_COUNT = 0;
    
    // Outer Components
    IAnimatorController _animatorController;
    NavMeshAgent _navMeshAgent;
    
    // Outer function links
    
    public  Action OuterTrigger_RestartSightFieldFlag;

    // MEMTimers (in seconds)
    float MEMTimer_SeekTargetForget = 60f;

    // Flags
    bool _seekFlag;
    bool _followFlag;

    // Timers
    float _lastStrikeTime = 0;

    // Keys
    GameObject Key_target;
    Vector3 Key_targetLastPosition;

    float Key_distanceOfAttack = 3f;
    float Key_seekAcceptanceRange = 2f;
    float Key_AttackCooldown = 5f;


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
        // Action Components creation
        ACS_Movement acs_movement = gameObject.AddComponent<ACS_Movement>();
        acs_movement.Initialize(2f, 5f, 10, .5f, _navMeshAgent);
        ACS_Attacking acs_attacking = gameObject.AddComponent<ACS_Attacking>();



        // Sense assigning
        sight.ViewFieldEntered += SightHandle;
        sight.ViewFieldExited += SightExitHandle;
        damage.DamagedTriggered += DamagedHandle;
        
        OuterTrigger_RestartSightFieldFlag += sight.RefreshSence;
        acs_attacking.StrikeFinished += delegate { OuterTrigger_RestartSightFieldFlag.Invoke(); };


        // Tasking
        #region Task creation

        BehaviorTask Task_followTarget = new BehaviorTask();
        Task_followTarget.Initialize("Task_followTarget", Priority.Advanced,
            // Logic
            delegate {
                Debug.Log("Task_followTarget");
                _memory.WriteMemory(delegate { }, 0f);
                acs_movement.Follow(Key_target);
                if (!_followFlag)
                {
                    SetFlag(0);
                    StartCoroutine(FollowRoutine());
                }
            },
            // Animation
            delegate { _animatorController.PlayWalkAnimation(false);
                       _animatorController.PlayRunAnimation(true); }
        );


        BehaviorTask Task_seekAroundLastPosition = new BehaviorTask();
        Task_seekAroundLastPosition.Initialize("Task_seekAroundLastPosition",Priority.Basic,
            // Logic
            delegate {
                Debug.Log("Task_seekAroundLastPosition");
                       acs_movement.StopFollow();
                       acs_movement.Seek(Key_targetLastPosition);
                       if (!_seekFlag) {
                            SetFlag(1);
                            StartCoroutine(SeekRoutine());}
                       },
            // Animation
            delegate {
                _animatorController.PlayRunAnimation(false);
                _animatorController.PlayWalkAnimation(true);
            }
        );

        BehaviorTask Task_startForgetTarget = new BehaviorTask();
        Task_startForgetTarget.Initialize("Task_startForgetTarget",Priority.Basic,
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
        TaskSeq_SeekAndForget.Initialize("TaskSeq_SeekAndForget",new List<IBehaviorNode>() { Task_seekAroundLastPosition, Task_startForgetTarget });

        BehaviorTask_DelayedFinishExec Task_attackTarget = new BehaviorTask_DelayedFinishExec();
        Task_attackTarget.Initialize("Task_attackTarget",Priority.High, (callback) => acs_attacking.StrikeFinished += callback,
            // Logic
            delegate {
                _navMeshAgent.speed = 0;
                StartCoroutine(acs_attacking.Attack());
            }
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
        MindControlTrigger_SightExit.Invoke();
    }

    private void DamagedHandle(GameObject target)
    {
        Key_target = target;
        MindControlTrigger_Damaged.Invoke();
    }

    #endregion

    #region Continuous actions routine
    
    IEnumerator FollowRoutine()
    {
        LOGGER_PARAM___FOLLOW_ROUTINE_COUNT += 1;
        while (_followFlag)
        {
            if (DistanceLessThanRadius(Key_distanceOfAttack) && Time.time - _lastStrikeTime >= Key_AttackCooldown)
            {
                _lastStrikeTime = Time.time;
                _followFlag = false;
                break;
            }

            yield return null;
        }
        MindControlTrigger_AttackTriggered.Invoke();
        LOGGER_PARAM___FOLLOW_ROUTINE_COUNT -= 1;
    }

    IEnumerator SeekRoutine() 
    {
        LOGGER_PARAM___SEEK_ROUTINE_COUNT += 1;
        while (_seekFlag)
        {
            if (DistanceLessThanRadius(Key_seekAcceptanceRange))
            {
                MindControlTrigger_NewSeekDestination.Invoke();
            }

            yield return null;
        }
        LOGGER_PARAM___SEEK_ROUTINE_COUNT -= 1;
    }
    #endregion



    #region Utils
    private bool DistanceLessThanRadius(float acceptanceRadius)
    {
        return Vector3.Distance(transform.position, _navMeshAgent.destination) < acceptanceRadius;
    }

    private void StopMovement()
    {
        _navMeshAgent.speed = 0f;

        _animatorController.PlayRunAnimation(false);
        _animatorController.PlayWalkAnimation(false);
    }

    /// <summary>
    /// 0: _followFlag <br/>
    /// 1: _seekFlag <br/>
    /// </summary>
    private void SetFlag(int flagIdx)
    {
        List<string> flags = new List<string>() { "_followFlag", "_seekFlag" };
        _followFlag = flagIdx == flags.IndexOf("_followFlag");
        _seekFlag = flagIdx == flags.IndexOf("_seekFlag");
    }
    #endregion

}
