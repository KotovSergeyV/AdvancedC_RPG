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
    public Action<GameObject> ViewFieldExited = delegate { };
   

    void Start()
    {
        _targetList.Add(GameObject.FindGameObjectWithTag("Player"));
        _viewTrasnsform = gameObject.transform.position + _viewOffset;
    }

    void Update()
    {
        foreach (var target in _targetList) 
        {
            if (!_canSee && Vector3.Distance(_viewTrasnsform, target.transform.position) <= _distance &&
                Vector3.Angle(transform.forward, target.transform.position) <= _viewAngle)
            {
                Debug.DrawLine(transform.position, target.transform.position);
                Debug.DrawLine(transform.position, transform.position +  transform.forward * 100);
                Debug.Log("See1!");
                _canSee = true;
                ViewFieldEntered.Invoke(target);
            }
            else if (_canSee && (Vector3.Distance(_viewTrasnsform, target.transform.position) > _distance ||
                Vector3.Angle(transform.forward, target.transform.position) > _viewAngle))
            {

                _canSee = false;
                Debug.Log("Unsee!");
                ViewFieldExited.Invoke(target);
            }
        }
    }
}
