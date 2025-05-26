using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossState_SpecialAttack : Abs_State_Boss
{
    private float cooldownAfterCast = 1.5f;
    private float cooldownAfterCastTimer;

    
    protected override IEnumerator StateFixedUpdate()
    {
        Debug.Log("BossState_SpecialAttack");
        while (InWork)
        {
            if (Time.time - cooldownAfterCastTimer > cooldownAfterCast) StateMachine.TryChangeState();
            yield return new WaitForFixedUpdate();
        }
        
    }

    public override void Enter(StateMachine_Boss stateMachine)
    {
        cooldownAfterCastTimer = Time.time;

                
        Struct_DamageData dd = new Struct_DamageData();
        dd.DamageAmount = 10;
        dd.DamageType = Enum_DamageTypes.Fire;
        dd.isBlockable = false;
        dd.isInnevitable = false;
        dd.Responce = Enum_DamageResponses.NoResponse;
        
        
        base.Enter(stateMachine);
        BossController owner = StateMachine.GetMachineOwner();
        GameObject magic = Instantiate(owner.MagicPrefab);
        magic.transform.position = owner.transform.position + (owner.transform.forward + Vector3.up)*1.5f;
        
        magic.GetComponent<MagicProjectile_Base>().Initialize(owner.gameObject, 1, 12,
            owner.transform.forward, 10, dd, "");
        
    }
}
