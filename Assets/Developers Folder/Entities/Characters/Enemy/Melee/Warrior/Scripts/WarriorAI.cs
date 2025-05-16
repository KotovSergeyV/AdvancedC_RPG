using UnityEngine;

public class WarriorAI : EnemyAIBase
{
    [SerializeField] private float _attackCooldown;
    private float _lastAttackTime;

    private new void Start()
    {
        base.Start();
        _lastAttackTime = -_attackCooldown;
        isFriendly = false;
    }


    protected override void Update()
    {
        base.Update();

        switch (_currentState)
        {
            case AI_States.Patrolling:
                LookForTarget();
                break;
            case AI_States.Attacking:
                transform.LookAt(_target);
                Attack();
                break;
            case AI_States.Fallback:
                RunBack();
                break;
            case AI_States.Dead:
                Dead();
                break;
        }
    }

    private new void LookForTarget()
    {
        base.LookForTarget();
    }

    protected override void Attack()
    {
        if (_target == null) return;

        float distance = Vector3.Distance(transform.position, _target.position);

        switch (distance)
        {
            case var d when d > _spotRange:
                _currentState = AI_States.Patrolling;
                Patrol();
                break;

            case var d when d > _attackRange:
                GoToTarget(_target, _runSpeed);
                break;

            default:
                if (Time.time - _lastAttackTime >= _attackCooldown)
                {
                    _lastAttackTime = Time.time;
                    AnimatorController?.PlayAttackAnimationByTrigger();
                    AnimatorController?.PlayWalkAnimation(false);
                    StopMoving();
                }
                break;
        }
    }
}
