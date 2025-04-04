using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
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
        
        if (target.layer != _physicLayer) 
        {
            Debug.Log(target);

            if (_rootOwner.TryGetComponent<I_Stat>(out I_Stat stats))
            {
                _rootOwner.GetComponent<DamageDeallerComponent>()?.Damage(target, _weaponDamageData, stats);
            }

            else
            {
                _rootOwner.GetComponent<DamageDeallerComponent>()?.Damage(target, _weaponDamageData);
            }

              
        }
    }

}
