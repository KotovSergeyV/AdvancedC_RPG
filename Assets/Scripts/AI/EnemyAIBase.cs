using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyAIBase : Movement
{
    [SerializeField] protected float _spotRange;
    protected Transform[] _waypoints;
    protected int _currentWaypointIndex = 0;
    protected States _currentState;


    [Header("Звуки")]
    [SerializeField] protected AudioClip[] _takeDamageClips;
    [SerializeField] protected AudioClip[] _spoteClips;

    protected override void Awake()
    {
        base.Awake();
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
    }
}

public enum States
{
    Patrolling,
    Attacking
}