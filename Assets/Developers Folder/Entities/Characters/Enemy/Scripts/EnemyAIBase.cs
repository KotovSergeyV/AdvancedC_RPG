using System.Linq;
using UnityEngine;

public abstract class EnemyAIBase : Movable
{
    [SerializeField] protected float _spotRange;
    protected Transform[] _waypoints;
    protected int _currentWaypointIndex = 0;
    protected AI_States _currentState;
    protected ManagerSFX _managerSFX;
    protected bool isFriendly;
    protected Transform PlayerTarget;


    [SerializeField] protected AudioClip[] _takeDamageClips;
    [SerializeField] protected AudioClip[] _spoteClips;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Initialize(ManagerSFX managerSFX) { _managerSFX = managerSFX; }

    protected virtual void Start()
    {
        _currentState = AI_States.Patrolling;
        Patrol();
        FindWaypoints();
        PlayerTarget = GameObject.FindGameObjectWithTag("Player").transform;
    }

    /// <summary>
    /// Find all waypoint with tag "Waypoints"
    /// </summary>
    private void FindWaypoints()
    {
        GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("Waypoint");
        _waypoints = waypointObjects.Select(obj => obj.transform).ToArray();
    }

    /// <summary>
    /// State to walk on all Waypoints
    /// </summary>
    protected virtual void Patrol()
    {
        if (_waypoints == null || _waypoints.Length == 0) return;

        Transform nextWaypoint = _waypoints[_currentWaypointIndex];
        base.GoToTarget(nextWaypoint, _walkSpeed);
    }

    protected void UpdatePatrol()
    {
        if (_waypoints == null || _waypoints.Length == 0) return;

        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
            Patrol();
        }
    }

    protected virtual void Attack()
    {
        ;
    }

    protected virtual void Update()
    {
        if (_currentState == AI_States.Patrolling)
        {
            UpdatePatrol();
        }

        int currentHp;
        int maxHp;
        if (TryGetComponent<EntityCoreSystem>(out EntityCoreSystem core)) {
            currentHp = core.GetHealthSystem().GetHp();
            maxHp = core.GetHealthSystem().GetMaxHp();
            if ((currentHp <= maxHp / 2) && isFriendly)
            {
                _currentState = AI_States.Fallback;
                //AnimatorController.PlayDeathAnimation(_dead);
            }
        }

        bool _dead;
        if (TryGetComponent<EntityCoreSystem>(out core)) {
            _dead = core.GetHealthSystem().GetIsDead();
            if (_dead)
            {
                _currentState = AI_States.Dead;
                //AnimatorController.PlayDeathAnimation(_dead);
            }
        }
    }

    protected void LookForTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= _spotRange)
        {
            _managerSFX.PlaySFX(_spoteClips?[Random.Range(0, _spoteClips.Length)], transform.position, ManagerSFX.MixerGroupType.Voice, null, true, 1, 0);
            _target = player.transform;
            _currentState = AI_States.Attacking;
        }
    }

    protected virtual void Dead() 
    {
        Rigidbody _rb = GetComponent<Rigidbody>();
        Destroy(_rb);
        Collider _col = GetComponent<Collider>();
        Destroy(_col);
        this.enabled = false;
    }

    protected virtual void RunBack()
    {
        if (PlayerTarget == null) return;

        Vector3 directionAwayFromPlayer = (transform.position - PlayerTarget.position).normalized;

        Vector3 runToPosition = transform.position + directionAwayFromPlayer * 10f;

        GameObject tempTarget = new GameObject("TargetToAway");
        tempTarget.transform.position = runToPosition;

        base.GoToTarget(tempTarget.transform, _runSpeed);

        Destroy(tempTarget, 1f);

        Debug.Log("AAAAAAAAAA");
    }


}

public enum AI_States
{
    Patrolling,
    Attacking,
    Dead,
    Fallback,
    Idle
}