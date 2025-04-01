using UnityEngine;

public class MeleeWeapon : WeaponComponent
{
    private void OnTriggerEnter(Collider other)
    {
        OnWeaponDamage(other.gameObject);
    }
}
