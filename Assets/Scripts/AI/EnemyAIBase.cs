using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyAIBase : Movement
{
    [SerializeField] protected float _spotRange;
    protected Transform[] _waypoints;
    protected int _currentWaypointIndex = 0;
    protected States _currentState;

    protected override void Awake()
    {
        base.Awake();
        FindWaypoints();
    }

    protected virtual void Start()
    {
        _currentState = States.Patrolling;
        Patrol();
    }

    /// <summary>
    /// Ќаходит все точки патрулировани€ по тегу "Waypoint"
    /// </summary>
    private void FindWaypoints()
    {
        GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("Waypoint");
        _waypoints = waypointObjects.Select(obj => obj.transform).ToArray();
    }

    /// <summary>
    /// «апускает патрулирование между точками
    /// </summary>
    protected virtual void Patrol()
    {
        if (_waypoints == null || _waypoints.Length == 0) return;

        Transform nextWaypoint = _waypoints[_currentWaypointIndex];
        base.GoToTarget(nextWaypoint, _walkSpeed);
    }

    /// <summary>
    /// ѕровер€ет, достиг ли враг точки патрулировани€, и переключает цель
    /// </summary>
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