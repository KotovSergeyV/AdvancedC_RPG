using UnityEngine;

internal interface IMovable
{
    void GoToTarget(Transform target, float speed);
}