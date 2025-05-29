using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Abs_State_Boss : MonoBehaviour
{
    protected StateMachine_Boss StateMachine;
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

    public virtual void Enter(StateMachine_Boss stateMachine)
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
