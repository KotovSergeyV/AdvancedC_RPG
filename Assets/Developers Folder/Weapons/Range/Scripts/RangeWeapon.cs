using System.Collections;
using UnityEngine;

public class RangeWeapon : WeaponSystem
{
    [SerializeField] float _range;

    [SerializeField] float _cooldown;
    [SerializeField] float _lastShot;

    //------------------ Debug
    private void Start()
    {
        RootAssignation();
        StartCoroutine(DEBUG_FIRE());
    }

    private IEnumerator DEBUG_FIRE()
    {
        while (true) {
            yield return new WaitForSeconds(_cooldown);
            OnFire(); 
        }
    }

    //

    public void OnFire()
    { 
        RaycastHit hit;
        if (_lastShot+_cooldown < Time.time)
        {
            _lastShot = Time.time;

            if (Physics.Raycast(transform.position, transform.right, out hit, _range)) {
                OnWeaponDamage(hit.transform.gameObject);

                Debug.DrawRay(transform.position, transform.right * _range, color: Color.red, duration: 10f);
            }
            
        }
    }
}
