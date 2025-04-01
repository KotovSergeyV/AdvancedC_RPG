using System;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private bool _gravityAffected;
    [SerializeField] private bool _isHoming;
    [SerializeField] private float _turningSpeed;
    [SerializeField] private Transform _target;

    private Rigidbody _rb;

    public Action OnHit { get; private set; }


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = _gravityAffected;
    }

    private void FixedUpdate()
    {
        if (_isHoming && _target != null)
        {
            Vector3 directionToTarget = (_target.position - transform.position).normalized;
            _rb.linearVelocity = Vector3.RotateTowards(_rb.linearVelocity, directionToTarget, _turningSpeed * Time.deltaTime, 0f);
        }
        else
        {
            _rb.linearVelocity = transform.forward * _speed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnHit.Invoke();
    }

    public void SetDirection(Vector3 direction)
    {
        transform.forward = direction;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        _isHoming = true;
    }
}