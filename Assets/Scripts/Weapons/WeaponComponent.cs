using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    /*
    [SerializeField] private int _attack;
    [SerializeField] private Enum_DamageTypes _damageType;
    [SerializeField] private Enum_DamageResponses _damageResponce;
    [SerializeField] private bool isBlockable;
    [SerializeField] private bool isInneviåtable;
    */
    [SerializeField] private int _physicLayer;
    [SerializeField] private GameObject _rootOwner;


    [SerializeField] private Struct_DamageData _weaponDamageData;

    private void Start()
    {
        _rootOwner = gameObject.transform.root.gameObject;
        _physicLayer = _rootOwner.layer;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer != _physicLayer) 
        {

            if (_rootOwner.TryGetComponent<I_Stat>(out I_Stat stats))
            {
                _rootOwner.GetComponent<DamageDeallerComponent>()?.Damage(other.gameObject, _weaponDamageData, stats);
            }

            else {
                _rootOwner.GetComponent<DamageDeallerComponent>()?.Damage(other.gameObject, _weaponDamageData);
            }
              
        }
    }
}
