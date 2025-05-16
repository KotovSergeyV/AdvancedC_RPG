using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ACS_Attacking : MonoBehaviour
{
    // TaskFinishAction
    public Action StrikeFinished = delegate { };

    // Outer Components
    IAnimatorController _animatorController;
    NavMeshAgent _navMeshAgent;

    private void Start()
    {
        _animatorController = gameObject.GetComponent<IAnimatorController>();
        _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    public IEnumerator Attack(GameObject Key_targetPosition)
    {
        float prevSpeed = _navMeshAgent.speed;
        _navMeshAgent.speed = 0;
        transform.LookAt(Key_targetPosition.transform.position);
        _animatorController.PlayAttackAnimationByTrigger();
        yield return new WaitForSeconds(3); //_animatorController.GetLengthOfClip());
        _navMeshAgent.speed = prevSpeed;
        StrikeFinished.Invoke();

        Debug.Log("StrikeFinished.Invoke");
    }


}
