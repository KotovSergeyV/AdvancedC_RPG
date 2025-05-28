using UnityEngine;
using System;
using System.Collections.Generic;


public class BossController : MonoBehaviour, IEntity
{
    private StateMachine_Boss _stateMachine;
    private Transform _target;
    
    private float _viewRange = 20f;
    private float _attackRange = 2.5f;
    private float _specialCooldownTime = 10f;
    private float _specialCooldownTimer = 0;

    public MagicProjectileFactory MagicFactory;

    public ManagerSFX ManagerSFX {get; private set; }

    public void Initialize(ManagerSFX managerSFX)
    {
        ManagerSFX =  managerSFX;
    }
    
    
    void Start()
    {
        _target = GameObject.FindGameObjectWithTag("Player").transform;

        var idleState = gameObject.AddComponent<BossState_Idle>();
        var followState = gameObject.AddComponent<BossState_Follow>();
        var attackState = gameObject.AddComponent<BossState_Attack>();
        var specialAttackState = gameObject.AddComponent<BossState_SpecialAttack>();

        #region Transitions (Normal)


        var stateTransitions = new Dictionary<Abs_State_Boss, List<(Func<Abs_State_Boss, bool> condition,
            Func<Abs_State_Boss, Abs_State_Boss> transition)>>()
        {
            {
                idleState,
                new List<(Func<Abs_State_Boss, bool> condition, Func<Abs_State_Boss, Abs_State_Boss> transition)>
                {
                    (state => CanSpecialAttack(), state => specialAttackState),
                    (state => InAttackRange(), state => attackState),
                    (state => InFollowRange(), state => followState)
                }
            },
            {
                followState,
                new List<(Func<Abs_State_Boss, bool> condition, Func<Abs_State_Boss, Abs_State_Boss> transition)>
                {
                    (state => CanSpecialAttack(), state => specialAttackState),
                    (state => InAttackRange(), state => attackState),
                    (state => OutOfFollowRange(), state => idleState)
                }
            },
            {
                attackState,
                new List<(Func<Abs_State_Boss, bool> condition, Func<Abs_State_Boss, Abs_State_Boss> transition)>
                {
                    (state => CanSpecialAttack(), state => specialAttackState),
                    (state => OutOfFollowRange(), state => idleState),
                    (state => OutOfAttackRange(), state => followState),
                }
            },
            {
                specialAttackState,
                new List<(Func<Abs_State_Boss, bool> condition, Func<Abs_State_Boss, Abs_State_Boss> transition)>
                {
                    (state => InAttackRange(), state => attackState),
                    (state => InFollowRange(), state => followState),
                    (state => OutOfFollowRange(), state => idleState)
                }
            },
        };

        #endregion

        #region Transitions (Peaceful)
        var stateTransitions_Peaceful = new Dictionary<Abs_State_Boss, List<(Func<Abs_State_Boss, bool> condition,
            Func<Abs_State_Boss, Abs_State_Boss> transition)>>()
        {
            {
                idleState,
                new List<(Func<Abs_State_Boss, bool> condition, Func<Abs_State_Boss, Abs_State_Boss> transition)>
                {
                }
            },
        };        
        #endregion
        
        if (!FindFirstObjectByType<Peacemode>().PeacefulMode)
            CreateMachine(stateTransitions);
        else
        {
            GetComponent<EntityCoreSystem>().GetHealthSystem().OnDamaged +=
                delegate { CreateMachine(stateTransitions); };
            CreateMachine(stateTransitions_Peaceful);
            
        }
    }

    void CreateMachine(Dictionary<Abs_State_Boss, List<(Func<Abs_State_Boss, bool> condition,
        Func<Abs_State_Boss, Abs_State_Boss> transition)>> stateTransitions)
    {
        if (_stateMachine != null) _stateMachine.Delete();
        _stateMachine = new StateMachine_Boss(this, stateTransitions);
    }

    #region Sensors
    private bool InFollowRange()
    {
        return Vector3.Distance(transform.position, _target.position) < _viewRange;
    }
    private bool OutOfFollowRange()
    {
        return !InFollowRange();
    }
    
    private bool InAttackRange()
    {
        return Vector3.Distance(transform.position, _target.position) < _attackRange;
    }

    private bool OutOfAttackRange()
    {
        return !InAttackRange();
    }

    private bool CanSpecialAttack()
    {
        if (Time.time- _specialCooldownTimer >= _specialCooldownTime && InFollowRange())
        {
            _specialCooldownTimer = Time.time;
            return true;
        }
        return false;
    }

    #endregion


    public Transform GetTarget()
    {
        return _target;
    }



}