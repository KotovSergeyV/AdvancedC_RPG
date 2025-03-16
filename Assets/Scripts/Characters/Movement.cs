using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour, IMovable
{
    public Transform _target;
    public float _walkSpeed = 2f;
    public float _runSpeed = 5f;
    public float _attackRange = 2f;

    private NavMeshAgent _agent;
    private IAnimatorController _animatorController;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animatorController = GetComponent<IAnimatorController>();
    }

    public void GoToTarget(Transform target, float speed)
    {
        this._target = target;
        _agent.speed = speed;
        _agent.SetDestination(target.position);

        if (speed >= _runSpeed)
        {
            _animatorController.PlayRunAnimation(true);
            _animatorController.SetFloatToAnimation(0, _runSpeed);
        }
        else
        {
            _animatorController.PlayWalkAnimation(true);
            _animatorController.SetFloatToAnimation(0, _walkSpeed);
        }
    }

    void Update()
    {
        if (_target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, _target.position);

        if (distanceToTarget <= _attackRange)
        {
            _agent.isStopped = true;
            _animatorController.PlayRunAnimation(false);
            _animatorController.PlayWalkAnimation(false);
            _animatorController.SetFloatToAnimation(0, 0);

            //Изменить состаяние на атаку
        }
        else
        {
            _agent.isStopped = false;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            GoToTarget(_target, 5f);
        }
    }
}