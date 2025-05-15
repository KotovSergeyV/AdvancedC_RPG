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

    private void Start()
    {
        _animatorController = gameObject.GetComponent<IAnimatorController>();
    }

    public IEnumerator Attack()
    {
        _animatorController.PlayAttackAnimationByTrigger();
        yield return new WaitForSeconds(3); //_animatorController.GetLengthOfClip());
        StrikeFinished.Invoke();
        Debug.Log("StrikeFinished.Invoke");
    }


}
