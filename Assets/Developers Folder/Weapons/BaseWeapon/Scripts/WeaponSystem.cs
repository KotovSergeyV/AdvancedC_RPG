using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{

    [SerializeField] private int _physicLayer;
    [SerializeField] private GameObject _rootOwner;


    [SerializeField] private Struct_DamageData _weaponDamageData;

    private void Start()
    {
        RootAssignation();
    }

    protected void RootAssignation()
    {
        _rootOwner = gameObject.transform.root.gameObject;
        _physicLayer = _rootOwner.layer;
    }

    protected void OnWeaponDamage(GameObject target)
    {
        Debug.Log("OnWeaponDamage started");
        if (target.layer != _physicLayer) 
        {
            Debug.Log(target);
            EntityCoreSystem coreSystem = _rootOwner.GetComponent<EntityCoreSystem>();
            coreSystem.GetDamageCalculationSystem().Damage(target, _weaponDamageData, coreSystem.GetStatSystem());
        }
    }

}
