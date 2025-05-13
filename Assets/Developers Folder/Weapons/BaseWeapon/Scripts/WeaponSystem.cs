using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{

    [SerializeField] private int _physicLayer;
    [SerializeField] private GameObject _rootOwner;

    [SerializeField] private float _damageCooldown = 1f;
    [SerializeField] private bool _canDamage = true;


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

    private IEnumerator ReloadCanDamage()
    {
        yield return new WaitForSeconds(_damageCooldown);
        _canDamage = true;
    }

    protected void OnWeaponDamage(GameObject target)
    {
        if (target.layer != _physicLayer && _canDamage)
        {
            _canDamage = false;
            
                EntityCoreSystem coreSystem = _rootOwner.GetComponent<EntityCoreSystem>();
                coreSystem.GetDamageCalculationSystem().Damage(_rootOwner, target, _weaponDamageData, coreSystem.GetStatSystem());

            StartCoroutine(ReloadCanDamage());
            
        }
        
    }

}
