using UnityEngine;

public class MeleeWeapon : WeaponSystem
{
    private void OnTriggerEnter(Collider other)
    {
        OnWeaponDamage(other.gameObject);
    }
}
