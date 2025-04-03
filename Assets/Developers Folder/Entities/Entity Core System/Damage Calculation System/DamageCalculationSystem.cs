using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DamageCalculationSystem : IDamageCalculationSystem
{
    public int Damage(GameObject target, Struct_DamageData damageData, IStatSystem ownerStats=null) 
    {
        Debug.Log("DamageStarted");
        if (CheckNull(target))
        { 
            Debug.Log("No target");
            return 0;
        }

        if (ownerStats != null) { damageData = CalaculatePotentialDamage(ownerStats, damageData); }

        EntityCoreSystem targetCore = target.GetComponent<EntityCoreSystem>();
        if (targetCore == null)
        {
            return 0;
        }
        IStatSystem targetObject = targetCore.GetStatSystem();

        int luck = targetObject.GetLuck();
        int defence = targetObject.GetDefence();
        int agility = targetObject.GetAgility();

        targetCore.GetHealthSystem().GetHp();

        if (!damageData.isInneviåtable)
        {
            int baseDodgeChance = 5;
            float dodgeChance = (float)(baseDodgeChance + (agility * 0.1) + (luck * 0.05));

            if (dodgeChance >= Random.Range(1, 100))
            {
                Debug.Log("Dodged");
                return 0;
            }

            else if (targetCore.GetStatesSystem().GetEntityState() == Enum_EntityStates.Blocking && damageData.isBlockable)
            {
                SetTargetState(targetCore.GetStatesSystem(), damageData.Responce);
                Debug.Log("DamagedBlocked");
                return ApplyDamage(target.GetComponent<HealthSystem>(), defence, damageData.DamageAmount * 0.1f);
            }

        }

        SetTargetState(targetCore.GetStatesSystem(), damageData.Responce);
        Debug.Log("DamagedFull: "+ damageData.DamageAmount);
        return ApplyDamage(targetCore.GetHealthSystem(), defence, damageData.DamageAmount);


    }

    private int ApplyDamage(IHealthSystem target, int targetsDefence, float damage)
    {
        if (CheckNull(target)) { return 0; }

        int summarisedDamage = (int)(damage - (targetsDefence * 0.5f));
        summarisedDamage = Mathf.Max(summarisedDamage, 0);
        Debug.Log(target.Damage(summarisedDamage));
        return target.Damage(summarisedDamage);
    }


    private bool CheckNull(object target) { return target == null; }


    private void SetTargetState(IEntityStatesSystem target, Enum_DamageResponses response)
    {
        if (CheckNull(target)) { return; }
        switch (response)
        {
            case Enum_DamageResponses.SmallStun:
                target.SetEntityState(Enum_EntityStates.SmallStunned);
                break;

            case Enum_DamageResponses.Stun:
                target.SetEntityState(Enum_EntityStates.Stunned);
                break;

            default:
                break;
        }
    }

    private Struct_DamageData CalaculatePotentialDamage(IStatSystem weaponHolderStats, Struct_DamageData wDamageData)
    {
        wDamageData.DamageAmount += weaponHolderStats.GetAttack();
        return wDamageData;
    }
}
