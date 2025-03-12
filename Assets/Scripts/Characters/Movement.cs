using UnityEngine;
using UnityEngine.AI;

public class Movement : MonoBehaviour, IMovable
{
    public Transform target;
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float attackRange = 2f;

    private NavMeshAgent agent;
    private IAnimatorController animatorController;
    private bool _stop = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animatorController = GetComponent<IAnimatorController>();
    }

    public void GoToTarget(Transform target, float speed)
    {
        this.target = target;
        agent.speed = speed;
        agent.SetDestination(target.position);

        if (speed >= runSpeed)
        {
            animatorController.PlayAnimation("isRunning", true);
            animatorController.SetFloatAnimation(0, runSpeed);
        }
        else
        {
            animatorController.PlayAnimation("isWalking", true);
            animatorController.SetFloatAnimation(0, walkSpeed);
        }
    }

    void Update()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= attackRange)
        {
            agent.isStopped = true;
            animatorController.PlayAnimation("isRunning", false);
            animatorController.PlayAnimation("isWalking", false);
            animatorController.SetFloatAnimation(0, 0);
            Attack();
        }
        else
        {
            agent.isStopped = false;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            GoToTarget(target, 1f);
        }
    }

    void Attack()
    {
        animatorController.TriggerAnimation("Attack");
    }
}