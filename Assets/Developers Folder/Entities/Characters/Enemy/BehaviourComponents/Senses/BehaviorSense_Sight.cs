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

    public void RefreshSence()
    {
        foreach (var target in _targetList)
        {
            _viewTrasnsform = gameObject.transform.position + _viewOffset;

            bool inDistance = Vector3.Distance(_viewTrasnsform, target.transform.position) <= _distance;
            bool inViewAngle = Vector3.Angle(gameObject.transform.forward,
                target.transform.position-gameObject.transform.position) <= _viewAngle;
            if (inDistance && inViewAngle)
            {
                _canSee = true;
                ViewFieldEntered.Invoke(target);
                Debug.Log("RefreshSence ViewFieldEntered");
            }
            else if (!(inDistance || inViewAngle))
            {
                _canSee = false;
                ViewFieldExited.Invoke();
                Debug.Log("RefreshSence ViewFieldExited");
            }
        }
    }

    void Update()
    {
        foreach (var target in _targetList)
        {
            _viewTrasnsform = gameObject.transform.position + _viewOffset;
            
             bool inDistance = Vector3.Distance(_viewTrasnsform, target.transform.position) <= _distance;
             bool inViewAngle = Vector3.Angle(gameObject.transform.forward, 
                 target.transform.position - gameObject.transform.position) <= _viewAngle;

             if (!_canSee && inDistance && inViewAngle)
             {
                 _canSee = true;
                 ViewFieldEntered.Invoke(target);
             }
             else if (_canSee && (!inDistance || !inViewAngle))
             {
                 _canSee = false;
                 ViewFieldExited.Invoke();
             }
            // Vector3 raycastTarget = target.transform.position - _viewTrasnsform;
            //
            // LayerMask layerMask = LayerMask.GetMask("Default");
            //
            // RaycastHit hit;
            // Physics.Raycast(_viewTrasnsform, raycastTarget, out hit, _distance, layerMask);
            // Debug.DrawRay(_viewTrasnsform, raycastTarget, new Color(255, 0, 0));
            //
            // if (!_canSee && hit.collider != null && hit.collider.gameObject == target &&
            //     Vector3.Angle(transform.forward, raycastTarget) <= _viewAngle)
            // {
            //     Debug.Log("See!");
            //     _canSee = true;
            //     ViewFieldEntered.Invoke(target);
            // }
            //
            // else if (_canSee && (hit.collider == null || !hit.collider.gameObject == target ||
            //     Vector3.Angle(transform.forward, raycastTarget) > _viewAngle))
            // {
            //
            //     _canSee = false;
            //     Debug.Log("Unsee!");
            //     ViewFieldExited.Invoke();
            // }
        }
    }

    public bool InViewField() { return _canSee; }
}
