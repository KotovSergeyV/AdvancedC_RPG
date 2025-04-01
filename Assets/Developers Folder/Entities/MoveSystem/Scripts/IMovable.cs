using UnityEngine;

public interface IMovable
{
    void GoToTarget(Transform target, float speed);
}

public interface IPlayerControlled : IMovable
{
    void Move(Vector2 direction);
}