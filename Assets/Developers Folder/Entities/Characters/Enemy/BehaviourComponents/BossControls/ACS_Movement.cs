using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ACS_Movement : MonoBehaviour
{

    // Movement Params
    float _runSpeed;
    float _walkSpeed;
    float _seekDistMutiplier;
    float _minSeekDistPercent;

    // Flags
    bool _chaseFlag;
    bool _isChasing;

    // Outer Components
    NavMeshAgent _navMeshAgent;




    public void Initialize(float walkSpeed, float runSpeed, float seekDistMutiplier, 
        float minSeekDistPercent, NavMeshAgent navAgent)
    {
        _walkSpeed = walkSpeed;
        _runSpeed = runSpeed;
        _seekDistMutiplier = seekDistMutiplier;
        _minSeekDistPercent = minSeekDistPercent;
        _navMeshAgent = navAgent;

    }

    public void Follow(GameObject Key_targetPosition)
    {
        _navMeshAgent.speed = _runSpeed;
        transform.LookAt(Key_targetPosition.transform.position);
        _chaseFlag = true;
        StartCoroutine( Chasing(Key_targetPosition));
    }
    public void StopFollow() { 
        _chaseFlag = false;
        Debug.Log("_chaseFlag is sety to " + _chaseFlag);
    }

    IEnumerator Chasing(GameObject Key_targetPosition)
    {
        if (!_isChasing)
        {
            _isChasing = true;
            while (_chaseFlag)
            {
                _navMeshAgent.SetDestination(Key_targetPosition.transform.position);
                yield return null;
            }
            _isChasing = false;
        }
    }

    public void Seek(Vector3 Key_lastTargetPosition)
    {
        _navMeshAgent.speed = _walkSpeed;

        Vector3 randomPosition;

        randomPosition = UnityEngine.Random.insideUnitSphere;
        while (randomPosition.sqrMagnitude < _minSeekDistPercent * _minSeekDistPercent) randomPosition = UnityEngine.Random.insideUnitCircle;
        randomPosition *= _seekDistMutiplier;
        randomPosition += Key_lastTargetPosition;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPosition, out hit, _seekDistMutiplier, NavMesh.AllAreas))
            _navMeshAgent.SetDestination(hit.position);

    }


}

