using UnityEngine;

internal interface IMovable
{
    void GoToTarget(Transform target, float speed);
}

internal interface IPlayerControlled : IMovable
{
    void Move(Vector2 direction);
}