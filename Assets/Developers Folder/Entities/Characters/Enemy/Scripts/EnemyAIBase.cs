using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyAIBase : Movable
{
    [SerializeField] protected float _spotRange;
    protected Transform[] _waypoints;
    protected int _currentWaypointIndex = 0;
    protected States _currentState;


    [SerializeField] protected AudioClip[] _takeDamageClips;
    [SerializeField] protected AudioClip[] _spoteClips;

    protected override void Awake()
    {
        base.Awake();
        ManagerUI.Instance?.RegisterEnemyCanvas(gameObject);
    }

    protected virtual void Start()
    {
        _currentState = States.Patrolling;
        Patrol();
        FindWaypoints();
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

    protected abstract void Attack();

    protected virtual void Update()
    {
        if (_currentState == States.Patrolling)
        {
            UpdatePatrol();
        }

        bool _dead;
        if (TryGetComponent<EntityCoreSystem>(out EntityCoreSystem core)) {
            _dead = core.GetHealthSystem().GetIsDead();
            if (_dead)
            {
                _currentState = States.Dead;
                //AnimatorController.PlayDeathAnimation(_dead);
            }
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

    private void OnDisable()
    {
        ManagerUI.Instance?.UnregisterEnemyCanvas(this.gameObject);
    }

    private void OnDestroy()
    {
        ManagerUI.Instance?.UnregisterEnemyCanvas(this.gameObject);
    }
}

public enum States
{
    Patrolling,
    Attacking,
    Dead
}