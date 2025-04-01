using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour, IMovable
{
    protected Transform _target;
    protected float _walkSpeed = 2f;
    protected float _runSpeed = 5f;
    [SerializeField] protected float _attackRange = 2f;

    protected NavMeshAgent _agent;
    private IAnimatorController _animatorController;
    internal IAnimatorController AnimatorController => _animatorController;

    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animatorController = GetComponent<AnimatorController>();
    }

    public virtual void GoToTarget(Transform target, float speed)
    {
        if (_target != null)
        {
            transform.LookAt(_target.position);
        }

        if (_agent == null || target == null) return;

        _target = target;
        _agent.speed = speed;
        _agent.SetDestination(target.position);

        if (_animatorController != null)
        {
            if (speed >= _runSpeed)
            {
                _animatorController.PlayRunAnimation(true);
                _animatorController.PlayWalkAnimation(false);
                _animatorController.SetFloatToAnimation(0, _runSpeed);
            }
            else
            {
                _animatorController.PlayWalkAnimation(true);
                _animatorController.PlayRunAnimation(false);
                _animatorController.SetFloatToAnimation(0, _walkSpeed);
            }
        }
    }

    public virtual void StopMoving()
    {
        if (_agent == null) return;

        _agent.isStopped = true;
        _agent.ResetPath();

        if (_animatorController != null)
        {
            _animatorController.PlayWalkAnimation(false);
            _animatorController.PlayRunAnimation(false);
        }
    }
}
