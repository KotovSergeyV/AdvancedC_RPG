using System.Collections;
using System.Threading.Tasks;
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

                
        Struct_DamageData damageData = new Struct_DamageData();
        damageData.DamageAmount = 10;
        damageData.DamageType = Enum_DamageTypes.Fire;
        damageData.isBlockable = false;
        damageData.isInnevitable = false;
        damageData.Responce = Enum_DamageResponses.NoResponse;
        
        
        base.Enter(stateMachine);
        BossController owner = StateMachine.GetMachineOwner();
        
        GetMagicInstance(owner, damageData);

        
    }

    private async void GetMagicInstance(BossController owner, Struct_DamageData damageData)
    {
        GameObject magic = await owner.MagicFactory.GetMagicAsync();
        magic.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        var magicPrj = Instantiate(magic, owner.transform.position + (owner.transform.forward + Vector3.up)*1.5f,
            owner.transform.rotation );
        magicPrj.GetComponent<MagicProjectile_Base>().Initialize(owner.gameObject, 1, 12,
            owner.transform.forward, 10, damageData);

    }
    
}
