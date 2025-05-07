using System.Collections;
using UnityEngine;

public class DEBUG_AttackSphere : MonoBehaviour
{

    private void FixedUpdate()
    {
        transform.position += transform.forward * 2 * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collider)
    {
        Struct_DamageData damageData = new Struct_DamageData();

        damageData.DamageAmount = 20;
        damageData.Responce = Enum_DamageResponses.Stun;
        damageData.DamageType = Enum_DamageTypes.Physic;
        damageData.isInneviåtable = false;
        damageData.isBlockable = false;

        //gameObject.GetComponent<IDamageCalculationSystem>().Damage(gameobject, collider.gameObject, damageData);
        Destroy(gameObject);
    }
}
