using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorSense_Sight : BehaviorSense
{

    [SerializeField] bool _canSee;

    [SerializeField] float _distance;
    [SerializeField] float _viewAngle;
    [SerializeField] Vector3 _viewOffset;

    Vector3 _viewTrasnsform;

    [SerializeField] List<GameObject> _targetList;

    public Action<GameObject> ViewFieldEntered = delegate { };
    public Action ViewFieldExited = delegate { };
   

    void Start()
    {
        _targetList.Add(GameObject.FindGameObjectWithTag("Player"));
        _canSee = false;
    }

    void Update()
    {
        foreach (var target in _targetList)
        {
            _viewTrasnsform = gameObject.transform.position + _viewOffset;
            Vector3 raycastTarget = target.transform.position - _viewTrasnsform;

            LayerMask layerMask = LayerMask.GetMask("Default");

            RaycastHit hit;
            Physics.Raycast(_viewTrasnsform, raycastTarget, out hit, _distance, layerMask);
            Debug.DrawRay(_viewTrasnsform, raycastTarget, new Color(255, 0, 0));

            if (!_canSee && hit.collider != null && hit.collider.gameObject == target &&
                Vector3.Angle(transform.forward, raycastTarget) <= _viewAngle)
            {
                Debug.Log("See!");
                _canSee = true;
                ViewFieldEntered.Invoke(target);
            }

            else if (_canSee && (hit.collider == null || !hit.collider.gameObject == target ||
                Vector3.Angle(transform.forward, raycastTarget) > _viewAngle))
            {

                _canSee = false;
                Debug.Log("Unsee!");
                ViewFieldExited.Invoke();
            }
        }
    }

    public bool InViewField() { return _canSee; }
}
