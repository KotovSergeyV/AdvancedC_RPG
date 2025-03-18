using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DamageDeallerComponent : MonoBehaviour, I_DamageDealler
{
    public int Damage(GameObject target, Struct_DamageData damageData) 
    {
        if (CheckNull(target))
        {
            Debug.Log("No target"); return 0; }

        else 
        {
            I_Stat targetObject = target.GetComponent<I_Stat>();
            if (CheckNull(targetObject))
            {
                Debug.Log("No I_Stat"); 
                return 0;
               
            }

            int luck = targetObject.GetLuck();
            int defence = targetObject.GetDefence();
            int agility = targetObject.GetAgility();

            if (!damageData.isInneviåtable)
            {
                int baseDodgeChance = 5;
                float dodgeChance = (float)(baseDodgeChance + (agility * 0.1) + (luck * 0.05));

                if (dodgeChance >= Random.Range(1, 100))
                {
                    Debug.Log("Dodged");
                    return 0;
                }

                else if (target.GetComponent<I_EntityStates>().GetEntityState() == Enum_EntityStates.Blocking && damageData.isBlockable)
                {

                    SetTargetState(target.GetComponent<I_EntityStates>(), damageData.Responce);
                    Debug.Log("DamagedBlocked");
                    return ApplyDamage(target.GetComponent<I_Health>(), defence, damageData.DamageAmount * 0.1f);
                }
                
            }

            SetTargetState(target.GetComponent<I_EntityStates>(), damageData.Responce);
            Debug.Log("DamagedFull");
            return ApplyDamage(target.GetComponent<I_Health>(), defence, damageData.DamageAmount);

        }
    }

    private int ApplyDamage(I_Health target, int targetsDefence, float damage)
    {
        if (CheckNull(target)) { return 0; }

        int summarisedDamage = (int)(damage - (targetsDefence * 0.5f));
        summarisedDamage = Mathf.Max(summarisedDamage, 0); 

        return target.Damage(summarisedDamage);
    }


    private bool CheckNull(object target) { return target == null; }


    private void SetTargetState(I_EntityStates target, Enum_DamageResponses response)
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
}
