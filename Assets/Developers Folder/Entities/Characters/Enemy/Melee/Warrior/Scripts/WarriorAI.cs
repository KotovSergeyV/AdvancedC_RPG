using UnityEngine;

public class WarriorAI : EnemyAIBase
{
    [SerializeField] private float _attackCooldown;
    private float _lastAttackTime;

    private new void Start()
    {
        base.Start();
        _lastAttackTime = -_attackCooldown;
    }


    protected override void Update()
    {
        base.Update();

        switch (_currentState)
        {
            case States.Patrolling:
                LookForTarget();
                break;
            case States.Attacking:
                transform.LookAt(_target);
                Attack();
                break;
            case States.Dead:
                Dead();
                break;
        }
    }

    /// <summary>
    /// Spot Range to find Player
    /// </summary>
    private void LookForTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= _spotRange)
        {
            _managerSFX.PlaySFX(_spoteClips?[Random.Range(0, _spoteClips.Length)], transform.position, ManagerSFX.MixerGroupType.Voice, null, true, 1, 0);
            _target = player.transform;
            _currentState = States.Attacking;
        }
    }

    protected override void Attack()
    {
        if (_target == null) return;

        float distance = Vector3.Distance(transform.position, _target.position);

        switch (distance)
        {
            case var d when d > _spotRange:
                _currentState = States.Patrolling;
                Patrol();
                break;

            case var d when d > _attackRange:
                GoToTarget(_target, _runSpeed);
                break;

            default:
                if (Time.time - _lastAttackTime >= _attackCooldown)
                {
                    Debug.Log("Attacking");
                    _lastAttackTime = Time.time;
                    AnimatorController?.PlayAttackAnimationByTrigger();
                    AnimatorController?.PlayWalkAnimation(false);
                    StopMoving();
                }
                break;
        }
    }
}
