using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Abs_State : MonoBehaviour
{
    protected IStateMachine StateMachine;
    protected bool InWork;

    protected virtual IEnumerator StateUpdate()
    {
        while (InWork)
        {
            
            yield return new WaitForEndOfFrame();
        }
    }

    protected virtual IEnumerator StateFixedUpdate()
    {
        while (InWork)
        {
            
            yield return new WaitForNextFrameUnit();
        }
    }

    public virtual void Enter(IStateMachine stateMachine)
    {
        StateMachine = stateMachine;
        InWork = true;
        StartCoroutine(StateUpdate());
        StartCoroutine(StateFixedUpdate());
    }

    public virtual void Exit()
    {
        InWork = false;
    }
}
