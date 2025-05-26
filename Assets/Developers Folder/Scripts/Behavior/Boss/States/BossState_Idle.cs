using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class BossState_Idle : Abs_State_Boss
{

    protected override IEnumerator StateFixedUpdate()
    {
        Debug.Log("IDLE");
        while (InWork)
        {
            StateMachine.TryChangeState();
            yield return new WaitForFixedUpdate();
        }
    }

    public override void Enter(StateMachine_Boss stateMachine)
    {
        StateMachine = stateMachine;
        InWork = true;
        StartCoroutine(StateFixedUpdate());
    }
    
}
